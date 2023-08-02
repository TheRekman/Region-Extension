using RegionExtension.Commands.Parameters;
using RegionExtension.RegionTriggers.Actions;
using RegionExtension.RegionTriggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;

namespace RegionExtension.Commands.SubCommands
{
    internal class AddProperty : SubCommand
    {
        public override string[] Names => new string[] { "add", "a" };

        public override string Description => "Adds property to the region";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new RegionParam("region", "which region will be triggered."),
                new PropertyParam("property", "which property will be added.")
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var region = (Region)_params[0].Value;
            var trigger = (PropertyFormer)_params[1].Value;
            AddRegionProperty(args, region,  trigger);
        }

        private void AddRegionProperty(CommandArgsExtension args, Region region, PropertyFormer former)
        {
            if (Plugin.RegionExtensionManager.PropertyManager.AddRegionProps(region, former.Name, former.Params))
                args.Player.SendSuccessMessage("Property added!");
            else
                args.Player.SendErrorMessage("Failed add property!");
        }
    }
}
