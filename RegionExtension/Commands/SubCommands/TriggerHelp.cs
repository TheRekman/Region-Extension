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
    internal class TriggerHelp : SubCommand
    {
        public override string[] Names => new string[] { "helptrigger", "ht" };

        public override string Description => "TriggerHelpDesc";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new TriggerFormerParam("trigger", "which trigger params will be given."),
                new IntParam("page", "page of help.", true, 1),
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var trigger = (ActionFormer)Params[0].Value;
            var page = (int)Params[1].Value;
            SendHelpList(args, trigger, page);
        }

        private void SendHelpList(CommandArgsExtension args, ActionFormer trigger, int page)
        {
            var paramsInfo = trigger.Params.Select(p => "{0} - {1}".SFormat(p.GetColoredBracketName(), p.Description));
            PaginationTools.SendPage(
                      args.Player, page, paramsInfo.ToList(),
                        new PaginationTools.Settings
                        {
                            HeaderFormat = "Params of '{0}' Trigger:".SFormat(trigger.Names[0]),
                            NothingToDisplayString = "This property don't require any param"
                        });
        }
    }
}
