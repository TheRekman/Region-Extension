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
    internal class ResizeSubCommand : SubCommand
    {
        public bool _checkRegionOwn;

        public override string[] Names => new[] { "resize", "rs", "expand", "exp"};
        public override string Description => "RegionResizeDesc";
        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new RegionParam("region", "which region must be resized."),
                new DirectionParam("direction", "direction of move. u/d/r/l"),
                new IntParam("amount", "amount on which region must be resized.")
            };
        }

        public ResizeSubCommand(bool checkRegionOwn = false)
        {
            _checkRegionOwn = checkRegionOwn;
        }

        public override void Execute(CommandArgsExtension args)
        {
            var region = (Region)Params[0].Value;
            var direction = (Direction)Params[1].Value;
            var amount = (int)Params[2].Value;
            if (_checkRegionOwn && !CheckRegionOwn(args, region))
            {
                args.Player.SendErrorMessage("You cannot manage '{0}' region!".SFormat(region.Name));
                return;
            }
            MoveRegion(args, region, amount, direction);
        }

        private bool CheckRegionOwn(CommandArgsExtension args, Region region)
            => region.Owner == args.Player.Account.Name;

        private void MoveRegion(CommandArgsExtension args, Region region, int amount, Direction direction)
        {
            if (Plugin.RegionExtensionManager.Resize(args, region, amount, direction.TshockDirection))
                args.Player.SendSuccessMessage("Region resized.");
            else
                args.Player.SendErrorMessage("Failed resize region.");
        }
    }
}
