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
    public class UndoSubCommand : SubCommand
    {
        private bool _checkRegionOwn;

        public override string[] Names => new string[] { "undo", "u" };

        public override string Description => "undo actions on region.";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new IntParam("count", "count of undo"),
                new RegionParam("region", "region. default: region in your location.", true)
            };
        }

        public UndoSubCommand(bool checkRegionOwn = false)
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
            UndoActions(args, count, region);
        }

        private void UndoActions(CommandArgsExtension args, int count, Region region)
        {
            args.Plugin.RegionExtensionManager.HistoryManager.Undo(count, region.ID);
            args.Player.SendSuccessMessage("Undo success.");
        }

        public bool CheckRegionOwn(CommandArgsExtension args, Region region)
            => region.Owner == args.Player.Account.Name;
    }
}
