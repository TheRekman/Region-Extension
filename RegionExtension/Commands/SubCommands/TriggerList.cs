﻿using RegionExtension.Commands.Parameters;
using RegionExtension.RegionTriggers.Actions;
using RegionExtension.RegionTriggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;
using TShockAPI;

namespace RegionExtension.Commands.SubCommands
{
    public class TriggerList : SubCommand
    {
        public override string[] Names => new string[] { "list", "l" };

        public override string Description => "List triggers of the region";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new RegionParam("region", "region of which triggers will be given."),
                new IntParam("page", "page of list.")
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var region = (Region)_params[0].Value;
            var page = (int)_params[1].Value;
            AddRegionTrigger(args, region, page);
        }

        private void AddRegionTrigger(CommandArgsExtension args, Region region, int page)
        {
            var triggers = Plugin.RegionExtensionManager.TriggerManager.GetTriggers(region).Select(t => "({0}) {1} {2} {3}".SFormat(t.LocalId, t.Event.ToString(), t.Action.Name, t.Action.GetArgsString()));
            var usedName = args.Message.Split(' ')[0].Remove(0, 1);
            var usedSubCommandName = args.Parameters[0];
            PaginationTools.SendPage(args.Player, page, triggers.ToList(),
                        new PaginationTools.Settings
                        {
                            HeaderFormat = "Region triggers ({0}/{1}):",
                            FooterFormat = "Type {0}{1} {2} {{0}} {3} for more."
                                           .SFormat(TShockAPI.Commands.Specifier, usedName, usedSubCommandName, region.Name),
                            NothingToDisplayString = "There are currently no regions."
                        });
        }
    }
}