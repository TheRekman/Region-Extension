using RegionExtension.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace RegionExtension
{
    public static class PluginCommands
    {
        public static Plugin Plugin { get { return _plugin; } }
        private static Plugin _plugin;

        public static void Initialize(Plugin plugin)
        {
            _plugin = plugin;


            InitializeCommands(plugin,
                new RegionExtensionCommand()
                );
            TShockAPI.Commands.ChatCommands.Add(new Command("regionext.own", RegionOwningCmd, "/regionown", "/ro"));
        }

        public static void InitializeCommands(this Plugin plugin, params CommandExtension[] commands)
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

        private static void RegionOwningCmd(CommandArgs args)
        {
            var param = args.Parameters;
            var plr = args.Player;
            if (param.Count < 1) param.Add("help");
            int pCount = param.Count;
            string specifier = TShock.Config.Settings.CommandSpecifier;
            var regions = TShock.Regions.Regions.FindAll(reg => reg.Owner == plr.Account.Name && reg.WorldID == Main.worldID.ToString());

            switch (param[0])
            {
                #region list
                case "list":
                    int pageNumberList;
                    if (!PaginationTools.TryParsePageNumber(param, 1, plr, out pageNumberList))
                        return;
                    var regionNamesList = new List<string>();
                    for (int i = 0; i < regions.Count; i++)
                        regionNamesList.Add(regions[i].Name);
                    PaginationTools.SendPage(plr, pageNumberList, PaginationTools.BuildLinesFromTerms(regionNamesList),
                        new PaginationTools.Settings
                        {
                            HeaderFormat = "Your regions ({0}/{1}):",
                            FooterFormat = "Type {0}/ro list {{0}} for more.".SFormat(specifier),
                            NothingToDisplayString = "There are currently no regions."
                        });
                    break;
                #endregion
                #region info
                case "info":
                    if (pCount == 1 || pCount > 3)
                    {
                        args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}region info <region> [page]", specifier);
                        return;
                    }

                    string regionName = param[1];

                    Region region = regions.FirstOrDefault(reg => reg.Name == regionName);
                    if (region == null)
                    {
                        plr.SendErrorMessage("Invalid region \"{0}\"!", regionName);
                        return;
                    }

                    int pageNumberIndex = 2;
                    int pageNumberInfo;
                    if (!PaginationTools.TryParsePageNumber(param, pageNumberIndex, plr, out pageNumberInfo))
                        return;

                    var lines = new List<string>
                        {
                            string.Format("X: {0}; Y: {1}; W: {2}; H: {3}, Z: {4}", region.Area.X, region.Area.Y, region.Area.Width, region.Area.Height, region.Z),
                            string.Concat("Owner: ", region.Owner),
                            string.Concat("Protected: ", region.DisableBuild.ToString()),
                        };

                    if (region.AllowedIDs.Count > 0)
                    {
                        IEnumerable<string> sharedUsersSelector = region.AllowedIDs.Select(userId =>
                        {
                            UserAccount user = TShock.UserAccounts.GetUserAccountByID(userId);
                            if (user != null)
                                return user.Name;

                            return string.Concat("{ID: ", userId, "}");
                        });
                        List<string> extraLines = PaginationTools.BuildLinesFromTerms(sharedUsersSelector.Distinct());
                        extraLines[0] = "Shared with: " + extraLines[0];
                        lines.AddRange(extraLines);
                    }
                    else
                    {
                        lines.Add("Region is not shared with any users.");
                    }

                    if (region.AllowedGroups.Count > 0)
                    {
                        List<string> extraLines = PaginationTools.BuildLinesFromTerms(region.AllowedGroups.Distinct());
                        extraLines[0] = "Shared with groups: " + extraLines[0];
                        lines.AddRange(extraLines);
                    }
                    else
                    {
                        lines.Add("Region is not shared with any groups.");
                    }

                    PaginationTools.SendPage(
                        plr, pageNumberInfo, lines, new PaginationTools.Settings
                        {
                            HeaderFormat = string.Format("Information About Region \"{0}\" ({{0}}/{{1}}):", region.Name),
                            FooterFormat = string.Format("Type {0}/ro info {1} {{0}} for more information.", specifier, regionName)
                        }
                    );
                    break;
                #endregion
                #region allow
                case "allow":
                    if (pCount != 3)
                    {
                        plr.SendErrorMessage("Invalid syntax! Proper syntax: {0}/ro allow <name> <region>", specifier);
                        return;
                    }
                    if (regions.FirstOrDefault(reg => reg.Name == param[2]) == null)
                    {
                        plr.SendErrorMessage($"Invalid region \"{param[2]}\".");
                        return;
                    }
                    if (TShock.UserAccounts.GetUserAccountByName(param[1]) == null)
                    {
                        plr.SendErrorMessage($"Invalid username \"{param[1]}\".");
                        return;
                    }
                    if (TShock.Regions.AddNewUser(param[2], param[1])) plr.SendSuccessMessage($"Allowed user {param[1]} into {param[2]}");
                    break;
                #endregion
                #region remove
                case "remove":
                    if (pCount != 3)
                    {
                        plr.SendErrorMessage("Invalid syntax! Proper syntax: {0}/ro remove <name> <region>", specifier);
                        return;
                    }
                    if (regions.FirstOrDefault(reg => reg.Name == param[2]) == null)
                    {
                        plr.SendErrorMessage($"Invalid region \"{param[2]}\".");
                        return;
                    }
                    if (TShock.UserAccounts.GetUserAccountByName(param[1]) == null)
                    {
                        plr.SendErrorMessage($"Invalid username \"{param[1]}\".");
                    }
                    if (TShock.Regions.RemoveUser(param[2], param[1])) plr.SendSuccessMessage($"Removed user {param[1]} from {param[2]}");

                    break;
                #endregion
                #region clearmembers
                case "clearmembers":
                case "clearm":
                    if (pCount != 2)
                    {
                        plr.SendErrorMessage("Invalid syntax! Proper syntax: {0}/ro clearm <regionname>", specifier);
                        return;
                    }
                    if (regions.FirstOrDefault(reg => reg.Name == param[1]) == null)
                    {
                        plr.SendErrorMessage($"Invalid region \"{param[1]}\".");
                        return;
                    }
                    if (Plugin.ExtManager.ClearAllowUsers(param[1]))
                        plr.SendSuccessMessage("Region clear member success!");
                    else plr.SendErrorMessage("Region clear member failed!");
                    break;
                #endregion
                #region giveowner
                case "giveowner":
                case "giveown":
                case "gow":
                case "go":
                    if (pCount != 3)
                    {
                        plr.SendErrorMessage("Invalid syntax! Proper syntax: {0}/ro gow <user> <region>", specifier);
                        return;
                    }
                    if (regions.FirstOrDefault(reg => reg.Name == param[2]) == null)
                    {
                        plr.SendErrorMessage($"Invalid region \"{param[2]}\".");
                        return;
                    }
                    if (TShock.UserAccounts.GetUserAccountByName(param[1]) == null)
                    {
                        plr.SendErrorMessage($"Invalid username \"{param[1]}\".");
                    }
                    if (TShock.Regions.ChangeOwner(param[2], param[1]))
                        plr.SendSuccessMessage("Region changeowner success!");
                    else plr.SendErrorMessage("Region changeowner failed!");
                    break;
                #endregion
                #region help
                case "help":
                    int pageNumber;
                    if (!PaginationTools.TryParsePageNumber(param, 1, plr, out pageNumber))
                        return;

                    lines = new List<string> {
                          "list - Lists of all your regions.",
                          "allow <user> <region> - Allows a user to a region.",
                          "remove <user> <region> - Removes a user from a region.",
                          "giveown <user> <region> - Change owner of region (You lose own for region!).",
                          "clearm <regionname> - Removes all allowed users from region",
                          "info <region> - Displays several information about the given region."
                        };

                    PaginationTools.SendPage(
                      plr, pageNumber, lines,
                      new PaginationTools.Settings
                      {
                          HeaderFormat = "Available Region Owning Sub-Commands ({0}/{1}):",
                          FooterFormat = "Type {0}/ro help {{0}} for more sub-commands.".SFormat(specifier)
                      }
                    );
                    break;
                #endregion
                default:
                    plr.SendErrorMessage("Invalid sub-command! Check {0}/ro help for more information.", specifier);
                    break;
            }
        }
    }
}
