using RegionExtension.Commands.Parameters;
using RegionExtension.RegionTriggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Commands.SubCommands
{
    public class TriggerList : SubCommand
    {
        public override string[] Names => new string[] { "list", "l" };

        public override string Description => "TriggerListDesc";

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
            SendTriggerList(args, page);
        }

        private void SendTriggerList(CommandArgsExtension args, int page)
        {
            var triggers = TriggerManager.Formers.Select(f => "{0} {1} - {2}".SFormat(string.Join("/", f.Names), string.Join(' ', f.Params.Select(p => p.GetBracketName())), Localization.GetStringForPlayer(f.Description, args.Player)));
            var usedName = args.Message.Split(' ')[0];
            var usedSubCommandName = args.Parameters[0];
            PaginationTools.SendPage(args.Player, page, triggers.ToList(),
                        new PaginationTools.Settings
                        {
                            HeaderFormat = "Region triggers ({0}/{1}):",
                            FooterFormat = "Type {0}{1} {2} {{0}} for more."
                                           .SFormat(TShockAPI.Commands.Specifier, usedName, usedSubCommandName),
                            NothingToDisplayString = "There are currently no triggers."
                        });
        }
    }
}
