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

namespace RegionExtension
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        #region plugin overrides
        public override string Author => "Rekman";
        public override string Description => "More region command & functionality";
        public override string Name => "Region Extension";
        public override Version Version => new Version(1, 1, 0, 0);
        #endregion

        #region fields
        public ContextManager Contexts;
        public List<FastRegion> FastRegions;
        public ConfigFile Config;
        public RegionExtManager ExtManager;
        #endregion

        #region initialize
        public Plugin(Main game) : base(game)
        {

        }

        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.NetGetData.Register(this, OnGetData);
            PlayerHooks.PlayerLogout += OnPlayerLogout;
            PlayerHooks.PlayerCommand += OnPlayerCommand;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
                PlayerHooks.PlayerLogout -= OnPlayerLogout;
                PlayerHooks.PlayerCommand -= OnPlayerCommand;
            }
        }

        private void OnInitialize(EventArgs args)
        {
            Contexts = new ContextManager();
            Contexts.Initialize();
            PluginCommands.Initialize(this);
            ExtManager = new RegionExtManager(TShock.DB);
            Config = ConfigFile.Read();
            FastRegions = new List<FastRegion>();
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
            int id = FindFastRegionByUser(TShock.Players[args.Msg.whoAmI].Account);
            if (id == -1) return;
            switch (args.MsgID)
            {
                case (PacketTypes.MassWireOperation):
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
            for (int i = 0; i < FastRegions.Count; i++)
                if (FastRegions[i].User == user) return i;
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
                    for(int i = 1; i < args.Parameters.Count; i++)
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
