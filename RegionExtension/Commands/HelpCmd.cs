using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;
using TShockAPI;
using Microsoft.Xna.Framework;

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

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new IntParam("page", "page of the list. Default: 1", true, 1)

            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var page = ((IntParam)Params[0]).TValue;
            SendHelpList(args, page);
        }

        private void SendHelpList(CommandArgsExtension args, int page)
        {
            var commandsInfo = _targetCommand.SubCommands.Select(sc => "{0} {1} - {2}".SFormat(string.Join(' ', sc.Names),
                                                                                               string.Join(' ', sc.Params.Select(p => p.GetBracketName())),
                                                                                               sc.Description));
            var usedName = args.Message.Split(' ')[0].Remove(0, 1);
            PaginationTools.SendPage(args.Player, page, commandsInfo.ToList(),
                        new PaginationTools.Settings
                        {
                            HeaderFormat = "Available '{0}' Sub-Commands ({{0}}/{{1}}):".SFormat(usedName),
                            FooterFormat = "Type {0}{1} help {{0}} for more sub-commands."
                                          .SFormat(TShockAPI.Commands.Specifier, usedName),
                        });
            args.Player.SendInfoMessage("Additional names: " + string.Join(' ', _targetCommand.Names));
        }

        public string FormCommandParameters(ISubCommand subCommands) =>
            string.Join(' ', subCommands.Params.Select(p => p.GetBracketName()));

    }
}
