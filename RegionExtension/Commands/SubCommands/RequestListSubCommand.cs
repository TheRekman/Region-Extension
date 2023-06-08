using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using Terraria;
using RegionExtension.Database;

namespace RegionExtension.Commands.SubCommands
{
    public class RequestListSubCommand : SubCommand
    {
        public override string[] Names => new string[] { "listrequest", "lr" };

        public override string Description => "Lists all region requests.";

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
            var regionNames = Plugin.RegionExtensionManager.RegionRequestManager.Requests.OrderBy(r => r.DateCreation)
                                                                                         .Select(r => Utils.GetGradientByDateTime(r.Region.Name, r.DateCreation,
                                                                                                      r.DateCreation + StringTime.FromString(Plugin.Config.RequestTime)));
            var usedName = args.Message.Split(' ')[0];
            var usedSubCommandName = args.Parameters[0];
            PaginationTools.SendPage(args.Player, page, PaginationTools.BuildLinesFromTerms(regionNames),
                        new PaginationTools.Settings
                        {
                            HeaderFormat = "Requests ({0}/{1}):",
                            FooterFormat = "Type {0}{1} {2} {{0}} for more."
                                           .SFormat(TShockAPI.Commands.Specifier, usedName, usedSubCommandName),
                            NothingToDisplayString = "There are currently no regions."
                        });
        }
    }
}
