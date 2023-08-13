using RegionExtension.Commands.Parameters;
using RegionExtension.RegionTriggers.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;

namespace RegionExtension.Commands.SubCommands
{
    public class RemovePropertyCondition : SubCommand
    {
        public override string[] Names => new string[] { "removecond", "rc" };

        public override string Description => "RemovePropertyConditionDesc";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new RegionParam("region", "which region will be edited."),
                new ConditionParam("condition", "which condition will be added."),
                new PropertyParam("property", "which property condition will be added")
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var region = (Region)_params[0].Value;
            var regionEvent = (IRegionCondition)_params[1].Value;
            var trigger = (PropertyFormer)_params[2].Value;
            RemoveRegionProperty(args, region, regionEvent, trigger);
        }

        private void RemoveRegionProperty(CommandArgsExtension args, Region region, IRegionCondition condition, PropertyFormer former)
        {
            if (Plugin.RegionExtensionManager.PropertyManager.RemoveRegionCondition(region, former.Name, former.Params, condition))
                args.Player.SendSuccessMessage("Property condition removed!");
            else
                args.Player.SendErrorMessage("Failed remove some conditions!");
        }
    }
}
