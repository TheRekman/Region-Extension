using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace RegionExtension
{
    [ApiVersion(2, 1)]
    public class RegionExt : TerrariaPlugin
    {

        public override string Author => "Rekman";
        public override string Description => "More region command & functionality";
        public override string Name => "Region Extension";
        public override Version Version => new Version(1,0,0,0);

        private List<ContextCommand> Contexts;
        private List<FastRegion> FastRegions;
        private ConfigFile Config;
        private RegionExtManager ExtManager;
        public RegionExt(Main game) : base(game)
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
            Commands.ChatCommands.Add(new Command(Permissions.manageregion, RegionExtenionCmd, "/re", "/regionext"));
            Contexts = new List<ContextCommand>();
            Contexts.Add(new ContextCommand("this", ContextThis));
            Contexts.Add(new ContextCommand("myname", ContextMyName));
            ExtManager = new RegionExtManager(TShock.DB);
            Config = ConfigFile.Read();
            FastRegions = new List<FastRegion>();
    }

        private void OnPlayerLogout(PlayerLogoutEventArgs e)
        {
            int id = FindFastRegionByUser(e.Player.User);
            if (id != -1) FastRegions.RemoveAt(id);
        }

        private void OnGetData(GetDataEventArgs args)
        {
            int id = FindFastRegionByUser(TShock.Players[args.Msg.whoAmI].User);
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
        private int FindFastRegionByUser(User user)
        {
            for (int i = 0; i < FastRegions.Count; i++)
                if (FastRegions[i].User == user) return i;
                return -1;
        }

        private void OnPlayerCommand(PlayerCommandEventArgs args)
        {
            if (!args.Player.HasPermission(Permissions.manageregion)) return;
            switch (args.CommandName)
            {
                case "/re":
                case "/regionext":
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
            => args.Parameters[paramID] = args.Player.User.Name;

        private void RegionExtenionCmd(CommandArgs args)
        {
            var param = args.Parameters;
            var plr = args.Player;
            if (param.Count < 1) param.Add("help");
            int pCount = param.Count;
            string specifier = TShock.Config.CommandSpecifier;
            string regionName;

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
                        plr.SendErrorMessage("Invalid syntax! Proper syntax: {0}/re fr <regionname> <u/d/r/l> <amount>", specifier);
                        return;
                    }

                    regionName = param[1];
                    var reg = TShock.Regions.GetRegionByName(regionName);
                    if (reg == null)
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

                    if (TShock.Regions.PositionRegion(regionName, reg.Area.X + addX, reg.Area.Y + addY, reg.Area.Width, reg.Area.Height))
                        plr.SendSuccessMessage("Region move success!");
                    else plr.SendErrorMessage("Region move failed!");
                    break;
                #endregion
                #region fastregion
                case "fastregion":
                case "fr":
                    if (pCount < 2)
                    {
                        plr.SendErrorMessage("Invalid syntax! Proper syntax: {0}/re fr <regionname/username> [username] [z] [protect]", specifier);
                        return;
                    }
                    if (FindFastRegionByUser(plr.User) != -1)
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
                    string ownerName = plr.User.Name;
                    int z = 0;
                    bool protect = true;
                    if (pCount > 2)
                    {
                        if(TShock.Users.GetUserByName(param[2]) == null)
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
                    int id = FindFastRegionByUser(plr.User);
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
                    if(TShock.Users.GetUserByName(param[2]) == null)
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
                #region help
                case "help":
                    int pageNumber = 1;
                    if (args.Parameters.Count > 1)
                    {
                        int pageParamIndex = 1;
                        if (!PaginationTools.TryParsePageNumber(param, pageParamIndex, plr, out pageNumber))
                            return;
                    }

                    List<string> lines = new List<string> {
                        "/re rename <oldname> <newname> - Set the name of the region.",
                        "/re move <regionname> <u/d/r/l> <amount> - Move region coordinate at the given direction.",
                        "/re fr <regionname> [ownername] [z] [protect] - Defines the region with given points.",
                        "/re frbreak - Break fastregion request.",
                        "/re clearm <regionname> - Clear all allowed members at the given region.",
                        "/re setowner <regionname> <username> - Set region owner."
                        };

                    PaginationTools.SendPage(
                      plr, pageNumber, lines,
                      new PaginationTools.Settings
                      {
                          HeaderFormat = "Available Region Extension Sub-Commands ({0}/{1}):",
                          FooterFormat = "Type {0}re help {{0}} for more sub-commands.".SFormat(specifier)
                      }
                    );
                    break;
                #endregion
                default:
                    plr.SendErrorMessage("Invalid sub-command! Check {0}/re help for more information.", specifier);
                    break;
            }
        }
    }
}
