using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using Terraria;
using TShockAPI;
using TShockAPI.DB;
using MySql.Data.MySqlClient;
using TerrariaApi.Server;
using TShockAPI.Hooks;
using RegionExtension.Commands;
using RegionExtension.Commands.Parameters;
using RegionExtension.Database.Actions;
using RegionExtension.Database.EventsArgs;
using IL.SteelSeries.GameSense;

namespace RegionExtension.Database
{
    public class RegionExtManager
    {
        private IDbConnection _tshockDatabase;
        private RegionInfoManager _regionInfoManager;
        private RegionHistoryManager _historyManager;

        public event Action<AllowArgs> OnRegionAllow;
        public event Action<RemoveArgs> OnRegionRemove;
        public event Action<SetZArgs> OnRegionSetZ;
        public event Action<ProtectArgs> OnRegionProtect;
        public event Action<ResizeArgs> OnRegionResize;
        public event Action<AllowGroupArgs> OnRegionAllowGroup;
        public event Action<RemoveGroupArgs> OnRegionRemoveGroup;
        public event Action<MoveArgs> OnRegionMove;
        public event Action<RenameArgs> OnRegionRename;
        public event Action<ChangeOwnerArgs> OnRegionChangeOwner;

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
            _historyManager = new RegionHistoryManager(database);
        }

        public void EventHandler()
        {
            OnRegionMove += (args) => RegisterAction(new Move(args), args);
            OnRegionAllow += (args) => RegisterAction(new Allow(args), args);
            OnRegionRemove += (args) => RegisterAction(new Remove(args), args);
            OnRegionChangeOwner += (args) => RegisterAction(new ChangeOwner(args), args);
            OnRegionAllowGroup += (args) => RegisterAction(new AllowGroup(args), args);
            OnRegionRemoveGroup += (args) => RegisterAction(new RemoveGroup(args), args);
            OnRegionRename += (args) => RegisterAction(new Rename(args), args);
            OnRegionResize += (args) => RegisterAction(new Resize(args), args);
        }

        public void RegisterAction(IAction action, BaseRegionArgs args)
        {
            RegisterCommand(args.UserExecutor, args.Region);
            _historyManager.SaveAction(action, args.Region, args.UserExecutor.Account);
        }

        public void RegisterRegionDefine(RegionHooks.RegionCreatedEventArgs args)
        {
            _regionInfoManager.AddNewRegion(args.Region.ID, TShock.UserAccounts.GetUserAccountByName(args.Region.Owner).ID);
        }

        public void RegisterRegionDeletion(RegionHooks.RegionDeletedEventArgs args)
        {
            _regionInfoManager.RemoveRegion(args.Region.ID);
        }

        public bool MoveRegion(CommandArgsExtension args, Region region, int amount, Direction direction)
        {
            var newPos = direction.GetNewPosition(region.Area.X, region.Area.Y, amount);
            OnRegionMove(new MoveArgs(args.Player, region, amount, direction));
            return TShock.Regions.PositionRegion(region.Name, newPos.x, newPos.y, region.Area.Width, region.Area.Height);
        }

        public bool AllowUser(CommandArgsExtension args, Region region, UserAccount account)
        {
            OnRegionAllow(new AllowArgs(args.Player, region, account));
            return TShock.Regions.AddNewUser(region.Name, account.Name);
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

        public void RegisterCommand(TSPlayer executor, Region region)
        {
            _regionInfoManager.UpdateLastUpdate(region, DateTime.Now);
            _regionInfoManager.UpdateLastUser(region, executor.Account);
        }
    }
}
