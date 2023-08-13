using RegionExtension.Commands.Parameters;
using RegionExtension.RegionTriggers;
using RegionExtension.RegionTriggers.RegionProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
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

                new IntParam("page", "page of list. Default: 1", true, 1),
                new RegionParam("region", "region of which triggers will be given. Default: current region", true)
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var page = (int)_params[0].Value;
            var region = (Region)_params[1].Value;
            GetPropertyList(args, region, page);
        }

        private void GetPropertyList(CommandArgsExtension args, Region region, int page)
        {
            var usedName = args.Message.Split(' ')[0];
            var usedSubCommandName = args.Parameters[0];
            var info = Plugin.RegionExtensionManager.PropertyManager.RegionProperties.Where(p => p.DefinedRegions.Contains(region))
                                                                                     .Select(p => (Name: p.Names[0], Conditions: p.GetStringArgs(region).Conditions, Args: p.GetStringArgs(region).Args))
                                                                                     .SelectMany(i => new string[] { i.Name + (i.Conditions.Length != 0 ? " | " + i.Conditions : ""), i.Args })
                                                                                     .ToArray();
            PaginationTools.SendPage(args.Player, page, info,
                        new PaginationTools.Settings
                        {
                            HeaderFormat = "Properties of region '{0}' ({{0}}/{{1}}):".SFormat(region.Name),
                            FooterFormat = "Type {0}{1} {2} {{0}} {3} for more."
                                           .SFormat(TShockAPI.Commands.Specifier, usedName, usedSubCommandName, region.Name),
                            NothingToDisplayString = "There are currently no property."
                        });
        }
    }
}
