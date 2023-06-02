using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Commands.SubCommands
{
    public class GetStorySubCommand : SubCommand
    {
        private bool _checkRegionOwn;

        public override string[] Names => new string[] { "history", "h" };

        public override string Description => "gets history about region.";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new IntParam("page", "page of history"),
                new RegionParam("region", "region. default: region in your location.", true)
            };
        }

        public GetStorySubCommand(bool checkRegionOwn = false)
        {
            _checkRegionOwn = checkRegionOwn;
        }

        public override void Execute(CommandArgsExtension args)
        {
            var page = (int)Params[0].Value;
            var region = (Region)Params[1].Value;

            if (_checkRegionOwn && !CheckRegionOwn(args, region))
            {
                args.Player.SendErrorMessage("You cannot manage '{0}' region!".SFormat(region.Name));
                return;
            }
            SendRegionHistory(args, page, region);
        }

        private void SendRegionHistory(CommandArgsExtension args, int page, Region region)
        {
            var lines = args.Plugin.RegionExtensionManager.GetRegionHistory(50, region);
            var usedName = args.Message.Split(' ')[0];
            var usedSubCommandName = args.Parameters[0];
            PaginationTools.SendPage(
                args.Player, page, lines, new PaginationTools.Settings
                {
                    HeaderFormat = string.Format("History About Region \"{0}\" ({{0}}/{{1}}):", region.Name),
                    FooterFormat = string.Format("Type {0}{1} {2} {3} {{0}} for more information.", TShockAPI.Commands.Specifier, usedName, usedSubCommandName, region.Name),
                    NothingToDisplayString = "There is nothig to see."
                }
            );
        }

        public bool CheckRegionOwn(CommandArgsExtension args, Region region)
            => region.Owner == args.Player.Account.Name;
    }
}
