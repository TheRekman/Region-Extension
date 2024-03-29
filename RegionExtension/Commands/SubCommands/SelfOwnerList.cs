﻿using RegionExtension.Commands.Parameters;
using TShockAPI.DB;
using TShockAPI;
using Terraria;
using System.Linq;
using System;

namespace RegionExtension.Commands.SubCommands
{
    internal class SelfOwnerList : SubCommand
    {
        public override string[] Names => new string[] { "ownerlist", "ol" };

        public override string Description => "SelfOwnerDesc";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new IntParam("page", "page of the list. Default: 1", true, 1)
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var page = ((IntParam)Params[0]).TValue;
            SendOwnerList(args, page);
        }

        private void SendOwnerList(CommandArgsExtension args, int page)
        {
            var regionNames = TShock.Regions.Regions.Where(r => r.Owner == args.Player.Account.Name && r.WorldID == Main.worldID.ToString())
                                                    .Select(r => r.Name)
                                                    .ToList();
            var usedName = args.Message.Split(' ')[0].Remove(0, 1);
            var usedSubCommandName = args.Parameters[0];
            PaginationTools.SendPage(args.Player, page, PaginationTools.BuildLinesFromTerms(regionNames),
                        new PaginationTools.Settings
                        {
                            HeaderFormat = "Player regions ({0}/{1}):",
                            FooterFormat = "Type {0}{1} {2} {{0}} for more."
                                           .SFormat(TShockAPI.Commands.Specifier, usedName, usedSubCommandName),
                            NothingToDisplayString = "There are currently no regions."
                        });
        }
    }
}