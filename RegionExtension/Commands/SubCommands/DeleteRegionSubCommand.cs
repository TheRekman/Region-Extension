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
    public class DeleteRegionSubCommand : SubCommand
    {
        private bool _checkRegionOwn;

        public override string[] Names => new[] { "delete", "del" };
        public override string Description => "Deletes the given region.";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new RegionParam("region", "name of region. Default: region in your location", true),
            };
        }

        public DeleteRegionSubCommand(bool checkRegionOwn = false)
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
            DeleteRegion(args, region);
        }

        private void DeleteRegion(CommandArgsExtension args, Region region)
        {
            if (Plugin.RegionExtensionManager.DeleteRegion(args, region))
                args.Player.SendInfoMessage("Deleted region \"{0}\".".SFormat(region.Name));
            else
                args.Player.SendErrorMessage($"Could not find the region {region.Name}.");
        }

        public bool CheckRegionOwn(CommandArgsExtension args, Region region)
            => region.Owner == args.Player.Account.Name;
    }
}
