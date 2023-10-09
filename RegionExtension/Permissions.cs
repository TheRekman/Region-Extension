using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension
{
    public static class Permissions
    {
        public static readonly string Main = "regionext";
        public static readonly string RegionExtCmd = TShockAPI.Permissions.manageregion;
        public static readonly string RegionOwnCmd = string.Join('.', Main, "own");
        public static readonly string RegionTriggerCmd = string.Join('.', Main, "trigger");
        public static readonly string RegionPropertyCmd = string.Join('.', Main, "property");
        public static readonly string RegionHistoryCmd = string.Join('.', Main, "history");
        public static readonly string TriggerIgnore = string.Join('.', RegionTriggerCmd, "ignore");
        public static readonly string TriggerSendPacket = string.Join('.', RegionTriggerCmd, "sendpacket");
        public static readonly string TriggerMessage = string.Join('.', RegionTriggerCmd, "message");
        public static readonly string TriggerPush = string.Join('.', RegionTriggerCmd, "push");
        public static readonly string TriggerCommand = string.Join('.', RegionTriggerCmd, "command");
        public static readonly string TriggerWarp = string.Join('.', RegionTriggerCmd, "warp");
        public static readonly string TriggerSpawnNpc = string.Join('.', RegionTriggerCmd, "spawnnpc");
        public static readonly string TriggerItem = string.Join('.', RegionTriggerCmd, "giveitem");
        public static readonly string TriggerTeleportPosition = string.Join('.', RegionTriggerCmd, "tppos");
        public static readonly string TriggerProjetile = string.Join('.', RegionTriggerCmd, "spawnproj");
        public static readonly string TriggerKill = string.Join('.', RegionTriggerCmd, "kill");
        public static readonly string TriggerBuff = string.Join('.', RegionTriggerCmd, "buff");
        public static readonly string TriggerPvp = string.Join(".", RegionPropertyCmd, "pvp");
        public static readonly string PropertyPvp = string.Join(".", RegionPropertyCmd, "pvp");
        public static readonly string PropertyBanHostile = string.Join(".", RegionPropertyCmd, "banhostile");
        public static readonly string PropertySpawnRewrite = string.Join(".", RegionPropertyCmd, "spawnrewrite");
        public static readonly string PropertyProjectile = string.Join(".", RegionPropertyCmd, "projban");
        public static readonly string PropertyItem = string.Join(".", RegionPropertyCmd, "itemban");
        public static readonly string PropertyMaxSpawn = string.Join(".", RegionPropertyCmd, "maxspawn");
        public static readonly string PropertyBlockTileFrame = string.Join(".", RegionPropertyCmd, "blocktileframe");
        public static readonly string PropertyBlockDoorToggle = string.Join(".", RegionPropertyCmd, "blockdoortoggle");


        private static readonly IEnumerable<string> PermissionList = typeof(Permissions).GetFields(System.Reflection.BindingFlags.Static |
                                                                                                   System.Reflection.BindingFlags.Public |
                                                                                                   System.Reflection.BindingFlags.GetField)
                                                                                        .Select(f => (string)f.GetValue(null));

        public static string ResetSectionTrigger { get; internal set; }

        public static IEnumerable<string> GetAllPermissions() => 
            typeof(Permissions).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.GetField).Select(f => (string)f.GetValue(null));
    }
}
