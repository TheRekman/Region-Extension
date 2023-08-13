using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;
using TShockAPI;

namespace RegionExtension.Commands.SubCommands
{
    internal class TeleportToRegionSubCommand : SubCommand
    {
        private bool _checkRegionOwn;

        public override string[] Names => new string[] { "tp" };

        public override string Description => "TeleportToRegionDesc";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new RegionParam("region", "region."),
            };
        }

        public TeleportToRegionSubCommand(bool checkRegionOwn = false)
        {
            _checkRegionOwn = checkRegionOwn;
        }

        public override void Execute(CommandArgsExtension args)
        {
            var region = (Region)Params[0].Value;
            if (_checkRegionOwn && !CheckRegionOwn(args, region))
            {
                args.Player.SendErrorMessage("You cannot manage '{0}' region!".SFormat(region.Name));
                return;
            }
            TeleportToRegion(args, region);
        }

        private void TeleportToRegion(CommandArgsExtension args, Region region)
        {
            args.Player.Teleport(region.Area.Center.X * 16, region.Area.Center.Y * 16);
        }

        public bool CheckRegionOwn(CommandArgsExtension args, Region region)
            => region.Owner == args.Player.Account.Name;
    }
}
