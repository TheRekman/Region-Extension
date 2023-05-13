using RegionExtension.Commands.Parameters;
using System.Linq;
using TShockAPI;

namespace RegionExtension.Commands
{
    internal class HelpSubCmd : SubCommand
    {
        private CommandExtension _targetCommand;

        public HelpSubCmd(CommandExtension cmd)
        {
            _targetCommand = cmd;
        }

        public override string[] Names => new string[] { "helpsc" };

        public override string Description => "Returns all info about this sub-command.";

        public override void InitializeParams()
        {
            _params = new ICommandParam[]
            {
                new StringParam("subcommand", "which sub-command params will be given."),
            };
        }

        public override void Execute(CommandArgsExtension args)
        {
            var subCommandName = (string)Params[0].Value;
            SendHelpList(args, subCommandName);
        }

        private void SendHelpList(CommandArgsExtension args, string subCommandName)
        {
            var subCmd = _targetCommand.SubCommands.FirstOrDefault(sc => sc.Names.Contains(subCommandName));
            if (subCmd == null)
            {
                args.Player.SendErrorMessage("Invalid sub-command '{0}'!".SFormat(subCommandName));
                return;
            }
            var paramsInfo = subCmd.Params.Select(p => "{0} - {1}".SFormat(p.GetBracketName(), p.Description));
            PaginationTools.SendPage(
                      args.Player, 1, paramsInfo.ToList(),
                        new PaginationTools.Settings
                        {
                            HeaderFormat = "Params of '{0}' Sub-Commands:".SFormat(subCommandName)
                        });
        }
    }
}