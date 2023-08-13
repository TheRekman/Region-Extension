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
            TShockAPI.Commands.ChatCommands.Add(
                    new Command(
                    Permissions.TriggerIgnore,
                    args =>
                    {
                        Plugin.TriggerIgnores[args.Player.Index] = !Plugin.TriggerIgnores[args.Player.Index];
                        args.Player.SendInfoMessage("Trigger ignore is " + (Plugin.TriggerIgnores[args.Player.Index] ? "activated!" : "disabled!"));
                    },
                    "triggerignore", "ti")
                    { HelpText = "Ignores any trigger and property activation." });
            TShockAPI.Commands.ChatCommands.Add(
                    new Command(
                    TShockAPI.Permissions.managegroup,
                    args =>
                    {
                        var num = 0;
                        if (!PaginationTools.TryParsePageNumber(args.Parameters, 0, args.Player, out num))
                            return;
                        var data = PaginationTools.BuildLinesFromTerms(Permissions.GetAllPermissions());
                        PaginationTools.SendPage(args.Player, num, PaginationTools.BuildLinesFromTerms(Permissions.GetAllPermissions()),
                            new PaginationTools.Settings
                            {
                                HeaderFormat = "Region extension permissions ({0}/{1}):",
                                FooterFormat = "Type {0}{1} {{0}} for more."
                                               .SFormat(TShockAPI.Commands.Specifier, "reperm"),
                                NothingToDisplayString = "There are currently no permissions."
                            });
                        foreach (var item in data)
                            TShock.Log.Info("[RegionExtensionPermissions] " + item);
                    },
                    "reperm")
                    { HelpText = "Returns all permissions used by Region Extension plugin" });
            TShockAPI.Commands.ChatCommands.Add(
                    new Command(
                    args =>
                    {
                        var loc = args.Parameters.Count > 0 ? args.Parameters[0].ToUpper() : Plugin.Config.DefaultLocalization;
                        if(!Localization.Languages.ContainsKey(loc))
                        {
                            args.Player.SendErrorMessage("Invalid language '{0}'! Available localizations: {1}".SFormat(loc, string.Join(" ", Localization.Languages.Select(p => p.Key))));
                            return;
                        }
                        Localization.PlayersLocalization[args.Player.TPlayer.whoAmI] = loc;
                        args.Player.SendSuccessMessage("Localization changed to '{0}'!".SFormat(loc));
                    },
                    "reloc")
                    { HelpText = "Changes Region extension localization.", AllowServer = false });
        }
    }
}
