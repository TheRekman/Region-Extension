using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Commands
{
    public class CommandsInitializer
    {
        public void InitializeCommands(Plugin plugin, params CommandExtension[] commands)
        {
            foreach (var command in commands)
            {
                TShockAPI.Commands.ChatCommands.Add(
                    new Command(
                    command.Permissions.ToList(),
                    args => command.InitializeCommand(new CommandArgsExtension(args, plugin)),
                    command.Names)
                    { HelpText = command.HelpText });
            }
        }
    }
}
