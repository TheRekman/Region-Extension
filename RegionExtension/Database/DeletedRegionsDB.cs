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
        private int _maxCount = 20;

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
                 new SqlColumn(TableInfo.CreationDate.ToString(), MySqlDbType.DateTime),
                 new SqlColumn(TableInfo.DeletionDate.ToString(), MySqlDbType.DateTime)
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
                            region.DisableBuild.ToString(),
                            string.Join(' ', region.AllowedGroups),
                            region.Owner,
                            region.Z.ToString(),
                            info.DateCreation,
                            DateTime.UtcNow);
                _database.Query($"INSERT INTO {_table.Name} ({variablesString}) VALUES ({values});");
                if (_database.Query($"SELECT * FROM {_table.Name}") > _maxCount)
                    _database.Query($"DELETE FROM TABLE {_table.Name} JOIN (SELECT MIN(@0) AS max_id FROM TABLE) temp WHERE {_table.Name}.Id = temp.max_Id",
                                    TableInfo.DeletionDate.ToString());
                return true;
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
                return false;
            }
        }

        public List<string> GetRegionsInfo()
        {
            try
            {
                var res = new List<string>();
                using(var reader = _database.QueryReader($"SELECT * FROM {_table.Name} ORDER BY DeletionDate DESC"))
                {
                    while (reader.Read())
                    {
                        var userid = reader.Get<int>(TableInfo.DeleterId.ToString());
                        var username = userid == 0 ? "Server" : TShock.UserAccounts.GetUserAccountByID(userid).Name;
                        res.Add("\"" + string.Join("\" \"",
                                reader.Get<string>(TableInfo.RegionName.ToString()),
                                username,
                                reader.Get<DateTime>(TableInfo.DeletionDate.ToString()).ToString(Utils.DateFormat)) + "\"");
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
                return null;
            }
        }

        public RegionExtended GetRegionByName(string regionName)
        {
            try
            {
                var res = new List<string>();
                using (var reader = _database.QueryReader($"SELECT * FROM {_table.Name} WHERE {TableInfo.RegionName.ToString()}='{regionName}'"))
                {
                    if (reader.Read())
                    {
                        int id = reader.Get<int>(TableInfo.RegionId.ToString());
                        var worldId = reader.Get<string>(TableInfo.WorldId.ToString());
                        var name = reader.Get<string>(TableInfo.RegionName.ToString());
                        var area = new Microsoft.Xna.Framework.Rectangle(
                                   reader.Get<int>(TableInfo.X.ToString()),
                                   reader.Get<int>(TableInfo.Y.ToString()),
                                   reader.Get<int>(TableInfo.Width.ToString()),
                                   reader.Get<int>(TableInfo.Height.ToString())
                                   );
                        var allowIdString = reader.Get<string>(TableInfo.UserIds.ToString()).Split(',');
                        var allowIds = new List<int>();
                        foreach(var str in allowIdString)
                        {
                            int n = 0;
                            if(int.TryParse(str, out n))
                                allowIds.Add(n);
                        }
                        var disableBuild = reader.Get<int>(TableInfo.Protected.ToString()) == 0 ? false : true;
                        var allowedGroups = reader.Get<string>(TableInfo.Groups.ToString()).Split(' ').ToList();
                        var owner = reader.Get<string>(TableInfo.Owner.ToString());
                        var z = reader.Get<int>(TableInfo.Z.ToString());
                        return new RegionExtended()
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
                                DateTime.Parse(reader.Get<string>(TableInfo.CreationDate.ToString())),
                                DateTime.UtcNow,
                                DateTime.UtcNow
                            )
                        };
                    }
                       
                    else
                        return null;

                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
                return null;
            }
        }

        private enum TableInfo
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
}
