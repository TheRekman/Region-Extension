using IL.Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension
{
    public static class Utils
    {
        public const string ColorTagFormat = "[c/{0}:{1}]";

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

        public static string DateFormat { get { return "dd.MM.yyyy HH:mm:ss UTC+0"; } }
        public static string ShortDateFormat { get { return "dd.MM"; } }

        public static string ColorCommand(string str) =>
            ColorTagFormat.SFormat("b3c9ff", str);
        public static string ColorRegion(string str) =>
            ColorTagFormat.SFormat("d6d160", str);
        public static string ColorDate(string str) =>
            ColorTagFormat.SFormat("5cb5a3", str);

        public static string GetGradientByPos(string str, double pos)
        {
            var firstClr = Color.White;
            var secondClr = Color.Red;
            var r = firstClr.R + (secondClr.R - firstClr.R) * pos;
            var g = firstClr.G + (secondClr.G - firstClr.G) * pos;
            var b = firstClr.B + (secondClr.B - firstClr.B) * pos;
            var hex = firstClr.Hex3();
            return $"[c/{hex}:{str}]";
        }

        public static (bool res, string msg) CheckConfigConditions(TSPlayer player, Region region)
        {
            var count = Plugin.RegionExtensionManager.RegionRequestManager.Requests.Count(r => r.User.ID == player.Account.ID);
            var area = region.Area.Width * region.Area.Height;
            if(count > Plugin.Config.MaxRequestCount)
                return new (false, "You already have '{0}' requests!".SFormat(count));
            if(area > Plugin.Config.MaxRequestArea)
                return new (false, "Max region area is '{0}' tiles! Your is '{1}'.".SFormat(Plugin.Config.MaxRequestArea, area));
            if (region.Area.Width > Plugin.Config.MaxRequestWidth)
                return new(false, "Max region width is '{0}' tiles! Your is '{1}'.".SFormat(Plugin.Config.MaxRequestWidth, region.Area.Width));
            if (region.Area.Height > Plugin.Config.MaxRequestHeight)
                return new(false, "Max region height is '{0}' tiles! Your is '{1}'.".SFormat(Plugin.Config.MaxRequestWidth, region.Area.Height));
            return (true, "All checks passed");
        }

        public static string GetGradientByDateTime(string str, DateTime start, DateTime end)
        {
            var dateNow = DateTime.UtcNow;
            var pos = (end - start).TotalSeconds / (dateNow - start).TotalSeconds;
            return GetGradientByPos(str, pos);
        }
    }
}
