using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Chat.Commands;
using TShockAPI;

namespace RegionExtension.Commands
{
    public abstract class CommandExtension
    {
        public virtual string[] Names { get; private set; }
        public virtual string[] Permissions { get; private set; }
        public virtual ISubCommand[] SubCommands { get; private set; }

        public void InitializeCommand(CommandArgsExtension args)
        {
            var subCommandName = args.Parameters.Count > 0 ? args.Parameters[0] : "help";
            var subCommand = SubCommands.FirstOrDefault(cmd => cmd.Names.Contains(subCommandName));
            var usedName = args.Message.Split(' ')[0].Remove(0, 1);
            if (subCommand == null)
                args.Player.SendErrorMessage(string.Format("Invalid sub-command '{0}'! Use '{1}{2} help' for more information.",
                                                            args.Message, TShockAPI.Commands.Specifier, usedName));
            else
                subCommand.InitializeCommand(args);
        }
        
        public void Help(CommandArgs args)
        {
            int page;
            if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out page))
                return;
            var subCommandsInfo = SubCommands.Select(cmd => string.Format("{0} {1} - {2}", string.Join(' ', cmd.Names),
                                                            FormCommandParameters(cmd), cmd.Description));
            var usedName = args.Message.Split(' ')[0].Remove(0, 1);
            PaginationTools.SendPage(
                      args.Player, page, subCommandsInfo.ToList(),
                      new PaginationTools.Settings
                      {
                          HeaderFormat = "Available '{0}' Sub-Commands ({{0}}/{{1}}):".SFormat(usedName),
                          FooterFormat = "Type {0}{1} help {{0}} for more sub-commands.\nAdditional names:\n{3}"
                                          .SFormat(TShockAPI.Commands.Specifier, usedName, string.Join(' ', Names))
                      }
                    );
        }

        public string FormCommandParameters(ISubCommand subCommands) =>
            string.Join(' ', subCommands.Params.Select(p => string.Format(p.Optional ? "[{0}]" : "<{0}>", p.Name)));
    }
}
