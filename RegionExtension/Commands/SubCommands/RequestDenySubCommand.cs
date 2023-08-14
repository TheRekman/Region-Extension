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
    public class RequestDenySubCommand : SubCommand
    {
        public override string[] Names => new string[] { "requestdeny", "rd" };

        public override string Description => "RequestDenyDesc";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new RegionParam("region", "which region will be denied. Default: region in your location", true),
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var region = (Region)Params[0].Value;
            if (!Plugin.RegionExtensionManager.RegionRequestManager.Requests.Any(r => r.Region.ID == region.ID))
            {
                args.Player.SendErrorMessage("Region '{0}' does not have request!".SFormat(region.Name));
                return;
            }
            SendRegionInfo(args, region);
        }

        private void SendRegionInfo(CommandArgsExtension args, Region region)
        {
            if (Plugin.RegionExtensionManager.RemoveRequest(region, args.Player, false))
            {
                args.Player.SendSuccessMessage("Region '{0}' denied!".SFormat(region.Name));
            }
            else
            {
                args.Player.SendErrorMessage("Failed deny region '{0}'!".SFormat(region.Name));
            }
        }
    }
}
