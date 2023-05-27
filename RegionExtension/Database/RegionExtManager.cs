using System;
using System.Data;
using System.Collections.Generic;
using Terraria;
using TShockAPI;
using TShockAPI.DB;
using MySql.Data.MySqlClient;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Diagnostics;
using System.IO;
using TerrariaApi.Server;
using RegionExtension.Commands;
using TShockAPI.Hooks;

namespace RegionExtension.Database
{
    public class RegionExtManager
    {
        private IDbConnection _tshockDatabase;
        private RegionInfoManager _regionInfoManager;

        public RegionExtManager(IDbConnection db)
        {
            _tshockDatabase = db;
            InitializeDatabase();
        }

        public void InitializeDatabase()
        {
            IDbConnection database;
            if (TShock.Config.Settings.StorageType.ToLower() == "sqlite")
            {
                string sql = Path.Combine(TShock.SavePath, "RegionExtension.sqlite");
                Directory.CreateDirectory(Path.GetDirectoryName(sql));
                database = new Microsoft.Data.Sqlite.SqliteConnection(string.Format("Data Source={0}", sql));
            }
            else if (TShock.Config.Settings.StorageType.ToLower() == "mysql")
            {
                try
                {
                    var hostport = TShock.Config.Settings.MySqlHost.Split(':');
                    database = new MySqlConnection();
                    database.ConnectionString =
                        String.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4};",
                            hostport[0],
                            hostport.Length > 1 ? hostport[1] : "3306",
                            TShock.Config.Settings.MySqlDbName,
                            TShock.Config.Settings.MySqlUsername,
                            TShock.Config.Settings.MySqlPassword
                            );
                }
                catch (MySqlException ex)
                {
                    throw new Exception("MySql not setup correctly");
                }
            }
            else
            {
                throw new Exception("Invalid storage type");
            }
            _regionInfoManager = new RegionInfoManager(database);
        }

        public void RegisterRegionDefine(RegionHooks.RegionCreatedEventArgs args)
        {
            _regionInfoManager.AddNewRegion(args.Region.ID, TShock.UserAccounts.GetUserAccountByName(args.Region.Owner).ID);
        }

        public void RegisterRegionDeletion(RegionHooks.RegionDeletedEventArgs args)
        {
            _regionInfoManager.RemoveRegion(args.Region.ID);
        }

        public bool ClearAllowUsers(CommandArgsExtension args, string regionName)
        {
            RegisterCommand(args, TShock.Regions.GetRegionByName(regionName));
            return ClearAllowUsers(regionName);
        }

        public bool ClearAllowUsers(string regionName)
        {
            Region r = TShock.Regions.GetRegionByName(regionName);
            if (r != null)
            {
                r.AllowedIDs.Clear();
                string ids = string.Join(",", r.AllowedIDs);
                return _tshockDatabase.Query("UPDATE Regions SET UserIds=@0 WHERE RegionName=@1 AND WorldID=@2", ids,
                                       regionName, Main.worldID.ToString()) > 0;
            }
            return false;
        }

        public void RegisterCommand(CommandArgsExtension args, Region region)
        {
            _regionInfoManager.UpdateLastUpdate(region, DateTime.Now);
            _regionInfoManager.UpdateLastUser(region, args.Player.Account);
        }
    }
}
