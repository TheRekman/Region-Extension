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
        public static void InitializeCommands(Plugin plugin, params CommandExtension[] commands)
        {
            foreach (var command in commands)
            {
                foreach(var name in command.Names) 
                    TShockAPI.Commands.ChatCommands.RemoveAll(c => c.Names.Contains(name) || c.Name == name);
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
