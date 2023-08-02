using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;

namespace RegionExtension.Commands.SubCommands
{
    internal class RemoveProperty : SubCommand
    {
        public override string[] Names => new string[] { "remove", "r" };

        public override string Description => "Remove property from the region";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new RegionParam("region", "which region will be triggered."),
                new PropertyParam("property", "which property will be removed.")
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var region = (Region)_params[0].Value;
            var trigger = (PropertyFormer)_params[1].Value;
            RemoveRegionProperty(args, region, trigger);
        }

        private void RemoveRegionProperty(CommandArgsExtension args, Region region, PropertyFormer former)
        {
            if (Plugin.RegionExtensionManager.PropertyManager.RemoveRegionProperties(region, former.Name, former.Params))
                args.Player.SendSuccessMessage("Property removed!");
            else
                args.Player.SendErrorMessage("Failed remove property!");
        }
    }
}
