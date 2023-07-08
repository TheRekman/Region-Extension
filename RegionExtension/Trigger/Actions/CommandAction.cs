using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Trigger.Actions
{
    public class CommandAction : ITriggerAction
    {
        public string Name => "cmd";

        public string Description => "Failed initialize trigger command!";

        private string _commandString;

        private CommandAction(string commandString)
        {
            _commandString = commandString;
        }

        public static (CommandAction action, string msg) CreateCommandAction(string commandString)
        {
            var commandParams = commandString.Split(' ');
            if(!TShockAPI.Commands.ChatCommands.Any(c => c.Name == commandString || c.Names.Contains(commandString)))
                return (null, "Command was not founded!");
            return (new CommandAction(commandString), "");
        }

        public void Execute(TriggerActionArgs args)
        {
            _commandString = _commandString.Replace("@p", args.Player.Name);
            _commandString = _commandString.Replace("@r", args.Region.Name);
            try
            {
                TShockAPI.Commands.HandleCommand(TSPlayer.Server, _commandString);
            } catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
                args.Player.SendErrorMessage("Failed initialize trigger command!");
            }
        }
    }
}
