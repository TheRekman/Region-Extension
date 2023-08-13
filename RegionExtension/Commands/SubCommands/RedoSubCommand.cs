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
    public class RedoSubCommand : SubCommand
    {
        private bool _checkRegionOwn;

        public override string[] Names => new string[] { "redo", "r" };

        public override string Description => "RedoHistoryDesc";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new IntParam("count", "count of redo"),
                new RegionParam("region", "region. default: region in your location.", true)
            };
        }

        public RedoSubCommand(bool checkRegionOwn = false)
        {
            _checkRegionOwn = checkRegionOwn;
        }

        public override void Execute(CommandArgsExtension args)
        {
            var count = (int)Params[0].Value;
            var region = (Region)Params[1].Value;
            if (_checkRegionOwn && !CheckRegionOwn(args, region))
            {
                args.Player.SendErrorMessage("You cannot manage '{0}' region!".SFormat(region.Name));
                return;
            }
            RedoActions(args, count, region);
        }

        private void RedoActions(CommandArgsExtension args, int count, Region region)
        {
            Plugin.RegionExtensionManager.HistoryManager.Redo(count, region.ID);
            args.Player.SendSuccessMessage("Redo success.");
        }

        public bool CheckRegionOwn(CommandArgsExtension args, Region region)
            => region.Owner == args.Player.Account.Name;
    }
}
