using IL.Terraria.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension
{
    public static class Utils
    {
        public static string AutoCompleteSameName(string oldName, string format)
        {
            string newName = oldName;
            var reg = TShock.Regions.GetRegionByName(newName);
            for (int i = 1; reg != null; i++)
            {
                newName = string.Format(format, oldName, i);
                reg = TShock.Regions.GetRegionByName(newName);
            }
            return newName;
        }

        public static float CountDistance(float x1, float y1, float x2, float y2) =>
            (float)Math.Sqrt(Math.Pow(Math.Abs(x1 - x2), 2) + Math.Pow(Math.Abs(y1 - y2), 2));

        public static string ColorCommand(string str) =>
            string.Format("[c/{0}:{1}]", "b3c9ff", str);

        public static string DateFormat { get { return "dd.MM.yyyy HH:mm:ss UTC+0"; } }
    }
}
