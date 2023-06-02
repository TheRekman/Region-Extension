using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Terraria;
using MySql.Data.MySqlClient;
using TShockAPI;
using TShockAPI.DB;
using System.Data.SqlTypes;

namespace RegionExtension.Database
{
    public class RegionInfoManager
    {
        private IDbConnection _database;

        private SqlTable _table =
            new SqlTable("ExtendedRegions",
                         new SqlColumn(TableInfo.Id.ToString(), MySqlDbType.Int32) { Unique = true, NotNull = true },
                         new SqlColumn(TableInfo.WorldId.ToString(), MySqlDbType.Int32),
                         new SqlColumn(TableInfo.DateCreation.ToString(), MySqlDbType.DateTime) { DefaultCurrentTimestamp = true },
                         new SqlColumn(TableInfo.LastUser.ToString(), MySqlDbType.Int32),
                         new SqlColumn(TableInfo.LastUpdate.ToString(), MySqlDbType.DateTime) { DefaultCurrentTimestamp = true },
                         new SqlColumn(TableInfo.LastActivity.ToString(), MySqlDbType.DateTime) { DefaultCurrentTimestamp = true }
                         );

        public List<RegionExtensionInfo> RegionsInfo { get; private set; } = new List<RegionExtensionInfo>();

        public RegionInfoManager(IDbConnection db)
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

        public void PostInitialize()
        {
            LoadRegions();
        }

        private void LoadRegions()
        {
            try
            {
                using(var reader = _database.QueryReader($"SELECT * FROM {_table.Name} WHERE WorldId=@0", Main.worldID.ToString()))
                {
                    while(reader.Read())
                    {
                        RegionsInfo.Add(new RegionExtensionInfo(
                                            reader.Get<int>(TableInfo.Id.ToString()),
                                            reader.Get<int>(TableInfo.LastUser.ToString()),
                                            reader.Get<DateTime>(TableInfo.DateCreation.ToString()),
                                            reader.Get<DateTime>(TableInfo.LastUpdate.ToString()),
                                            reader.Get<DateTime>(TableInfo.LastActivity.ToString())));
                    }
                }
                foreach(var region in TShock.Regions.Regions)
                {
                    if(!RegionsInfo.Any(r => r.Id == region.ID))
                    {
                        AddNewRegion(region.ID, TShock.UserAccounts.GetUserAccountByName(region.Owner).ID);
                    }
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
            }
        }

        public bool AddNewRegion(int id, int userId)
        {
            try
            {
                var reader = _database.QueryReader($"SELECT 1 FROM {_table.Name} WHERE Id=@0", id);
                if (reader.Read() == true)
                    return true;
                
                var variablesString = string.Join(", ", _table.Columns.Select(c => c.Name));
                var sqlDateTime = new SqlDateTime(DateTime.UtcNow).ToSqlString();
                var values = "'" + string.Join("', '",
                             id.ToString(), Main.worldID, DateTime.UtcNow.ToString(), userId.ToString(), DateTime.UtcNow.ToString(), DateTime.UtcNow.ToString()) + "'";
                _database.Query($"INSERT INTO {_table.Name} ({variablesString}) VALUES ({values});");
                RegionsInfo.Add(new RegionExtensionInfo(id, userId));
                return true;
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
                return false;
            }
        }

        public bool RemoveRegion(int id)
        {
            try
            {
                _database.Query($"DELETE FROM {_table.Name} WHERE Id=@0", id);
                return true;
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
                return false;
            }
        }

        public bool UpdateQuery(IDbConnection db, string table, string collumn, string value, int id)
        {
            try
            {
                db.Query($"UPDATE {table} SET {collumn}={value} WHERE Id={id}");
                return true;
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
                return false;
            }
        }

        public bool UpdateLastUser(int id, int userId)
        {
            RegionsInfo.First(r => r.Id == id).LastUserId = userId;
            return UpdateQuery(_database, _table.Name, TableInfo.LastUser.ToString(), userId.ToString(), id);
            
        }

        public bool UpdateLastUser(Region region, UserAccount userId) =>
            UpdateLastUser(region.ID, userId.ID);

        public bool UpdateLastUpdate(int id, DateTime time)
        {
            RegionsInfo.First(r => r.Id == id).LastUpdate = time;
            return UpdateQuery(_database, _table.Name, TableInfo.LastUpdate.ToString(), time.ToString(), id);
        }

        public bool UpdateLastUpdate(Region region, DateTime time) =>
            UpdateLastUpdate(region.ID, time);

        public bool UpdateLastActivity(int id, DateTime time)
        {
            RegionsInfo.First(r => r.Id == id).LastActivity = time;
            return UpdateQuery(_database, _table.Name, TableInfo.LastActivity.ToString(), time.ToString(), id);
        }

        public bool UpdateLastActivity(Region region, DateTime time) =>
            UpdateLastActivity(region.ID, time);

        public List<string> GetRegionInfo(int id, bool insertDefaultInfo = true)
        {
            var region = TShock.Regions.GetRegionByID(id);
            if (region == null)
                return null;
            var lines = new List<string>
            {
                string.Format("X: {0}; Y: {1}; W: {2}; H: {3}, Z: {4}", region.Area.X, region.Area.Y, region.Area.Width, region.Area.Height, region.Z),
                string.Concat("Owner: ", region.Owner),
                string.Concat("Protected: ", region.DisableBuild.ToString()),
            };
            if (region.AllowedIDs.Count > 0)
            {
                IEnumerable<string> sharedUsersSelector = region.AllowedIDs.Select(userId =>
                {
                    UserAccount user = TShock.UserAccounts.GetUserAccountByID(userId);
                    if (user != null)
                        return user.Name;

                    return string.Concat("{ID: ", userId, "}");
                });
                List<string> extraLines = PaginationTools.BuildLinesFromTerms(sharedUsersSelector.Distinct());
                extraLines[0] = "Shared with: " + extraLines[0];
                lines.AddRange(extraLines);
            }
            else
            {
                lines.Add("Region is not shared with any users.");
            }

            if (region.AllowedGroups.Count > 0)
            {
                List<string> extraLines = PaginationTools.BuildLinesFromTerms(region.AllowedGroups.Distinct());
                extraLines[0] = "Shared with groups: " + extraLines[0];
                lines.AddRange(extraLines);
            }
            else
            {
                lines.Add("Region is not shared with any groups.");
            }
            var extInfo = RegionsInfo.First(ri => ri.Id == id);
            var userName = extInfo.LastUserId == 0 ? "Server" : TShock.UserAccounts.GetUserAccountByID(extInfo.LastUserId).Name;
            lines.Add(string.Concat("Last user: ", userName));
            lines.Add(string.Concat("Last update: ", extInfo.LastUpdate.ToString(Utils.DateFormat)));
            lines.Add(string.Concat("Last activity: ", extInfo.LastActivity.ToString(Utils.DateFormat)));
            lines.Add(string.Concat("Date creation: ", extInfo.DateCreation.ToString(Utils.DateFormat)));
            return lines;
        }

        private enum TableInfo
        {
            Id,
            WorldId,
            DateCreation,
            LastUser,
            LastUpdate,
            LastActivity
        }

    }

    public class RegionExtensionInfo
    {
        public RegionExtensionInfo(int id, int lastUser) :
            this(id, lastUser, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow)
        {
            Id = id;
            LastUserId = lastUser;
        }

        public RegionExtensionInfo(int id, int lastUserId, DateTime dateCreation, DateTime lastUpdate, DateTime lastActivity)
        {
            Id = id;
            DateCreation = dateCreation;
            LastUserId = lastUserId;
            LastUpdate = lastUpdate;
            LastActivity = lastActivity;
        }

        public int Id { get; set; }
        public int LastUserId { get; set; }
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
        public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    }
}