﻿using System;
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
using System.Linq;
using Steamworks;
using TShockAPI.Configuration;
using Microsoft.Xna.Framework;
using RegionExtension.RegionTriggers;

namespace RegionExtension.Database
{
    public class RegionExtManager
    {
        private IDbConnection _tshockDatabase;
        private RegionInfoManager _regionInfoManager;
        private RegionHistoryManager _historyManager;
        private DeletedRegionsDB _deletedRegionsDB;
        private RegionRequestManager _regionRequestManager;
        private DateTime _lastUpdate = DateTime.UtcNow;
        private DateTime _lastNotify = DateTime.UtcNow;

        private bool _fullyLoaded = false;

        public static event Action<AllowArgs> OnRegionAllow;
        public static event Action<RemoveArgs> OnRegionRemove;
        public static event Action<SetZArgs> OnRegionSetZ;
        public static event Action<ProtectArgs> OnRegionProtect;
        public static event Action<ResizeArgs> OnRegionResize;
        public static event Action<AllowGroupArgs> OnRegionAllowGroup;
        public static event Action<RemoveGroupArgs> OnRegionRemoveGroup;
        public static event Action<MoveArgs> OnRegionMove;
        public static event Action<RenameArgs> OnRegionRename;
        public static event Action<ChangeOwnerArgs> OnRegionChangeOwner;

        public static event Action<BaseRegionArgs> OnRegionDelete;
        public static event Action<BaseRegionArgs> OnRegionDeleted;
        public static event Action<BaseRegionArgs> OnRegionDefined;

        public static event Action<IDbConnection> OnPostInitialize;
        
        public static event Action<RequestCreatedArgs> OnRequestCreated;
        public static event Action<RequestRemovedArgs> OnRequestRemoved;

        public RegionHistoryManager HistoryManager { get { return _historyManager; } }
        public DeletedRegionsDB DeletedRegions { get { return _deletedRegionsDB; } }
        public RegionInfoManager InfoManager { get { return _regionInfoManager; } }
        public RegionRequestManager RegionRequestManager { get => _regionRequestManager; }
        public TriggerManager TriggerManager { get; private set; }
        public PropertyManager PropertyManager { get; private set; }

        public RegionExtManager(IDbConnection db)
        {
            _tshockDatabase = db;
            EventHandler();
        }

        public void InitializeDatabase(TerrariaPlugin plugin)
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
                catch
                {
                    throw new Exception("MySql not setup correctly");
                }
            }
            else
            {
                throw new Exception("Invalid storage type");
            }
            _regionInfoManager = new RegionInfoManager(database);
            TShock.Log.Info("Info manager loaded.");
            _historyManager = new RegionHistoryManager(database);
            TShock.Log.Info("History manager loaded.");
            _deletedRegionsDB = new DeletedRegionsDB(database);
            TShock.Log.Info("Deleted region database loaded.");
            _regionRequestManager = new RegionRequestManager(database);
            TShock.Log.Info("Request manager loaded.");
            TriggerManager = new TriggerManager(database);
            TShock.Log.Info("Trigger manager loaded.");
            PropertyManager = new PropertyManager(database, plugin);
            TShock.Log.Info("Property manager loaded.");
            if (OnPostInitialize != null)
                OnPostInitialize(database);
            _fullyLoaded = true;
            TShock.Log.Info("Region extension manager fully loaded!");
        }

        public void PostInitialize(TerrariaPlugin plugin)
        {
            InitializeDatabase(plugin);
            _regionInfoManager.PostInitialize();
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
            OnRegionDeleted += (args) =>
            {
                if (_regionRequestManager.Requests.Any(r => r.Region.ID == args.Region.ID))
                    RemoveRequest(args.Region, args.UserExecutor.Account, false);
            };
            OnRequestRemoved += (args) =>
            {
                if (!args.Approved && TShock.Regions.Regions.Any(r => args.Request.Region.ID == r.ID))
                    TShock.Regions.DeleteRegion(args.Request.Region.ID);
            };
            OnRegionDefined += (args) =>
                _regionInfoManager.AddNewRegion(args.Region.ID, args.UserExecutor.Account.ID);
        }

        public void Dispose()
        {
            OnRegionMove = null;
            OnRegionAllow = null;
            OnRegionRemove = null;
            OnRegionChangeOwner = null;
            OnRegionAllowGroup = null;
            OnRegionRemoveGroup = null;
            OnRegionRename = null;
            OnRegionResize = null;
            OnRegionSetZ = null;
            OnRegionProtect = null;
            OnRegionDelete = null;
            OnRegionDeleted = null;
            OnRequestRemoved = null;
            OnRegionDefined = null;
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

        public bool DeleteRegion(CommandArgsExtension args, Region region) =>
            DeleteRegion(args.Player, region);

        public bool DeleteRegion(TSPlayer user, Region region)
        {
            OnRegionDelete(new BaseRegionArgs(user, region));
            var res = TShock.Regions.DeleteRegion(region.Name);
            if (res && OnRegionDelete != null)
                OnRegionDeleted(new BaseRegionArgs(user, region));
            return res;
        }

        public bool DefineRegion(CommandArgsExtension args, Region region) =>
            DefineRegion(args.Player, region);

        public bool DefineRegion(TSPlayer user, Region region)
        {
            var res = TShock.Regions.AddRegion(region.Area.X, region.Area.Y, region.Area.Width, region.Area.Height, region.Name, region.Owner, region.WorldID, region.Z) &&
                      TShock.Regions.SetRegionState(region.Name, region.DisableBuild);
            if (res)
            {
                region = TShock.Regions.GetRegionByName(region.Name);
                OnRegionDefined(new BaseRegionArgs(user, region));
            }
            return res;
        }

        public bool CreateRequest(Region region, TSPlayer user)
        {
            if (!DefineRegion(user, region))
                return false;
            region = TShock.Regions.GetRegionByName(region.Name);
            if (region == null)
                return false;
            if (_regionRequestManager.AddRequest(region, user.Account))
            {
                var req = _regionRequestManager.Requests.First(r => r.Region.ID == region.ID);
                if (OnRequestCreated != null)
                    OnRequestCreated(new RequestCreatedArgs(req));
            }
            return true;
        }

        public bool ApproveRequest(UserAccount user, int regionId)
        {
            var req = _regionRequestManager.Requests.FirstOrDefault(req => req.Region.ID == regionId);
            if (req == null)
                return false;
            return ApproveRequest(user, req);
        }

        public bool ApproveRequest(UserAccount user, Region region)
        {
            var req = _regionRequestManager.Requests.FirstOrDefault(req => req.Region.ID == region.ID);
            if (req == null)
                return false;
            return ApproveRequest(user, req);
        }

        public bool ApproveRequest(UserAccount user, Request request)
        {
            if (!_regionRequestManager.Requests.Contains(request))
                return false;
            return RemoveRequest(request.Region, user, true);
        }

        public bool DenyRequest(UserAccount user, int regionId)
        {
            var req = _regionRequestManager.Requests.FirstOrDefault(req => req.Region.ID == regionId);
            if (req == null)
                return false;
            return DenyRequest(user, req);
        }

        public bool DenyRequest(UserAccount user, Region region)
        {
            var req = _regionRequestManager.Requests.FirstOrDefault(req => req.Region.ID == region.ID);
            if (req == null)
                return false;
            return DenyRequest(user, req);
        }

        public bool DenyRequest(UserAccount user, Request request)
        {
            if (!_regionRequestManager.Requests.Contains(request))
                return false;
            return RemoveRequest(request.Region, user, false);
        }

        public bool RemoveRequest(Region region, UserAccount user, bool approved)
        {
            if (region == null)
                return false;
            var req = _regionRequestManager.Requests.First(r => r.Region.ID == region.ID);
            if (req == null)
                return false;
            bool res = _regionRequestManager.DeleteRequest(region);
            if (res && OnRequestRemoved != null)
                OnRequestRemoved(new RequestRemovedArgs(user, req, approved));
            return res;
        }

        public void Update()
        {
            if (!_fullyLoaded)
                return;
            TriggerManager.OnUpdate();
            if (DateTime.UtcNow < _lastUpdate.AddSeconds(10))
                return;
            var requestsToRemove = _regionRequestManager.Requests.Where(r =>
            {
                var settings = Utils.GetSettingsByUserAccount(r.User).RequestTime;
                var time = StringTime.FromString(Utils.GetSettingsByUserAccount(r.User).RequestTime);
                if(time.IsZero())
                    return false;
                return r.DateCreation + time < DateTime.UtcNow;
            }).ToArray();
            foreach(var req in requestsToRemove)
            {
                RemoveRequest(req.Region, null, Utils.GetSettingsByUserAccount(req.User).AutoApproveRequest);
            }
            var timePeriod = StringTime.FromString(Plugin.Config.NotificationPeriod);
            if (!timePeriod.IsZero() && _lastNotify + timePeriod < DateTime.UtcNow)
            {
                var players = TShock.Players.Where(p => p != null && p.Account != null && p.HasPermission(Permissions.RegionExtCmd));
                foreach (var plr in players)
                    SendRequestNotify(plr, _regionRequestManager.GetSortedRegionRequestsNames());
                _lastNotify = DateTime.UtcNow;
            }
            _lastUpdate = DateTime.UtcNow;
        }

        public void SendRequestNotify(TSPlayer player, IEnumerable<string> strings)
        {
            var players = TShock.Players.Where(p => p != null && p.Account != null && p.HasPermission(Permissions.RegionExtCmd));
                PaginationTools.SendPage(player, 0, PaginationTools.BuildLinesFromTerms(strings, null, ", ", 240), new PaginationTools.Settings()
                {
                    HeaderFormat = "Active region requests:",
                    IncludeFooter = false,
                    LineTextColor = Color.White
                });
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
            _regionInfoManager.UpdateLastUpdate(region.ID, DateTime.UtcNow);
            _regionInfoManager.UpdateLastUser(region.ID, executor.Account.ID);
        }

        internal void Reload(ReloadEventArgs e)
        {
            TriggerManager.Reload(e);
            PropertyManager.Reload(e);
        }
    }
}
