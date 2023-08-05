using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;
using RegionExtension.Database;
using System.Reflection;
using Org.BouncyCastle.Crypto.Engines;
using Terraria.DataStructures;
using System.Security.AccessControl;
using System.Threading.Tasks;

namespace RegionExtension
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        #region plugin overrides
        public override string Author => "Rekman";
        public override string Description => "More region command & functionality";
        public override string Name => "Region Extension";
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version;
        #endregion

        #region fields
        public ContextManager Contexts;
        public List<FastRegion> FastRegions;
        public static ConfigFile Config;
        public static RegionExtManager RegionExtensionManager;
        private List<Point16> _lastActive = new List<Point16>();
        private DateTime _lastActiveCheck = DateTime.UtcNow;
        #endregion

        bool _checkingHasBuild = false;

        #region initialize
        public Plugin(Main game) : base(game)
        {

        }

        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.NetGetData.Register(this, OnGetData);
            ServerApi.Hooks.GamePostInitialize.Register(this, OnPostInitialize, int.MinValue);
            ServerApi.Hooks.GamePostUpdate.Register(this, OnPostUpdate);
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreetPlayer);
            PlayerHooks.PlayerLogout += OnPlayerLogout;
            PlayerHooks.PlayerPostLogin += OnPlayerLogin;
            PlayerHooks.PlayerCommand += OnPlayerCommand;
            PlayerHooks.PlayerHasBuildPermission += OnHasPlayerPermission;
            RegionExtensionManager = new RegionExtManager(TShock.DB);
        }

        private void OnGreetPlayer(GreetPlayerEventArgs args)
        {
            RegionExtensionManager.TriggerManager.OnPlayerEnter(args);
        }

        private void OnPostUpdate(EventArgs args)
        {
            RegionExtensionManager.Update();
            UpdateLastActive();
        }

        private void OnPlayerLogin(PlayerPostLoginEventArgs e)
        {
            if (!StringTime.FromString(Config.NotificationPeriod).IsZero() || !e.Player.HasPermission(Permissions.manageregion))
                return;
            RegionExtensionManager.SendRequestNotify(e.Player, RegionExtensionManager.RegionRequestManager.GetSortedRegionRequestsNames());
        }

        private void OnPostInitialize(EventArgs args)
        {
            Contexts = new ContextManager();
            Contexts.Initialize();
            PluginCommands.Initialize(this);
            Config = ConfigFile.Read();
            FastRegions = new List<FastRegion>();
            RegionExtensionManager.PostInitialize(this);
        }

        private void OnHasPlayerPermission(PlayerHasBuildPermissionEventArgs e)
        {
            if (_checkingHasBuild)
            {
                e.Result = PermissionHookResult.Unhandled;
                return;
            }
            _checkingHasBuild = true;
            if (e.Player.HasBuildPermission(e.X, e.Y, true) && TShock.Regions.InArea(e.X, e.Y))
                _lastActive.Add(new Point16(e.X, e.Y));
            _checkingHasBuild = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
                ServerApi.Hooks.GamePostInitialize.Deregister(this, OnPostInitialize);
                ServerApi.Hooks.GamePostUpdate.Deregister(this, OnPostUpdate);
                PlayerHooks.PlayerLogout -= OnPlayerLogout;
                PlayerHooks.PlayerPostLogin -= OnPlayerLogin;
                PlayerHooks.PlayerCommand -= OnPlayerCommand;
                PlayerHooks.PlayerHasBuildPermission -= OnHasPlayerPermission;
            }
        }

        private void UpdateLastActive()
        {
            if (DateTime.UtcNow < _lastActiveCheck.AddSeconds(90))
                return;
            var points = _lastActive;
            _lastActive = new List<Point16>();
            Task.Run(() =>
            {
                var regionsToUpdate = new HashSet<int>();
                foreach (var point in points)
                    foreach (var id in TShock.Regions.InAreaRegionID(point.X, point.Y))
                            regionsToUpdate.Add(id);
                foreach (var id in regionsToUpdate)
                    RegionExtensionManager.InfoManager.UpdateLastActivity(id, DateTime.UtcNow);
                points.Clear();
            });
            _lastActiveCheck = DateTime.UtcNow;
        }

        private void OnInitialize(EventArgs args)
        {
        }
        #endregion

        #region hooks & events

        private void OnPlayerLogout(PlayerLogoutEventArgs e)
        {
            int id = FindFastRegionByUser(e.Player.Account);
            if (id != -1) FastRegions.RemoveAt(id);
        }

        private void OnGetData(GetDataEventArgs args)
        {
            int id;
            switch (args.MsgID)
            {
                case (PacketTypes.MassWireOperation):
                    id = FindFastRegionByUser(TShock.Players[args.Msg.whoAmI]?.Account);
                    if (id == -1) return;
                    using (var reader = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
                    {

                        int startX = reader.ReadInt16();
                        int startY = reader.ReadInt16();
                        int endX = reader.ReadInt16();
                        int endY = reader.ReadInt16();
                        if (startX >= 0 && startY >= 0 && endX >= 0 && endY >= 0 && startX < Main.maxTilesX && startY < Main.maxTilesY && endX < Main.maxTilesX && endY < Main.maxTilesY)
                        {
                            if (FastRegions[id].SetPoints(startX, startY, endX, endY)) FastRegions.RemoveAt(id);
                        }
                        
                    }
                    args.Handled = true;
                    break;
                case (PacketTypes.Tile):
                    id = FindFastRegionByUser(TShock.Players[args.Msg.whoAmI]?.Account);
                    if (id == -1) return;
                    using (var reader = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
                    {
                        reader.ReadByte();
                        int x = reader.ReadInt16();
                        int y = reader.ReadInt16();
                        if (x >= 0 && y >= 0 && x < Main.maxTilesX && y < Main.maxTilesY)
                        {
                            if (FastRegions[id].SetPoint(x, y)) FastRegions.RemoveAt(id);
                        }
                    }
                    args.Handled = true;
                    break;
            }
        }



        public int FindFastRegionByUser(UserAccount user)
        {
            if(user == null)
                return -1;
            if (FastRegions == null)
            {
                FastRegions = new List<FastRegion>();
                return -1;
            }
            for (int i = 0; i < FastRegions.Count; i++)
                if (FastRegions[i]?.User == user)
                    return i;
                return -1;
        }

        private void OnPlayerCommand(PlayerCommandEventArgs args)
        {
            if (!args.Player.HasPermission(Permissions.manageregion) && !args.Player.HasPermission("regionext.own")) return;
            switch (args.CommandName)
            {
                case "/re":
                case "/regionext":
                case "/ro":
                case "/regionown":
                case "region":
                case "re":
                    for (int i = 1; i < args.Parameters.Count; i++)
                        if (args.Parameters[i].StartsWith(Config.ContextSpecifier))
                            Contexts.InitializeContext(i, args);
                    if (Config.AutoCompleteSameName && args.Parameters.Count > 1 && "define" == args.Parameters[0])
                        args.Parameters[1] = Utils.AutoCompleteSameName(args.Parameters[1], Config.AutoCompleteSameNameFormat);
                    break;
            }
        }
        #endregion


    }
}
