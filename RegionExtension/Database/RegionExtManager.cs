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
using System.Linq;
using Steamworks;
using TShockAPI.Configuration;

namespace RegionExtension.Database
{
    public class RegionExtManager
    {
        private IDbConnection _tshockDatabase;
        private RegionInfoManager _regionInfoManager;
        private RegionHistoryManager _historyManager;
        private DeletedRegionsDB _deletedRegionsDB;

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

        public event Action<BaseRegionArgs> OnRegionDelete;
        public event Action<BaseRegionArgs> OnRegionDefine;

        public RegionHistoryManager HistoryManager { get { return _historyManager; } }
        public DeletedRegionsDB DeletedRegions { get { return _deletedRegionsDB; } }
        public RegionInfoManager InfoManager { get { return _regionInfoManager; } }

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
            _deletedRegionsDB = new DeletedRegionsDB(database);
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
            OnRegionSetZ += (args) => RegisterAction(new SetZ(args), args);
            OnRegionProtect += (args) => RegisterAction(new Protect(args), args);

            OnRegionDelete += (args) =>
            {
                _deletedRegionsDB.RegisterDeletedRegion(args.Region, args.UserExecutor.Account,
                                                    _regionInfoManager.RegionsInfo.First(reg => reg.Id == args.Region.ID));
                _regionInfoManager.RemoveRegion(args.Region.ID);
            };
            OnRegionDefine += (args) =>
                _regionInfoManager.AddNewRegion(args.Region.ID, args.UserExecutor.Account.ID);
        }

        public void RegisterAction(IAction action, BaseRegionArgs args)
        {
            RegisterCommand(args.UserExecutor, args.Region);
            _historyManager.SaveAction(action, args.Region, args.UserExecutor.Account);
        }

        public bool RenameRegion(CommandArgsExtension args, Region region, string newName)
        {
            OnRegionRename(new RenameArgs(args.Player, region, newName));
            return TShock.Regions.RenameRegion(region.Name, newName);
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

        public bool RemoveUser(CommandArgsExtension args, Region region, UserAccount account)
        {
            OnRegionRemove(new RemoveArgs(args.Player, region, account));
            return TShock.Regions.RemoveUser(region.Name, account.Name);
        }

        public bool AllowGroup(CommandArgsExtension args, Region region, Group group)
        {
            OnRegionAllowGroup(new AllowGroupArgs(args.Player, region, group));
            return TShock.Regions.AllowGroup(region.Name, group.Name);
        }

        public bool RemoveGroup(CommandArgsExtension args, Region region, Group group)
        {
            OnRegionRemoveGroup(new RemoveGroupArgs(args.Player, region, group));
            return TShock.Regions.RemoveGroup(region.Name, group.Name);
        }

        public bool SetZ(CommandArgsExtension args, Region region, int amount)
        {
            OnRegionSetZ(new SetZArgs(args.Player, region, amount));
            return TShock.Regions.SetZ(region.Name, amount);
        }

        public bool Protect(CommandArgsExtension args, Region region, bool protect)
        {
            OnRegionProtect(new ProtectArgs(args.Player, region, protect));
            return TShock.Regions.SetRegionState(region.Name, protect);
        }

        public bool Resize(CommandArgsExtension args, Region region, int amount, int direction)
        {
            OnRegionResize(new ResizeArgs(args.Player, region, amount, direction));
            return TShock.Regions.ResizeRegion(region.Name, amount, direction);
        }

        public bool ChangeOwner(CommandArgsExtension args, Region region, UserAccount account)
        {
            OnRegionChangeOwner(new ChangeOwnerArgs(args.Player, region, account));
            return TShock.Regions.ChangeOwner(region.Name, account.Name);
        }

        public bool DeleteRegion(CommandArgsExtension args, Region region)
        {
            OnRegionDelete(new BaseRegionArgs(args.Player, region));
            return TShock.Regions.DeleteRegion(region.Name);
        }

        public bool DefineRegion(CommandArgsExtension args, Region region)
        {
            var res = TShock.Regions.AddRegion(region.Area.X, region.Area.Y, region.Area.Width, region.Area.Height, region.Name, region.Owner, region.WorldID, region.Z);
            if (res)
            {
                region = TShock.Regions.GetRegionByName(region.Name);
                OnRegionDefine(new BaseRegionArgs(args.Player, region));
            }
            return res;
        }

        public List<string> GetRegionInfo(Region region) =>
            _regionInfoManager.GetRegionInfo(region.ID);

        public List<string> GetRegionHistory(int count, Region region) =>
            _historyManager.GetActionsInfo(count, region.ID);

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
