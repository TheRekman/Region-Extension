using RegionExtension.Commands.Parameters;
using RegionExtension.RegionTriggers.Actions;
using RegionExtension.RegionTriggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegionExtension.RegionTriggers.Conditions;
using TShockAPI.DB;

namespace RegionExtension.Commands.SubCommands
{
    internal class AddTriggerCondition : SubCommand
    {
        public override string[] Names => new string[] { "addcond", "ac" };

        public override string Description => "AddConditionTriggerDesc";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new RegionParam("region", "which region will be triggered."),
                new ConditionParam("condition", "which condition will be added."),
                new ArrayParam<int>("ids...", "trigger region ids.", 0, true)
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var region = (Region)_params[0].Value;
            var regionEvent = (IRegionCondition)_params[1].Value;
            var trigger = (int[])_params[2].Value;
            AddRegionTrigger(args, region, regionEvent, trigger);
        }

        private void AddRegionTrigger(CommandArgsExtension args, Region region, IRegionCondition condition, int[] ids)
        {
            if (Plugin.RegionExtensionManager.TriggerManager.AddCondition(region, condition, ids))
                args.Player.SendSuccessMessage("Triggers condition added!");
            else
                args.Player.SendErrorMessage("Failed added some conditions!");
        }
    }
}
