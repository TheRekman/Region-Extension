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
    }
}
