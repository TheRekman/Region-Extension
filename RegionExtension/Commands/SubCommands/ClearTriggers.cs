using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;

namespace RegionExtension.Commands.SubCommands
{
    internal class ClearTriggers : SubCommand
    {
        public override string[] Names => new string[] { "clear", "c" };

        public override string Description => "Clears triggers from the region";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new RegionParam("region", "in which region triggers will be removed.", true)
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var region = (Region)_params[0].Value;
            AddRegionTrigger(args, region);
        }

        private void AddRegionTrigger(CommandArgsExtension args, Region region)
        {
            if (Plugin.RegionExtensionManager.TriggerManager.ClearTriggers(region))
                args.Player.SendSuccessMessage("Triggers cleared!");
            else
                args.Player.SendErrorMessage("Failed clear triggers!");
        }
    }
}
