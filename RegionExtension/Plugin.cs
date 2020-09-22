using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace RegionExtension
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        #region plugin overrides
        public override string Author => "Rekman";
        public override string Description => "More region command & functionality";
        public override string Name => "Region Extension";
        public override Version Version => new Version(1, 1, 0, 0);
        #endregion

        #region fields
        private List<ContextCommand> Contexts;
        private List<FastRegion> FastRegions;
        private ConfigFile Config;
        private RegionExtManager ExtManager;
        #endregion

        #region initialize
        public Plugin(Main game) : base(game)
        {

        }

        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.NetGetData.Register(this, OnGetData);
            PlayerHooks.PlayerLogout += OnPlayerLogout;
            PlayerHooks.PlayerCommand += OnPlayerCommand;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
                PlayerHooks.PlayerLogout -= OnPlayerLogout;
                PlayerHooks.PlayerCommand -= OnPlayerCommand;
            }
        }

        private void OnInitialize(EventArgs args)
        {
            Commands.ChatCommands.Add(new Command(Permissions.manageregion, RegionExtenionCmd, "/regionext", "/re"));
            Commands.ChatCommands.Add(new Command("regionext.own", RegionOwningCmd, "/regionown", "/ro"));
            Contexts = new List<ContextCommand>();
            Contexts.Add(new ContextCommand("this", ContextThis));
            Contexts.Add(new ContextCommand("myname", ContextMyName));
            Contexts.Add(new ContextCommand("near", ContextNearPlayer));
            ExtManager = new RegionExtManager(TShock.DB);
            Config = ConfigFile.Read();
            FastRegions = new List<FastRegion>();
    }
        #endregion

        #region hooks & events
        private void OnPlayerLogout(PlayerLogoutEventArgs e)
        {
            int id = FindFastRegionByUser(e.Player.Account);
            if (id != -1) FastRegions.RemoveAt(id);
        }

        private void OnGetData(GetDataEventArgs args)
        {
            int id = FindFastRegionByUser(TShock.Players[args.Msg.whoAmI].Account);
            if (id == -1) return;
            switch (args.MsgID)
            {
                case (PacketTypes.MassWireOperation):
                    using (var reader = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
                    {
                        int startX = reader.ReadInt16();
                        int startY = reader.ReadInt16();
                        int endX = reader.ReadInt16();
                        int endY = reader.ReadInt16();
                        if (startX >= 0 && startY >= 0 && endX >= 0 && endY >= 0 && startX < Main.maxTilesX && startY < Main.maxTilesY && endX < Main.maxTilesX && endY < Main.maxTilesY)
                        {
                            if (FastRegions[id].SetPoints(startX, startY, endX, endY)) FastRegions.RemoveAt(id);
                        }
                        
                    }
                    args.Handled = true;
                    break;
                case (PacketTypes.Tile):
                    using (var reader = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
                    {
                        reader.ReadByte();
                        int x = reader.ReadInt16();
                        int y = reader.ReadInt16();
                        if (x >= 0 && y >= 0 && x < Main.maxTilesX && y < Main.maxTilesY)
                        {
                            if (FastRegions[id].SetPoint(x, y)) FastRegions.RemoveAt(id);
                        }
                    }
                    args.Handled = true;
                    break;
            }
        }

        private int FindFastRegionByUser(UserAccount user)
        {
            for (int i = 0; i < FastRegions.Count; i++)
                if (FastRegions[i].User == user) return i;
                return -1;
        }

        private void OnPlayerCommand(PlayerCommandEventArgs args)
        {
            if (!args.Player.HasPermission(Permissions.manageregion) && !args.Player.HasPermission("regionext.own")) return;
            switch (args.CommandName)
            {
                case "/re":
                case "/regionext":
                case "/ro":
                case "/regionown":
                case "region":
                    for(int i = 1; i < args.Parameters.Count; i++)
                    {
                        if (args.Parameters[i].StartsWith(Config.ContextSpecifier))
                        {
                            bool success = false;
                            for(int j = 0; j < Contexts.Count; j++)
                            {
                                if(Contexts[j].Context == args.Parameters[i].Remove(0, 1))
                                {
                                    Contexts[j].Initialize(args, i);
                                    success = true;
                                    break;
                                }
                            }
                            if (!success)
                            {
                                args.Player.SendErrorMessage("Context: Invalid context command!");
                                args.Handled = true;
                                return;
                            }
                        }
                    }
                    if (Config.AutoCompleteSameName && args.Parameters.Count > 1 && "define" == args.Parameters[0])
                        args.Parameters[1] = AutoCompleteSameName(args.Parameters[1]);
                    break;
            }
        }
        #endregion

        private string AutoCompleteSameName(string oldName)
        {
            string newName = oldName;
            var reg = TShock.Regions.GetRegionByName(newName);
            for (int i = 1; reg != null; i++)
            {
                newName = string.Format(Config.AutoCompleteSameNameFormat, oldName, i);
                reg = TShock.Regions.GetRegionByName(newName);
            }
            return newName;
        }

        #region context
        private void ContextThis(PlayerCommandEventArgs args, int paramID)
        {
            if (args.Player.CurrentRegion == null)
            {
                args.Player.SendErrorMessage("Context: You are not in a region!");
                args.Handled = true;
                return;
            }
            args.Parameters[paramID] = args.Player.CurrentRegion.Name;
        }

        private void ContextMyName(PlayerCommandEventArgs args, int paramID)
            => args.Parameters[paramID] = args.Player.Account.Name;

        private void ContextNearPlayer(PlayerCommandEventArgs args, int paramID)
        {
            if (TShock.Utils.GetActivePlayerCount() == 1)
            {
                args.Player.SendErrorMessage("You are alone on server!");
                args.Handled = true;
                return;
            }

            float x = args.Player.X;
            float y = args.Player.Y;
            var player = TShock.Players.FirstOrDefault(plr => plr != null && plr.Active && plr != args.Player && plr.Account != null);
            if (player == null)
            {
                args.Player.SendErrorMessage("Failed found nearest player!");
                args.Handled = true;
                return;
            }
            float minDistance = CountDistance(x, y, player.X, player.Y);
            for (int i = 0; i < TShock.Players.Length; i++)
                if (TShock.Players[i] != null && TShock.Players[i].Active && TShock.Players[i] != args.Player && TShock.Players[i].Account != null)
                {
                    float distance = CountDistance(x, y, TShock.Players[i].X, TShock.Players[i].Y);
                    if (minDistance > distance)
                    {
                        player = TShock.Players[i];
                        minDistance = distance;
                    }
                }
            args.Parameters[paramID] = player.Account.Name;
        }
        private float CountDistance(float x1, float y1, float x2, float y2) =>
            (float)Math.Sqrt(Math.Pow(Math.Abs(x1 - x2), 2) + Math.Pow(Math.Abs(y1 - y2), 2));
        #endregion

        private void RegionExtenionCmd(CommandArgs args)
        {
            var param = args.Parameters;
            var plr = args.Player;
            if (param.Count < 1) param.Add("help");
            int pCount = param.Count;
            string specifier = TShock.Config.CommandSpecifier;
            string regionName;
            List<string> lines;

            switch (param[0])
            {
                #region rename
                case "rename":
                case "rn":
                    if (pCount != 3)
                    {
                        plr.SendErrorMessage("Invalid syntax! Proper syntax: {0}/re rename <oldname> <newname>", specifier);
                        return;
                    }

                    string newName = param[2];
                    string oldName = param[1];
                    if (TShock.Regions.GetRegionByName(oldName) == null)
                    {
                        plr.SendErrorMessage($"Invalid region \"{oldName}\".");
                        return;
                    }
                    if (newName == oldName)
                    {
                        plr.SendErrorMessage("Both names are the same.");
                        return;
                    }
                    if (TShock.Regions.GetRegionByName(newName) != null)
                    {
                        plr.SendErrorMessage($"Region \"{newName}\" already exists.");
                        return;
                    }
                    if (ExtManager.Rename(oldName, newName))
                    {
                        plr.SendSuccessMessage("Region renamed successfully!");
                    }
                    else
                    {
                        plr.SendErrorMessage("Failed to rename the region!");
                    }
                    break;
                #endregion
                #region move
                case "move":
                    if(pCount != 4)
                    {
                        plr.SendErrorMessage("Invalid syntax! Proper syntax: {0}/re move <regionname> <u/d/r/l> <amount>", specifier);
                        return;
                    }

                    regionName = param[1];
                    var region = TShock.Regions.GetRegionByName(regionName);
                    if (region == null)
                    {
                        plr.SendErrorMessage($"Invalid region \"{param[1]}\".");
                        return;
                    }

                    int amount;
                    if(!int.TryParse(param[3], out amount))
                    {
                        plr.SendErrorMessage("Invalid amount!");
                        return;
                    }
                    int addX = 0;
                    int addY = 0;
                    if (param[2].Contains("u")) addY = -amount;
                    else if (param[2].Contains("d")) addY = amount;
                    if (param[2].Contains("r")) addX = amount;
                    else if (param[2].Contains("l")) addX = -amount;
                    else
                    {
                        plr.SendErrorMessage("Invalid direction! u/d/r/l only!", specifier);
                        return;
                    }

                    if (TShock.Regions.PositionRegion(regionName, region.Area.X + addX, region.Area.Y + addY, region.Area.Width, region.Area.Height))
                        plr.SendSuccessMessage("Region move success!");
                    else plr.SendErrorMessage("Region move failed!");
                    break;
                #endregion
                #region fastregion
                case "fastregion":
                case "fr":
                    if (pCount < 2)
                    {
                        plr.SendErrorMessage("Invalid syntax! Proper syntax: {0}/re fr <regionname> [username] [z] [protect]", specifier);
                        return;
                    }
                    if (FindFastRegionByUser(plr.Account) != -1)
                    {
                        plr.SendErrorMessage("You already have active fastregion request!");
                        return;
                    }
                    regionName = param[1];
                    if(TShock.Regions.GetRegionByName(regionName) != null)
                    {
                        if (Config.AutoCompleteSameName)
                            regionName = AutoCompleteSameName(regionName);
                        else
                        {
                            plr.SendErrorMessage($"Region \"{regionName}\" already exist.");
                            return;
                        }
                    }
                    string ownerName = plr.Account.Name;
                    int z = 0;
                    bool protect = true;
                    if (pCount > 2)
                    {
                        if(TShock.UserAccounts.GetUserAccountByName(param[2]) == null)
                        {
                            plr.SendErrorMessage("Invalid user name!");
                            return;
                        }
                        ownerName = param[2];
                        if (pCount > 3)
                        {
                            if (!int.TryParse(param[3], out z))
                            {
                                plr.SendErrorMessage("Invalid z value!");
                                return;
                            }
                            if (pCount > 4)
                            {
                                if (!bool.TryParse(param[4], out protect))
                                {
                                    plr.SendErrorMessage("Invalid protect value! true/false only.");
                                    return;
                                }
                            }
                        }
                    }
                    FastRegions.Add(new FastRegion(plr, regionName, ownerName, z, protect));
                    break;
                case "frb":
                case "frbreak":
                case "fastregionbreak":
                case "fastregionb":
                    int id = FindFastRegionByUser(plr.Account);
                    if (id == -1)
                    {
                        plr.SendErrorMessage("You dont have active fastregion request!");
                        return;
                    }
                    FastRegions.RemoveAt(id);
                    plr.SendSuccessMessage("Your fastregion request breaked!");
                    break;
                #endregion
                #region clearmembers
                case "clearm":
                case "clearmembers":
                    if(pCount != 2)
                    {
                        plr.SendErrorMessage("Invalid syntax! Proper syntax: {0}/re clearm <regionname>", specifier);
                        return;
                    }
                    if(TShock.Regions.GetRegionByName(param[1]) == null)
                    {
                        plr.SendErrorMessage($"Invalid region \"{param[1]}\".");
                        return;
                    }
                    if (ExtManager.ClearAllowUsers(param[1]))
                        plr.SendSuccessMessage("Region clear member success!");
                    else plr.SendErrorMessage("Region clear member failed!");
                    break;
                #endregion
                #region setowner
                case "setowner":
                    if(pCount != 3)
                    {
                        plr.SendErrorMessage("Invalid syntax! Proper syntax: {0}/re setowner <regionname> <username>", specifier);
                        return;
                    }
                    if(TShock.Regions.GetRegionByName(param[1]) == null)
                    {
                        plr.SendErrorMessage($"Invalid region \"{param[1]}\".");
                        return;
                    }
                    if(TShock.UserAccounts.GetUserAccountByName(param[2]) == null)
                    {
                        plr.SendErrorMessage($"Invalid username \"{param[2]}\".");
                    }
                    if(TShock.Regions.ChangeOwner(param[1], param[2]))
                    {
                        plr.SendSuccessMessage("Region changeowner success!");
                    }
                    else plr.SendErrorMessage("Region changeowner failed!");
                    break;
                #endregion
                #region listown
                case "lo":
                case "listown":
                    if(pCount != 3)
                    {
                        plr.SendErrorMessage("Invalid syntax! Proper syntax: {0}/re lo <playername> <page>", specifier);
                        return;
                    };
                    if (TShock.UserAccounts.GetUserAccountByName(param[1]) == null)
                    {
                        plr.SendErrorMessage($"Invalid username \"{param[1]}\".");
                    };
                    var player = TShock.UserAccounts.GetUserAccountByName(param[1]);
                    var regions = TShock.Regions.Regions.FindAll(reg => reg.Owner == player.Name && reg.WorldID == Main.worldID.ToString());
                    int pageNumberList;
                    if (!PaginationTools.TryParsePageNumber(param, 2, plr, out pageNumberList))
                        return;
                    List<string> regionNamesList = new List<string>();
                    for (int i = 0; i < regions.Count; i++)
                        regionNamesList.Add(regions[i].Name);
                    PaginationTools.SendPage(plr, pageNumberList, PaginationTools.BuildLinesFromTerms(regionNamesList),
                        new PaginationTools.Settings
                        {
                            HeaderFormat = "Player regions ({0}/{1}):",
                            FooterFormat = "Type {0}/re lo {1} {{0}} for more.".SFormat(specifier, param[1]),
                            NothingToDisplayString = "There are currently no regions."
                        });
                    break;
                #endregion
                #region context list
                case "context":
                case "con":
                    int pageNumberCon = 1;
                    if (args.Parameters.Count > 1)
                    {
                        int pageParamIndexCon = 1;
                        if (!PaginationTools.TryParsePageNumber(param, pageParamIndexCon, plr, out pageNumberCon))
                            return;
                    }

                    lines = new List<string> {
                        $"{Config.ContextSpecifier}this - get current region.",
                        $"{Config.ContextSpecifier}myname - get account username."
                        };

                    PaginationTools.SendPage(
                      plr, pageNumberCon, lines,
                      new PaginationTools.Settings
                      {
                          HeaderFormat = "Available contexts command ({0}/{1}):",
                          FooterFormat = "Type {0}/re context {{0}} for more contexts.".SFormat(specifier)
                      }
                    );
                    break;
                #endregion
                #region help
                case "help":
                    int pageNumber;
                    if (!PaginationTools.TryParsePageNumber(param, 1, plr, out pageNumber))
                        return;

                    lines = new List<string> {
                        "rename <oldname> <newname> - Set the name of the region.",
                        "move <regionname> <u/d/r/l> <amount> - Move region coordinate at the given direction.",
                        "fr <regionname> [ownername] [z] [protect] - Defines the region with given points.",
                        "frbreak - Break fastregion request.",
                        "clearm <regionname> - Clear all allowed members at the given region.",
                        "setowner <regionname> <username> - Set region owner."
                        };
                    if (Config.ContextAllow) lines.Add("/re context [page] - Show available contexts command.");

                    PaginationTools.SendPage(
                      plr, pageNumber, lines,
                      new PaginationTools.Settings
                      {
                          HeaderFormat = "Available Region Extension Sub-Commands ({0}/{1}):",
                          FooterFormat = "Type {0}/re help {{0}} for more sub-commands.".SFormat(specifier)
                      }
                    );
                    break;
                #endregion
                default:
                    plr.SendErrorMessage("Invalid sub-command! Check {0}/re help for more information.", specifier);
                    break;
            }
        }

        private void RegionOwningCmd(CommandArgs args)
        {
            var param = args.Parameters;
            var plr = args.Player;
            if (param.Count < 1) param.Add("help");
            int pCount = param.Count;
            string specifier = TShock.Config.CommandSpecifier;
            var regions = TShock.Regions.Regions.FindAll(reg => reg.Owner == plr.Account.Name && reg.WorldID == Main.worldID.ToString());

            switch (param[0])
            {
                #region list
                case "list":
                    int pageNumberList;
                    if (!PaginationTools.TryParsePageNumber(param, 1, plr, out pageNumberList))
                        return;
                    List<string> regionNamesList = new List<string>();
                    for(int i = 0; i < regions.Count; i++)
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

                    List<string> lines = new List<string>
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
                    if(pCount != 3)
                    {
                        plr.SendErrorMessage("Invalid syntax! Proper syntax: {0}/ro allow <name> <region>", specifier);
                        return;
                    }
                    if(regions.FirstOrDefault(reg => reg.Name == param[2]) == null)
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
                    if (ExtManager.ClearAllowUsers(param[1]))
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
                #region context list
                case "context":
                case "con":
                    int pageNumberCon = 1;
                    if (args.Parameters.Count > 1)
                    {
                        int pageParamIndexCon = 1;
                        if (!PaginationTools.TryParsePageNumber(param, pageParamIndexCon, plr, out pageNumberCon))
                            return;
                    }

                    lines = new List<string> {
                        $"{Config.ContextSpecifier}this - get current region.",
                        $"{Config.ContextSpecifier}myname - get account username."
                        };

                    PaginationTools.SendPage(
                      plr, pageNumberCon, lines,
                      new PaginationTools.Settings
                      {
                          HeaderFormat = "Available contexts command ({0}/{1}):",
                          FooterFormat = "Type {0}/re context {{0}} for more contexts.".SFormat(specifier)
                      }
                    );
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
                    if (Config.ContextAllow) lines.Add("context [page] - Show available contexts command.");

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
