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
        public override string[] Names => new[] { "delete", "del" };
        public override string Description => "delete region with given name.";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new RegionParam("regionname", "name of region. Default: region in your location", true),
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var region = (Region)Params[0].Value;
            DeleteRegion(args, region);
        }

        private void DeleteRegion(CommandArgsExtension args, Region region)
        {
            if (args.Plugin.RegionExtensionManager.DeleteRegion(args, region))
                args.Player.SendInfoMessage("Deleted region \"{0}\".".SFormat(region.Name));
            else
                args.Player.SendErrorMessage($"Could not find the region {region.Name}.");
        }
    }
}
