﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.Hooks;

namespace RegionExtension
{
    public class ContextManager
    {
        List<ContextCommand> Contexts;
        public ContextManager()
        {

        }
        public void Initialize()
        {
            Contexts = new List<ContextCommand>();
            Contexts.Add(new ContextCommand(ContextThis, "this", "t")
            {
                Description = "get current region"
            });
            Contexts.Add(new ContextCommand(ContextMyName, "myname", "mn")
            {
                Description = "get account username"
            });
            Contexts.Add(new ContextCommand(ContextNearPlayer, "near", "n")
            {
                Description = "get nearest player"
            });

            Commands.ChatCommands.Add(new Command(new List<string> { Permissions.manageregion, "regionext.own" }, Help, "context"));
        }

        private void Help(CommandArgs args)
        {
            List<string> contextCommandDescs = new List<string>();
            for(int i = 0; i < Contexts.Count; i++)
            {
                string aliases = "";
                for(int j = 1; j < Contexts[i].Names.Length ; j++)
                    aliases += $"{Contexts[i].Names[j]}, ";
                aliases.Remove(aliases.Length - 2, 2);
                contextCommandDescs.Add("{0} ({1}) - {2}".SFormat(Contexts[i].Names[0], aliases, Contexts[i].Description));
            }

            int pageNumberCon = 1;
            if (args.Parameters.Count > 0)
            {
                int pageParamIndexCon = 0;
                if (!PaginationTools.TryParsePageNumber(args.Parameters, pageParamIndexCon, args.Player, out pageNumberCon))
                    return;
            }

            PaginationTools.SendPage(
              args.Player, pageNumberCon, contextCommandDescs,
              new PaginationTools.Settings
              {
                  HeaderFormat = "Available contexts command ({0}/{1}):",
                  FooterFormat = "Type {0}/re context {{0}} for more contexts.".SFormat(TShock.Config.Settings.CommandSpecifier)
              }
            );

        }

        public void InitializeContext(int param, PlayerCommandEventArgs args)
        {
            for (int i = 0; i < Contexts.Count; i++)
                if (Contexts[i].Names.Contains(args.Parameters[i].Remove(0, 1)))
                {
                    Contexts[i].Action(args, i);
                    return;
                }    
            args.Player.SendErrorMessage("Context: Invalid context command!");
        }

        #region methods
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
                args.Player.SendErrorMessage("Context: You are alone on server!");
                args.Handled = true;
                return;
            }

            float x = args.Player.X;
            float y = args.Player.Y;
            var player = TShock.Players.FirstOrDefault(plr => plr != null && plr.Active && plr != args.Player && plr.Account != null);
            if (player == null)
            {
                args.Player.SendErrorMessage("Context: Failed found nearest player!");
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
