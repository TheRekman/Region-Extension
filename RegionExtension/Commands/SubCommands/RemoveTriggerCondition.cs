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
    internal class RemoveTriggerCondition : SubCommand
    {
        public override string[] Names => new string[] { "removecond", "rc" };

        public override string Description => "RemoveTriggerConditionDesc";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new RegionParam("region", "which region will be triggered."),
                new ConditionParam("condition", "which condition will be removed."),
                new ArrayParam<int>("ids...", "trigger region ids.", 0, true)
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var region = (Region)_params[0].Value;
            var regionEvent = (IRegionCondition)_params[1].Value;
            var trigger = (int[])_params[2].Value;
            RemoveConditions(args, region, regionEvent, trigger);
        }

        private void RemoveConditions(CommandArgsExtension args, Region region, IRegionCondition condition, int[] ids)
        {
            if (Plugin.RegionExtensionManager.TriggerManager.RemoveCondition(region, condition, ids))
                args.Player.SendSuccessMessage("Triggers condition removed!");
            else
                args.Player.SendErrorMessage("Failed remove some conditions!");
        }
    }
}
