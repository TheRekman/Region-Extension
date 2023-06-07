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
    public class RequestAcceptSubCommand : SubCommand
    {
        public override string[] Names => new string[] { "requestaccept", "ra" };

        public override string Description => "Accept region request.";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new RegionParam("region", "which region will be accepted. Default: region in your location", true),
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var region = (Region)Params[0].Value;
            if(!Plugin.RegionExtensionManager.RegionRequestManager.Requests.Any(r => r.Region.ID == region.ID))
            {
                args.Player.SendErrorMessage("Region '{0}' does not have request!".SFormat(region.Name));
                return;
            }
            SendRegionInfo(args, region);
        }

        private void SendRegionInfo(CommandArgsExtension args, Region region)
        {
            if(Plugin.RegionExtensionManager.RemoveRequest(region, args.Player, true))
            {
                args.Player.SendSuccessMessage("Region '{0}' accepted!");
            }
            else
            {
                args.Player.SendErrorMessage("Failed accept region '{0}'!");
            }
        }
    }
}
