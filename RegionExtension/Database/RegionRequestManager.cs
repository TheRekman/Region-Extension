using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;
using TShockAPI;
using Terraria;
using Google.Protobuf.WellKnownTypes;
using MySqlX.XDevAPI.Relational;

namespace RegionExtension.Database
{
    public class RegionRequestManager
    {
        private IDbConnection _database;

        private SqlTable _table =
            new SqlTable("ExtendedRegions",
                         new SqlColumn(TableInfo.RegionId.ToString(), MySqlDbType.Int32),
                         new SqlColumn(TableInfo.WorldID.ToString(), MySqlDbType.Text),
                         new SqlColumn(TableInfo.UserID.ToString(), MySqlDbType.Int32),
                         new SqlColumn(TableInfo.DateCreation.ToString(), MySqlDbType.Text),
                         new SqlColumn(TableInfo.Denied.ToString(), MySqlDbType.Int32),
                         new SqlColumn(TableInfo.Denier.ToString(), MySqlDbType.Int32)
                         );

        private List<Request> _requests = new List<Request>();
        public List<Request> Requests { get { return _requests; } }

        public RegionRequestManager(IDbConnection db)
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

        public bool AddRequest(Region region, UserAccount user)
        {
            try
            {
                var variablesString = string.Join(", ", _table.Columns.Select(c => c.Name));
                var values = "'" + string.Join("', '",
                            region.ID, Main.worldID, user.ID, DateTime.UtcNow.ToString(), 0, 0) + "'";
                _database.Query($"INSERT INTO {_table.Name} ({variablesString}) VALUES ({values});");
                Requests.Add(new Request(region, user, null, false, DateTime.UtcNow));
                return true;
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
                return false;
            }
        }

        public bool DenyRequest(Region region, UserAccount user)
        {
            var req = Requests.FirstOrDefault(r => r.Region.ID == region.ID);
            if (req == null ||
                !UpdateQuery(_database, _table.Name, TableInfo.Denier.ToString(), user.ID.ToString(), region.ID) ||
                !UpdateQuery(_database, _table.Name, TableInfo.Denied.ToString(), "1", region.ID))
                return false;
            req.Denier = user;
            req.Denied = true;
            return true;
        }

        private bool UpdateQuery(IDbConnection db, string table, string column, string value, int RegionId)
        {
            try
            {
                db.Query($"UPDATE {table} SET {column}='{value}' WHERE RegionId={RegionId}");
                return true;
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
                return false;
            }
        }

        public void LoadInfo()
        {
            try
            {
                using (var reader = _database.QueryReader($"SELECT * FROM {_table.Name} WHERE WorldID=@0", Main.worldID.ToString()))
                {
                    while (reader.Read())
                    {
                        Region region = TShock.Regions.GetRegionByID(reader.Get<int>(TableInfo.RegionId.ToString()));
                        UserAccount user = TShock.UserAccounts.GetUserAccountByID(reader.Get<int>(TableInfo.UserID.ToString()));
                        if (region == null || user == null)
                            continue;
                        UserAccount Denier = TShock.UserAccounts.GetUserAccountByID(reader.Get<int>(TableInfo.UserID.ToString()));
                        DateTime date = DateTime.Parse(reader.Get<string>(TableInfo.DateCreation.ToString()));
                        _requests.Add(new Request(region, user, Denier, Denier != null, date));
                    }
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
            }
        }

        public bool DeleteRequest(Region region)
        {
            try
            {
                _database.Query($"DELETE FROM {_table.Name} WHERE RegionId=@0", region.ID);
                return true;
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
                return false;
            }
        }

        enum TableInfo
        {
            RegionId,
            WorldID,
            UserID,
            DateCreation,
            Denied,
            Denier
        }
    }

    public class Request
    {
        public Request(Region region, UserAccount user, UserAccount denier, bool denied, DateTime dateCreation)
        {
            Region = region;
            User = user;
            Denier = denier;
            Denied = denied;
            DateCreation = dateCreation;
        }

        public Region Region { get; set; }
        public UserAccount User { get; set; }
        public UserAccount Denier { get; set; }
        public bool Denied { get; set; }
        public DateTime DateCreation { get; set; }
    }
}
