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
        public static readonly string WarpTrigger = string.Join('.', RegionTriggerCmd, "warp");
        public static readonly string SpawnNpcTrigger = string.Join('.', RegionTriggerCmd, "spawnnpc");
        public static readonly string ItemTrigger = string.Join('.', RegionTriggerCmd, "giveitem");
        public static readonly string TeleportPositionTrigger = string.Join('.', RegionTriggerCmd, "tppos");
        public static readonly string ProjetileTrigger = string.Join('.', RegionTriggerCmd, "spawnproj");
    }
}
