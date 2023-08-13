using Microsoft.Xna.Framework;
using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Commands.SubCommands
{
    internal class PropertyList : SubCommand
    {
        public override string[] Names => new string[] { "list", "l" };

        public override string Description => "PropertyListDesc";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new IntParam("page", "page of list.", true, 1)
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var page = (int)_params[0].Value;
            ListProperties(args, page);
        }

        private void ListProperties(CommandArgsExtension args, int page)
        {
            var triggers = Plugin.RegionExtensionManager.PropertyManager.RegionProperties.Select(p => "{0} {1} - {2}".SFormat(string.Join('/', p.Names.Select(n => Utils.ColorCommand(n))), string.Join(' ', p.CommandParams.Select(p => p.GetColoredBracketName())), Localization.GetStringForPlayer(p.Description, args.Player)).Replace("  ", " "));
            var usedName = args.Message.Split(' ')[0];
            var usedSubCommandName = args.Parameters[0];
            PaginationTools.SendPage(args.Player, page, triggers.ToList(),
                        new PaginationTools.Settings
                        {
                            HeaderFormat = "Region properties ({0}/{1}):",
                            FooterFormat = "Type {0}{1} {2} {{0}} for more."
                                           .SFormat(TShockAPI.Commands.Specifier, usedName, usedSubCommandName),
                            NothingToDisplayString = "There are currently no regions.",
                            LineTextColor = Color.White
                        });
        }
    }
}
