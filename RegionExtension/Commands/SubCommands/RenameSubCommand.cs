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
    public class RenameSubCommand : SubCommand
    {
        public bool _checkRegionOwn;

        public override string[] Names => new[] { "rename", "rn" };
        public override string Description => "RenameRegionDesc";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new StringParam("newname", "new name for region."),
                new RegionParam("region", "which region must be renamed. Default: region in your location.", true)
            };
        }

        public RenameSubCommand(bool checkRegionOwn = false)
        {
            _checkRegionOwn = checkRegionOwn;
        }

        public override void Execute(CommandArgsExtension args)
        {
            var newname = (string)Params[0].Value;
            var region = (Region)Params[1].Value;
            if (_checkRegionOwn && !CheckRegionOwn(args, region))
            {
                args.Player.SendErrorMessage("You cannot manage '{0}' region!".SFormat(region.Name));
                return;
            }
            RenameRegion(args, newname, region);
        }

        public bool CheckRegionOwn(CommandArgsExtension args, Region region)
            => region.Owner == args.Player.Account.Name;

        private void RenameRegion(CommandArgsExtension args, string newname, Region region)
        {
            if (Plugin.RegionExtensionManager.RenameRegion(args, region, newname))
                args.Player.SendSuccessMessage("Region renamed.");
            else
                args.Player.SendErrorMessage("Failed rename region.");
        }
    }
}
