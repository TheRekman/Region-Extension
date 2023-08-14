using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;
using TShockAPI;
using RegionExtension.RegionTriggers;
using RegionExtension.RegionTriggers.Actions;

namespace RegionExtension.Commands.SubCommands
{
    internal class AddTrigger : SubCommand
    {
        public override string[] Names => new string[] { "add", "a" };

        public override string Description => "AddTriggerDesc";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new RegionParam("region", "which region will be triggered."),
                new RegionEventParam("event", "which region event will be used."),
                new TriggerParam("trigger", "which trigger will be added.")
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var region = (Region)_params[0].Value;
            var regionEvent = (RegionEvents)_params[1].Value;
            var trigger = (ITriggerAction)_params[2].Value;
            AddRegionTrigger(args, region, regionEvent, trigger);
        }

        private void AddRegionTrigger(CommandArgsExtension args, Region region, RegionEvents regionEvent, ITriggerAction triggerAction)
        {
            if (Plugin.RegionExtensionManager.TriggerManager.CreateTrigger(region, regionEvent, triggerAction))
                args.Player.SendSuccessMessage("Trigger added!");
            else
                args.Player.SendErrorMessage("Failed add trigger!");
        }
    }
}
