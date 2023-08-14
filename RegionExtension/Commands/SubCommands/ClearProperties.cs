using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;

namespace RegionExtension.Commands.SubCommands
{
    public class ClearProperties : SubCommand
    {
        public override string[] Names => new string[] { "clear", "c" };

        public override string Description => "ClearPropertiesDesc";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new RegionParam("region", "in which region properties will be removed.", true)
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var region = (Region)_params[0].Value;
            AddRegionTrigger(args, region);
        }

        private void AddRegionTrigger(CommandArgsExtension args, Region region)
        {
            if (Plugin.RegionExtensionManager.PropertyManager.RemoveAllProperties(region))
                args.Player.SendSuccessMessage("Properties cleared!");
            else
                args.Player.SendErrorMessage("Failed clear properties!");
        }
    }
}
