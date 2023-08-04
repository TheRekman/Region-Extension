using RegionExtension.Commands.Parameters;
using RegionExtension.RegionTriggers.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Commands.SubCommands
{
    public class ConditionList : SubCommand
    {
        public override string[] Names => new string[] { "conditionlist", "cl" };

        public override string Description => "List available conditions.";

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
            var triggers = ConditionManager.Formers.Select(f => "{0} {1} - {2}".SFormat(string.Join('/', f.Names), string.Join(" ", f.CommandParams.Select(p => p.GetBracketName())), f.Description));
            var usedName = args.Message.Split(' ')[0].Remove(0, 1);
            var usedSubCommandName = args.Parameters[0];
            PaginationTools.SendPage(args.Player, page, triggers.ToList(),
                        new PaginationTools.Settings
                        {
                            HeaderFormat = "Conditions ({0}/{1}):",
                            FooterFormat = "Type {0}{1} {2} {{0}} for more."
                                           .SFormat(TShockAPI.Commands.Specifier, usedName, usedSubCommandName),
                            NothingToDisplayString = "There are currently no conditions."
                        });
        }
    }
}
