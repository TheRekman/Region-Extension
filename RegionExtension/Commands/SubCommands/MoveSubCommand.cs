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
    public class MoveSubCommand : SubCommand
    {
        public bool _checkRegionOwn;

        public override string[] Names => new[] { "move", "mv" };
        public override string Description => "MoveRegionDesc";
        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new RegionParam("region", "which region must be moved."),
                new IntParam("amount", "amount on which region must be moved."),
                new DirectionParam("direction", "direction of move. u/d/r/l")
            };
        }

        public MoveSubCommand(bool checkRegionOwn = false)
        {
            _checkRegionOwn = checkRegionOwn;
        }

        public override void Execute(CommandArgsExtension args)
        {
            var region = (Region)Params[0].Value;
            var amount = ((IntParam)Params[1]).TValue;
            var direction = (Direction)Params[2].Value;
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
            if (Plugin.RegionExtensionManager.MoveRegion(args, region, amount, direction))
                args.Player.SendSuccessMessage("Region moved.");
            else
                args.Player.SendErrorMessage("Failed move region.");
        }
    }
}
