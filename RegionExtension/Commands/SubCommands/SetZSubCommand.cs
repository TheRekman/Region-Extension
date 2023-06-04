using RegionExtension.Commands.Parameters;
using RegionExtension.Database.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Commands.SubCommands
{
    internal class SetZSubCommand : SubCommand
    {
        public bool _checkRegionOwn;

        public override string[] Names => new[] { "z" };
        public override string Description => "Sets the z-order of the region.";
        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new IntParam("z", "new priority."),
                new RegionParam("region", "target region. default: region in your location.", true),
            };
        }

        public SetZSubCommand(bool checkRegionOwn = false)
        {
            _checkRegionOwn = checkRegionOwn;
        }

        public override void Execute(CommandArgsExtension args)
        {
            var z = (int)Params[0].Value;
            var region = (Region)Params[1].Value;
            if (_checkRegionOwn && !CheckRegionOwn(args, region))
            {
                args.Player.SendErrorMessage("You cannot manage '{0}' region!".SFormat(region.Name));
                return;
            }
            MoveRegion(args, region, z);
        }

        private bool CheckRegionOwn(CommandArgsExtension args, Region region) =>
            region.Owner == args.Player.Account.Name;

        private void MoveRegion(CommandArgsExtension args, Region region, int z)
        {
            if (Plugin.RegionExtensionManager.SetZ(args, region, z))
                args.Player.SendSuccessMessage("Region priority setted.");
            else
                args.Player.SendErrorMessage("Failed set region priority.");
        }
    }
}
