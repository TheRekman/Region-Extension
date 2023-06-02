using RegionExtension.Commands.Parameters;
using System.Linq;
using Terraria;
using TShockAPI;

namespace RegionExtension.Commands.SubCommands
{
    internal class LastActiveList : SubCommand
    {
        public override string[] Names => new string[] { "listact", "la" };

        public override string Description => "get list of regions.";

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
                                                    .Select(r => (Region: r, ExtInfo: args.Plugin.RegionExtensionManager.InfoManager.RegionsInfo.First(ri => ri.Id == r.ID)))
                                                    .OrderBy(r => r.ExtInfo.LastActivity)
                                                    .Reverse()
                                                    .Select(r => r.Region.Name + " " + r.ExtInfo.LastActivity.ToString(Utils.ShortDateFormat))
                                                    .ToList();
            var usedName = args.Message.Split(' ')[0].Remove(0, 1);
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
