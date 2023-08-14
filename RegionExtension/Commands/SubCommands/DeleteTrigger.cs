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
    internal class DeleteTrigger : SubCommand
    {
        public override string[] Names => new string[] { "delete", "d" };

        public override string Description => "DeleteTriggerDesc";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new RegionParam("region", "in which region trigger will be removed."),
                new IntParam("id", "trigger id to delete.")
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var region = (Region)_params[0].Value;
            var id = (int)_params[1].Value;
            AddRegionTrigger(args, region, id);
        }

        private void AddRegionTrigger(CommandArgsExtension args, Region region, int id)
        {
            if (Plugin.RegionExtensionManager.TriggerManager.RemoveTrigger(region, id))
                args.Player.SendSuccessMessage("Trigger deleted!");
            else
                args.Player.SendErrorMessage("Failed delete trigger!");
        }
    }
}
