using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;
using TShockAPI;
using OTAPI;
using Terraria;

namespace RegionExtension.Commands.SubCommands
{
    internal class RegionListSubCommand : SubCommand
    {
        public override string[] Names => new string[] { "list", "l" };

        public override string Description => "Lists all regions.";

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
            SendRegionList(args, page);
        }

        private void SendRegionList(CommandArgsExtension args, int page)
        {
            var regionNames = TShock.Regions.Regions.Where(r => r.WorldID == Main.worldID.ToString())
                                                    .Select(r => r.Name)
                                                    .ToList();
            var usedName = args.Message.Split(' ')[0];
            var usedSubCommandName = args.Parameters[0];
            PaginationTools.SendPage(args.Player, page, PaginationTools.BuildLinesFromTerms(regionNames),
                        new PaginationTools.Settings
                        {
                            HeaderFormat = "Regions ({0}/{1}):",
                            FooterFormat = "Type {0}{1} {2} {{0}} for more."
                                           .SFormat(TShockAPI.Commands.Specifier, usedName, usedSubCommandName),
                            NothingToDisplayString = "There are currently no regions."
                        });
        }
    }
}
