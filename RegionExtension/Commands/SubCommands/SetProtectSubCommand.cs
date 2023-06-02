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
    internal class SetProtectSubCommand : SubCommand
    {
        public override string[] Names => new[] { "protect", "p" };
        public override string Description => "Sets whether the tiles inside the region are protected or not.";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new BoolParam("state", "can user build on region. true/false"),
                new RegionParam("regionn", "name of region. Default: region in your location", true),
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var state = (bool)Params[0].Value;
            var region = (Region)Params[1].Value;
            DeleteRegion(args, region, state);
        }

        private void DeleteRegion(CommandArgsExtension args, Region region, bool state)
        {
            if (args.Plugin.RegionExtensionManager.Protect(args, region, state))
                args.Player.SendInfoMessage("Region '{0}' protect changed .".SFormat(region.Name));
            else
                args.Player.SendErrorMessage($"Region protect change failed.");
        }
    }
}
