using RegionExtension.Commands.Parameters;
using RegionExtension.RegionTriggers.RegionProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Commands.SubCommands
{
    internal class PropertyInfo : SubCommand
    {
        public override string[] Names => new string[] { "info", "i" };

        public override string Description => "Info about property of the region";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new StringParam("property", "which property will be given."),
                new RegionParam("region", "region of which triggers will be given. Default: current region", true),
                new IntParam("page", "page of list. Default: 1", true, 1)
            };
        }

        public override void Execute(CommandArgsExtension args)
        {

            var propertyName = (string)_params[0].Value;
            var region = (Region)_params[1].Value;
            var page = (int)_params[2].Value;
            var property = Plugin.RegionExtensionManager.PropertyManager.RegionProperties.FirstOrDefault(p => p.Names.Contains(propertyName));
            if(property == null)
            {
                args.Player.SendErrorMessage("Failed found property '{0}'!".SFormat(propertyName));
                return;
            }
            GetPropertyList(args, property, region, page);
        }

        private void GetPropertyList(CommandArgsExtension args, IRegionProperty property, Region region, int page)
        {
            var usedName = args.Message.Split(' ')[0].Remove(0, 1);
            var usedSubCommandName = args.Parameters[0];
            PaginationTools.SendPage(args.Player, page, property.GetStringArgs(region).Split(' '),
                        new PaginationTools.Settings
                        {
                            HeaderFormat = "Property {0} of region {1} ({{0}}/{{1}}):".SFormat(property.Names[0], region.Name),
                            FooterFormat = "Type {0}{1} {2} {{0}} {3} for more."
                                           .SFormat(TShockAPI.Commands.Specifier, usedName, usedSubCommandName, region.Name),
                            NothingToDisplayString = "There are currently no settings for {0} property.".SFormat(property.Names[0])
                        });
        }
    }
}
