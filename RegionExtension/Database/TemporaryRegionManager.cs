using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TShockAPI;
using TShockAPI.DB;
using Terraria;

namespace RegionExtension.Database
{
    public class TemporaryRegionManager
    {
        private IDbConnection _database;

        private SqlTable _table =
            new SqlTable("ExtendedRegions",
                         new SqlColumn(TableInfo.RegionId.ToString(), MySqlDbType.Int32),
                         new SqlColumn(TableInfo.WorldID.ToString(), MySqlDbType.Int32),
                         new SqlColumn(TableInfo.DeletionDate.ToString(), MySqlDbType.Text)
                         );

        private List<TemporaryRegion> _temporaryRegions = new List<TemporaryRegion>();
        private List<TemporaryRegion> TemporaryRegions { get { return _temporaryRegions; } }

        public TemporaryRegionManager(IDbConnection db)
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

        public bool DefineTemporary(Region region, DateTime time)
        {
            try
            {
                var variablesString = string.Join(", ", _table.Columns.Select(c => c.Name));
                var values = "'" + string.Join("', '",
                            region.ID, Main.worldID, time.ToString()) + "'";
                _database.Query($"INSERT INTO {_table.Name} ({variablesString}) VALUES ({values});");
                _temporaryRegions.Add(new TemporaryRegion(region, time));
                return true;
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
                return false;
            }
        }

        public List<TemporaryRegion> GetRegionsAndRemove()
        {
            var regionForDelete = _temporaryRegions.Where(r => r.dateTime < DateTime.UtcNow);
            _temporaryRegions.RemoveAll(r => r.dateTime < DateTime.UtcNow);
            foreach (var r in regionForDelete)
                DeleteTemporary(r.Region, r.dateTime);
            return regionForDelete.ToList();
        }

        public bool UpdateTime(Region region, DateTime time)
        {
            var res = UpdateQuery(_database, _table.Name, TableInfo.DeletionDate.ToString(), time.ToString(), region.ID);
            if(res)
                _temporaryRegions.First(r => r.Region.ID == region.ID).dateTime = time;
            return res;
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
                        if (region == null)
                            continue;
                        DateTime date = DateTime.Parse(reader.Get<string>(TableInfo.DeletionDate.ToString()));
                        _temporaryRegions.Add(new TemporaryRegion(region, date));
                    }
                }
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
            }
        }

        public bool DeleteTemporary(Region region, DateTime time)
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
            DeletionDate
        }
    }

    public class TemporaryRegion
    {
        public TemporaryRegion(Region region, DateTime dateTime)
        {
            Region = region;
            this.dateTime = dateTime;
        }

        public Region Region { get; set; }
        public DateTime dateTime { get; set; }
    }
}
