using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Database
{
    public class RegionInfoManager
    {
        private IDbConnection _database;

        private SqlTable _table =
            new SqlTable("ExtendedRegions",
                         new SqlColumn(TableInfo.Id.ToString(), MySqlDbType.Int32) { Unique = true, NotNull = true },
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
            LoadRegions();
        }

        public void InitializeTable()
        {
            IQueryBuilder queryCreator = _database.GetSqlType() == SqlType.Sqlite ?
                                            new SqliteQueryCreator() : new MysqlQueryCreator();
            var creator = new SqlTableCreator(_database, queryCreator);
            creator.EnsureTableStructure(_table);
        }

        private void LoadRegions()
        {
            try
            {
                foreach(var region in TShock.Regions.Regions)
                {
                    var reader = _database.QueryReader("SELECT 1 FROM @0 WHERE Id=@1", _table.Name, region.ID);
                    if(!reader.Read())
                    {
                        AddNewRegion(region.ID, TShock.UserAccounts.GetUserAccountID(region.Owner));
                        continue;
                    }
                    RegionsInfo.Add(new RegionExtensionInfo(
                                        reader.Get<int>(TableInfo.Id.ToString()),
                                        reader.Get<int>(TableInfo.LastUser.ToString()),
                                        reader.Get<DateTime>(TableInfo.DateCreation.ToString()),
                                        reader.Get<DateTime>(TableInfo.LastUpdate.ToString()),
                                        reader.Get<DateTime>(TableInfo.LastActivity.ToString())));
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
                var reader = _database.QueryReader("SELECT 1 FROM @0 WHERE Id=@1", _table.Name, id);
                if (reader.Read() == true)
                    return true;
                var variablesString = string.Join(' ', _table.Columns.Select(c => c.Name));
                var values = "{0} {1} {2} {3} {4}".SFormat(
                             id, DateTime.Now, userId, DateTime.Now, DateTime.Now);
                _database.Query("INSERT INTO @0 (@1) VALUES (@2);", _table.Name, variablesString, values);
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
                _database.Query("DELETE FROM @0 WHERE Id=@1", _table.Name, id);
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
                db.Query("UPDATE @0 SET Id=@1 WHERE @2=@3", table, collumn, value, id);
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
            lines.Add(string.Concat("Last user: ", extInfo.LastUserId));
            lines.Add(string.Concat("Last update: ", extInfo.LastUpdate.ToString()));
            lines.Add(string.Concat("Last activity: ", extInfo.LastActivity.ToString()));
            lines.Add(string.Concat("Date creation: ", extInfo.DateCreation.ToString()));
            return lines;
        }

        private enum TableInfo
        {
            Id,
            DateCreation,
            LastUser,
            LastUpdate,
            LastActivity
        }

    }

    public class RegionExtensionInfo
    {
        public RegionExtensionInfo(int id, int lastUser) :
            this(id, lastUser, DateTime.Now, DateTime.Now, DateTime.Now)
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
        public DateTime DateCreation { get; set; } = DateTime.Now;
        public DateTime LastUpdate { get; set; } = DateTime.Now;
        public DateTime LastActivity { get; set; } = DateTime.Now;
    }
}