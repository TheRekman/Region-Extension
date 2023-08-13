using RegionExtension.Commands.Parameters;
using RegionExtension.RegionTriggers.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Commands.SubCommands
{
    public class PropertyHelp : SubCommand
    {
        public override string[] Names => new string[] { "helpproperty", "hp" };

        public override string Description => "PropertyHelpDesc";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new PropertyParam("property", "which property params will be given.", ignoreParamSet:true),
                new IntParam("page", "page of help.", true, 1),
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var trigger = (PropertyFormer)Params[0].Value;
            var page = (int)Params[1].Value;
            SendHelpList(args, trigger, page);
        }

        private void SendHelpList(CommandArgsExtension args, PropertyFormer property, int page)
        {
            var paramsInfo = property.Params.Select(p => "{0} - {1}".SFormat(p.GetColoredBracketName(), p.Description));
            PaginationTools.SendPage(
                      args.Player, page, paramsInfo.ToList(),
                        new PaginationTools.Settings
                        {
                            HeaderFormat = "Params of '{0}' property:".SFormat(property.Name),
                            NothingToDisplayString = "This property don't require any param"
                        });
        }
    }
}
