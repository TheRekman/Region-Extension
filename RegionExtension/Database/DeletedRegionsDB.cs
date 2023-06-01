using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Data;
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
                 new SqlColumn(TableInfo.CreationDate.ToString(), MySqlDbType.Date),
                 new SqlColumn(TableInfo.DeletionDate.ToString(), MySqlDbType.Date)
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
                var variablesString = string.Join(' ', _table.Columns.Select(c => c.Name));
                int i = 0;
                string format = string.Join(' ', _table.Columns.Select(c =>
                { 
                    string res = "{" + i + "}"; 
                    i++; 
                    return res;
                }));

                while (_database.Query("SELECT * FROM @0 WHERE Id=@1", _table.Name, region.ID) > 0)
                    region.ID++;
                var values = format.SFormat(
                            region.ID,
                            userDeleter.ID,
                            region.WorldID,
                            region.Name,
                            region.Area.X,
                            region.Area.Y,
                            region.Area.Width,
                            region.Area.Height,
                            region.Owner,
                            region.Z,
                            info.DateCreation,
                            DateTime.Now);
                _database.Query("INSERT INTO @0 (@1) VALUES (@2);", _table.Name, variablesString, values);
                if (_database.Query("SELECT * FROM @0", _table.Name) > _maxCount)
                    _database.Query("DELETE FROM TABLE @0 JOIN (SELECT MIN(@1) AS max_id FROM TABLE) temp WHERE @0.Id = temp.max_Id",
                                    _table.Name, TableInfo.DeletionDate.ToString());
                return true;
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
                return false;
            }
        }

        public List<string> GetRegionsInfo(int count)
        {
            try
            {
                var res = new List<string>();
                using(var reader = _database.QueryReader("SELECT @1 FROM @0", _table.Name, count))
                {
                    while (reader.Read())
                    {
                        res.Add(string.Join(' ',
                                "Name:", reader.Get<string>(TableInfo.RegionName.ToString()),
                                "User:", reader.Get<string>(TableInfo.DeleterId.ToString()),
                                "Date:", reader.Get<DateTime>(TableInfo.DeletionDate.ToString())));
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
                using (var reader = _database.QueryReader("SELECT 1 FROM @0 WHERE @1=@2", _table.Name, TableInfo.RegionName.ToString(), regionName))
                {
                    return new RegionExtended()
                    {
                        Region = new Region()
                        {
                            ID = reader.Get<int>(TableInfo.RegionId.ToString()),
                            WorldID = reader.Get<string>(TableInfo.WorldId.ToString()),
                            Name = reader.Get<string>(TableInfo.RegionName.ToString()),
                            Area = new Microsoft.Xna.Framework.Rectangle(
                                reader.Get<int>(TableInfo.X.ToString()),
                                reader.Get<int>(TableInfo.Y.ToString()),
                                reader.Get<int>(TableInfo.Width.ToString()),
                                reader.Get<int>(TableInfo.Height.ToString())
                                ),
                            AllowedIDs = reader.Get<string>(TableInfo.UserIds.ToString()).Split(' ').Select(s => int.Parse(s)).ToList(),
                            DisableBuild = reader.Get<int>(TableInfo.Protected.ToString()) == 0 ? false : true,
                            AllowedGroups = reader.Get<string>(TableInfo.Groups.ToString()).Split(' ').ToList(),
                            Owner = reader.Get<string>(TableInfo.Owner.ToString()),
                            Z = reader.Get<int>(TableInfo.Z.ToString())
                        },
                        ExtensionInfo = new RegionExtensionInfo(
                            reader.Get<int>(TableInfo.RegionId.ToString()),
                            TShock.UserAccounts.GetUserAccountByName(reader.Get<string>(TableInfo.Owner.ToString())).ID,
                            reader.Get<DateTime>(TableInfo.CreationDate.ToString()),
                            DateTime.Now,
                            DateTime.Now
                            )
                    };
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
