using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;
using TShockAPI;

namespace RegionExtension.Commands
{
    public class HelpCmd : SubCommand
    {
        private CommandExtension _targetCommand;

        public HelpCmd(CommandExtension cmd)
        {
            _targetCommand = cmd;
        }

        public override string[] Names => new string[] { "help" };

        public override string Description => "Returns all info about this command.";

        public override ICommandParam[] Params => new ICommandParam[]
        {
            new IntParam("page", "page of the list. Default: 1", true, 1)
        };

        public override void Execute(CommandArgsExtension args)
        {
            var page = (int)Params[0].Value;
            SendHelpList(args, page);
        }

        private void SendHelpList(CommandArgsExtension args, int page)
        {
            var commandsInfo = _targetCommand.SubCommands.Select(sc => "{0} - {1}".SFormat(string.Join(' ', sc.Names),
                                                                                           string.Join(' ', sc.Params.Select(p => p.GetBracketName()))));
            var usedName = args.Message.Split(' ')[0].Remove(0, 1);
            PaginationTools.SendPage(args.Player, page, PaginationTools.BuildLinesFromTerms(commandsInfo),
                        new PaginationTools.Settings
                        {
                            HeaderFormat = "Available '{0}' Sub-Commands ({{0}}/{{1}}):".SFormat(usedName),
                            FooterFormat = "Type {0}{1} help {{0}} for more sub-commands.\nAdditional names:\n{3}"
                                          .SFormat(TShockAPI.Commands.Specifier, usedName, string.Join(' ', _targetCommand.Names))
                        });
        }

        public string FormCommandParameters(ISubCommand subCommands) =>
            string.Join(' ', subCommands.Params.Select(p => p.GetBracketName()));

    }
}
