﻿using RegionExtension.Commands.Parameters;
using System;
using System.Linq;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Commands.SubCommands
{
    public class OwnerList : SubCommand
    {
        public override string[] Names => new string[] { "ownerlist", "ol" };

        public override string Description => "OwnerListDesc";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new UserAccountParam("username", "which useraccount check. Default: your account", true),
                new IntParam("page", "page of the list. Default: 1", true, 1)
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var userAccount = (UserAccount)Params[0].Value;
            var page = ((IntParam)Params[1]).TValue;
            SendOwnerList(args, page, userAccount);
        }

        private void SendOwnerList(CommandArgsExtension args, int page, UserAccount userAccount)
        {
            var regionNames = TShock.Regions.Regions.Where(r => r.Owner == userAccount.Name && r.WorldID == Main.worldID.ToString())
                                                    .Select(r => r.Name)
                                                    .ToList();
            var usedName = args.Message.Split(' ')[0].Remove(0, 1);
            var usedSubCommandName = args.Parameters[0];
            PaginationTools.SendPage(args.Player, page, PaginationTools.BuildLinesFromTerms(regionNames),
                        new PaginationTools.Settings
                        {
                            HeaderFormat = "Player regions ({0}/{1}):",
                            FooterFormat = "Type {0}{1} {2} {{0}} {3} for more."
                                           .SFormat(TShockAPI.Commands.Specifier, usedName, usedSubCommandName, userAccount.Name),
                            NothingToDisplayString = "There are currently no regions."
                        });
        }
    }
}