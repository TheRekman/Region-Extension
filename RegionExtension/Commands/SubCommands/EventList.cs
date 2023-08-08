using RegionExtension.Commands.Parameters;
using RegionExtension.RegionTriggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Commands.SubCommands
{
    public class EventList : SubCommand
    {
        public override string[] Names => new string[] { "eventlist", "el" };

        public override string Description => "List all available events.";

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
            var triggers = TriggerManager.Events.Select(e => "{0} - {1}".SFormat(string.Join("/", e.Names), e.Description));
            var usedName = args.Message.Split(' ')[0];
            var usedSubCommandName = args.Parameters[0];
            PaginationTools.SendPage(args.Player, page, triggers.ToList(),
                        new PaginationTools.Settings
                        {
                            HeaderFormat = "Region triggers ({0}/{1}):",
                            FooterFormat = "Type {0}{1} {2} {{0}} for more."
                                           .SFormat(TShockAPI.Commands.Specifier, usedName, usedSubCommandName),
                            NothingToDisplayString = "There are currently no regions."
                        });
        }
    }
}
