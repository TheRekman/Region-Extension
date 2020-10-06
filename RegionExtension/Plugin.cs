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
        public List<ContextCommand> Contexts;
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
            Contexts = new List<ContextCommand>();
            Contexts.Add(new ContextCommand("this", ContextThis));
            Contexts.Add(new ContextCommand("myname", ContextMyName));
            Contexts.Add(new ContextCommand("near", ContextNearPlayer));
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
                    {
                        if (args.Parameters[i].StartsWith(Config.ContextSpecifier))
                        {
                            bool success = false;
                            for(int j = 0; j < Contexts.Count; j++)
                            {
                                if(Contexts[j].Context == args.Parameters[i].Remove(0, 1))
                                {
                                    Contexts[j].Initialize(args, i);
                                    success = true;
                                    break;
                                }
                            }
                            if (!success)
                            {
                                args.Player.SendErrorMessage("Context: Invalid context command!");
                                args.Handled = true;
                                return;
                            }
                        }
                    }
                    if (Config.AutoCompleteSameName && args.Parameters.Count > 1 && "define" == args.Parameters[0])
                        args.Parameters[1] = Utils.AutoCompleteSameName(args.Parameters[1], Config.AutoCompleteSameNameFormat);
                    break;
            }
        }
        #endregion


        #region context
        private void ContextThis(PlayerCommandEventArgs args, int paramID)
        {
            if (args.Player.CurrentRegion == null)
            {
                args.Player.SendErrorMessage("Context: You are not in a region!");
                args.Handled = true;
                return;
            }
            args.Parameters[paramID] = args.Player.CurrentRegion.Name;
        }

        private void ContextMyName(PlayerCommandEventArgs args, int paramID)
            => args.Parameters[paramID] = args.Player.Account.Name;

        private void ContextNearPlayer(PlayerCommandEventArgs args, int paramID)
        {
            if (TShock.Utils.GetActivePlayerCount() == 1)
            {
                args.Player.SendErrorMessage("You are alone on server!");
                args.Handled = true;
                return;
            }

            float x = args.Player.X;
            float y = args.Player.Y;
            var player = TShock.Players.FirstOrDefault(plr => plr != null && plr.Active && plr != args.Player && plr.Account != null);
            if (player == null)
            {
                args.Player.SendErrorMessage("Failed found nearest player!");
                args.Handled = true;
                return;
            }
            float minDistance = Utils.CountDistance(x, y, player.X, player.Y);
            for (int i = 0; i < TShock.Players.Length; i++)
                if (TShock.Players[i] != null && TShock.Players[i].Active && TShock.Players[i] != args.Player && TShock.Players[i].Account != null)
                {
                    float distance = Utils.CountDistance(x, y, TShock.Players[i].X, TShock.Players[i].Y);
                    if (minDistance > distance)
                    {
                        player = TShock.Players[i];
                        minDistance = distance;
                    }
                }
            args.Parameters[paramID] = player.Account.Name;
        }
        #endregion

    }
}
