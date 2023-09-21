using RegionExtension.Commands;
using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaApi.Server;
using TShockAPI;

namespace RegionExtension.RegionTriggers.Actions
{
    public class CommandAction : ITriggerAction
    {
        public string Name => "cmd";
        public string Description => "Command trigger!";

        public static ActionFormer Former { get; } = new ActionFormer(new[] { "command", "cmd" }, "CommandTriggerDesc",
                                                                      new ICommandParam[] {  new StringParam("command", "command string") },
                                                                      (param, args) => CreateTriggerAction(param, args),
                                                                      s => CreateCommandAction(s, TSPlayer.Server).Action)
                                                                      { Permission = Permissions.TriggerCommand };

        private string _commandString;

        private CommandAction(string commandString)
        {
            _commandString = commandString;
        }

        public static (CommandAction Action, string Message) CreateCommandAction(string commandString, TSPlayer player)
        {
            var commandParams = commandString.Split(' ');
            var cmd = TShockAPI.Commands.ChatCommands.FirstOrDefault(c => c.Name == commandParams[0] || c.Names.Contains(commandParams[0]));
            if (cmd == null)
                return (null, "Command '{0}' was not founded!".SFormat(commandParams[0]));
            if(Plugin.Config.BannedTriggerCommands.Any(c => cmd.Names.Contains(c)))
                return (null, "Command '{0}' was banned for trigger use!".SFormat(cmd.Name));
            bool havePermission = false;
            foreach(var perm in cmd.Permissions)
                if(player.HasPermission(perm))
                {
                    havePermission = true;
                    break;
                }
            if (!havePermission)
                return (null, "You don't have permission for this command!");
            return (new CommandAction(commandString), "");
        }

        public static ITriggerAction CreateTriggerAction(ICommandParam[] param, CommandArgsExtension args)
        {
            string command = (string)param[0].Value;
            var res = CreateCommandAction(command, args.Player);
            if (res.Action == null)
                args.Player.SendErrorMessage(res.Message);
            return CreateCommandAction(command, args.Player).Action;
        }

        public void Execute(TriggerActionArgs args)
        {
            var strCopy = _commandString;
            strCopy = strCopy.Replace("@i", "tsi:" + args.Player.Index);
            strCopy = strCopy.Replace("@p", "\"" + args.Player.Name + "\"");
            strCopy = strCopy.Replace("@r", "\"" + args.Region.Name + "\"");
            try
            {
                TShockAPI.Commands.HandleCommand(TSPlayer.Server, "." + strCopy);
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
                args.Player.SendErrorMessage("Failed initialize trigger command!");
            }
        }

        public string GetArgsString() =>
            _commandString;
    }
}
