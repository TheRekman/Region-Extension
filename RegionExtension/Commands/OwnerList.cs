using RegionExtension.Commands.Parameters;
using System;
using System.Linq;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Commands
{
    internal class OwnerList : SubCommand
    {
        public override string[] Names => new string[] { "ownerlist", "ol" };

        public override string Description => "get list of regions which the given player is owner";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new IntParam("page", "page of the list. Default: 1", true, 1),
                new UserAccountParam("username", "which useraccount check. Default: your account", true)
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var page = ((IntParam)Params[0]).TValue;
            var userAccount = (UserAccount)Params[1].Value;
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