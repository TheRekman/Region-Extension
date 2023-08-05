using Terraria;
using MonoMod;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace RegionExtension.Database
{
    public class DeletedRegionsDB
    {
        private IDbConnection _database;

        private List<DeletedInfo> _deletedInfo = new List<DeletedInfo>();

        private SqlTable _table =
            new SqlTable("DeletedRegions",
                 new SqlColumn(TableInfo.RegionId.ToString(), MySqlDbType.Int32) { NotNull = true },
                 new SqlColumn(TableInfo.DeleterId.ToString(), MySqlDbType.Int32),
                 new SqlColumn(TableInfo.WorldId.ToString(), MySqlDbType.Text),
                 new SqlColumn(TableInfo.RegionName.ToString(), MySqlDbType.Text),
                 new SqlColumn(TableInfo.X.ToString(), MySqlDbType.Int32),
                 new SqlColumn(TableInfo.Y.ToString(), MySqlDbType.Int32),
                 new SqlColumn(TableInfo.Width.ToString(), MySqlDbType.Int32),
                 new SqlColumn(TableInfo.Height.ToString(), MySqlDbType.Int32),
                 new SqlColumn(TableInfo.UserIds.ToString(), MySqlDbType.Text),
                 new SqlColumn(TableInfo.Protected.ToString(), MySqlDbType.Int32),
                 new SqlColumn(TableInfo.Groups.ToString(), MySqlDbType.Text),
                 new SqlColumn(TableInfo.Owner.ToString(), MySqlDbType.Text),
                 new SqlColumn(TableInfo.Z.ToString(), MySqlDbType.Int32),
                 new SqlColumn(TableInfo.CreationDate.ToString(), MySqlDbType.Text),
                 new SqlColumn(TableInfo.DeletionDate.ToString(), MySqlDbType.Text)
                 );

        public DeletedRegionsDB(IDbConnection db)
        {
            _database = db;
            InitializeTable();
        }

        public void InitializeTable()
        {
            IQueryBuilder queryCreator = _database.GetSqlType() == SqlType.Sqlite ?
                                            new SqliteQueryCreator() : new MysqlQueryCreator();
            var creator = new SqlTableCreator(_database, queryCreator);
            creator.EnsureTableStructure(_table);
            LoadRegions();
        }

        public bool RegisterDeletedRegion(Region region, UserAccount userDeleter, RegionExtensionInfo info)
        {
            try
            {
                var variablesString = string.Join(", ", _table.Columns.Select(c => c.Name));
                int i = 0;
                string format = string.Join(", ", _table.Columns.Select(c =>
                { 
                    string res = "'{" + i + "}'"; 
                    i++; 
                    return res;
                }));

                while (_database.Query($"SELECT * FROM {_table.Name} WHERE RegionId=@0", region.ID) > 0)
                    region.ID++;
                var values = format.SFormat(
                            region.ID.ToString(),
                            userDeleter.ID.ToString(),
                            region.WorldID,
                            region.Name,
                            region.Area.X.ToString(),
                            region.Area.Y.ToString(),
                            region.Area.Width.ToString(),
                            region.Area.Height.ToString(),
                            string.Join(' ', region.AllowedIDs),
                            region.DisableBuild ? 1 : 0,
                            string.Join(' ', region.AllowedGroups),
                            region.Owner,
                            region.Z.ToString(),
                            info.DateCreation,
                            DateTime.UtcNow);
                string query = $"INSERT INTO {_table.Name} (RegionId, DeleterId, WorldId, RegionName, X, Y, Width, Height, UserIds, Protected, `Groups`, Owner, Z, CreationDate, DeletionDate) VALUES ({values});";
                _database.Query(query);
                _deletedInfo.Add(new DeletedInfo(new RegionExtended() { Region = region, ExtensionInfo = info }, DateTime.UtcNow, userDeleter.Name));
                return true;
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
                return false;
            }
        }

        public bool LoadRegions()
        {
            try
            {
                using (var reader = _database.QueryReader($"SELECT * FROM {_table.Name} WHERE {TableInfo.WorldId}={Main.worldID}"))
                {
                    while (reader.Read())
                    {
                        var info = DeletedInfo.ReadFromDB(reader);
                        _deletedInfo.Add(info);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
                return false;
            }
        }

        public List<string> GetRegionsInfo() => 
            _deletedInfo.OrderBy(r => r.DeletionDate)
                               .Reverse()
                               .Select(r => string.Join(' ', r.DeletionDate.ToString(Utils.DateFormat), r.RegionName, r.DeleterUser))
                               .ToList();

        public bool RemoveRegionFromDeleted(int regionId)
        {
            try
            {
                _database.Query($"DELETE FROM {_table.Name} WHERE RegionId={regionId}");
                _deletedInfo.RemoveAll(r => r.RegionExt.Region.ID == regionId);
                return true;
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
                return false;
            }
        }

        public RegionExtended GetRegionByName(string regionName)
        {
            var reg = _deletedInfo.FirstOrDefault(r => r.RegionName == regionName);
            if (reg == null)
                return null;
            return reg.RegionExt;
        }

        public List<RegionExtended> GetRegionsByUser(UserAccount user) =>
            _deletedInfo.Where(r => r.DeleterUser == user.Name)
                        .Select(r => r.RegionExt)
                        .ToList();

        public enum TableInfo
        {
            RegionId,
            DeleterId,
            WorldId,
            RegionName,
            X,
            Y,
            Width,
            Height,
            UserIds,
            Protected,
            Groups,
            Owner,
            Z,
            CreationDate,
            DeletionDate
        }
    }
    public class DeletedInfo
    {
        public DeletedInfo(RegionExtended region, DateTime deletionDate, string deleterUser)
        {
            RegionExt = region;
            DeletionDate = deletionDate;
            DeleterUser = deleterUser;
        }

        public RegionExtended RegionExt { get; set; }
        public string RegionName { get => RegionExt.Region.Name; }
        public DateTime DeletionDate { get; set; }
        public string DeleterUser{ get; set; }

        public static DeletedInfo ReadFromDB(QueryResult reader)
        {
            int id = reader.Get<int>(DeletedRegionsDB.TableInfo.RegionId.ToString());
            var worldId = reader.Get<string>(DeletedRegionsDB.TableInfo.WorldId.ToString());
            var name = reader.Get<string>(DeletedRegionsDB.TableInfo.RegionName.ToString());
            var area = new Microsoft.Xna.Framework.Rectangle(
                        reader.Get<int>(DeletedRegionsDB.TableInfo.X.ToString()),
                        reader.Get<int>(DeletedRegionsDB.TableInfo.Y.ToString()),
                        reader.Get<int>(DeletedRegionsDB.TableInfo.Width.ToString()),
                        reader.Get<int>(DeletedRegionsDB.TableInfo.Height.ToString())
                        );
            var allowIdString = reader.Get<string>(DeletedRegionsDB.TableInfo.UserIds.ToString()).Split(',');
            var allowIds = new List<int>();
            foreach (var str in allowIdString)
            {
                int n = 0;
                if (int.TryParse(str, out n))
                    allowIds.Add(n);
            }
            var disableBuild = reader.Get<int>(DeletedRegionsDB.TableInfo.Protected.ToString()) == 1 ? true : false;
            var allowedGroups = reader.Get<string>(DeletedRegionsDB.TableInfo.Groups.ToString()).Split(' ').ToList();
            var owner = reader.Get<string>(DeletedRegionsDB.TableInfo.Owner.ToString());
            var z = reader.Get<int>(DeletedRegionsDB.TableInfo.Z.ToString());
            var deletionTime = DateTime.Parse(reader.Get<string>(DeletedRegionsDB.TableInfo.DeletionDate.ToString()));
            var userid = reader.Get<int>(DeletedRegionsDB.TableInfo.DeleterId.ToString());
            var user = TShock.UserAccounts.GetUserAccountByID(userid);
            var username = user == null ? "Server" : user.Name;
            return new DeletedInfo(new RegionExtended()
            {
                Region = new Region()
                {
                    ID = id,
                    WorldID = worldId,
                    Name = name,
                    Area = area,
                    AllowedIDs = allowIds,
                    DisableBuild = disableBuild,
                    AllowedGroups = allowedGroups,
                    Owner = owner,
                    Z = z
                },
                ExtensionInfo = new RegionExtensionInfo(
                    id,
                    TShock.UserAccounts.GetUserAccountByName(owner).ID,
                    DateTime.Parse(reader.Get<string>(DeletedRegionsDB.TableInfo.CreationDate.ToString())),
                    DateTime.UtcNow,
                    DateTime.UtcNow
                )
            }, deletionTime, username);
        }
    }
}
