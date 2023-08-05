using IL.Terraria.DataStructures;
using Microsoft.Xna.Framework;
using RegionExtension.Commands;
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
            int r = (int)Math.Floor(firstClr.R + (secondClr.R - firstClr.R) * pos);
            int g = (int)Math.Floor(firstClr.G + (secondClr.G - firstClr.G) * pos);
            int b = (int)Math.Floor(firstClr.B + (secondClr.B - firstClr.B) * pos);
            var hex = Color.FromNonPremultiplied(r, g, b, 255).Hex3();
            if(pos < 0 || pos > 1)
            {
                r = 255;
                g = 255;
                b = 255;
            }
            if(str.Contains("]"))
            {
                var strs = str.Split(']');
                var res = string.Join($"[c/{hex}:]]", strs.Select(s => string.IsNullOrEmpty(s) ? "" : $"[c/{hex}:{s}]"));
                return res;
            }
            return $"[c/{hex}:{str}]";
        }

        public static (bool res, string msg) CheckConfigConditions(TSPlayer player, Region region)
        {
            var count = Plugin.RegionExtensionManager.RegionRequestManager.Requests.Count(r => r.User.ID == player.Account.ID);
            var area = region.Area.Width * region.Area.Height;
            var settings = GetSettingsByTSPlayer(player);
            if(settings.MaxRequestCount != 0 && count >= settings.MaxRequestCount)
                return new (false, "You already have '{0}' requests!".SFormat(count));
            if(settings.MaxRequestArea != 0 && area > settings.MaxRequestArea)
                return new (false, "Max region area is '{0}' tiles! Your is '{1}'.".SFormat(settings.MaxRequestArea, area));
            if (settings.MaxRequestWidth != 0 && region.Area.Width > settings.MaxRequestWidth)
                return new(false, "Max region width is '{0}' tiles! Your is '{1}'.".SFormat(settings.MaxRequestWidth, region.Area.Width));
            if (settings.MaxRequestHeight != 0 && region.Area.Height > settings.MaxRequestHeight)
                return new(false, "Max region height is '{0}' tiles! Your is '{1}'.".SFormat(settings.MaxRequestWidth, region.Area.Height));
            return (true, "All checks passed");
        }

        public static RequestSettings GetSettingsByTSPlayer(TSPlayer plr)
        {
            var settings = Plugin.Config.RequestSettings.FirstOrDefault(r => r.GroupName == plr?.Group.Name);
            if (settings == null)
                settings = Plugin.Config.RequestSettings.FirstOrDefault(r => r.GroupName == "default");
            if (settings == null)
                settings = new RequestSettings();
            return settings;
        }

        public static RequestSettings GetSettingsByUserAccount(UserAccount account)
        {
            var settings = Plugin.Config.RequestSettings.FirstOrDefault(r => r.GroupName == account?.Group);
            if (settings == null)
                settings = Plugin.Config.RequestSettings.FirstOrDefault(r => r.GroupName == "default");
            if (settings == null)
                settings = new RequestSettings();
            return settings;
        }

        public static string GetGradientByDateTime(string str, DateTime start, DateTime end)
        {
            var dateNow = DateTime.UtcNow;
            var pos = (dateNow - start).TotalSeconds / (end - start).TotalSeconds;
            return GetGradientByPos(str, pos);
        }

        public static bool TryAutoComplete(string str, out string result)
        {
            if (!Plugin.Config.AutoCompleteSameName)
            {
                result = str;
                return !TShock.Regions.Regions.Any(r => r.Name.ToLower().Equals(str.ToLower()));
            }
            int num = 0;
            string res = str;
            while (TShock.Regions.Regions.Any(r => r.Name.ToLower().Equals(res.ToLower())))
            {
                res = Plugin.Config.AutoCompleteSameNameFormat.SFormat(str, num);
                num++;
            }
            result = res;
            return true;
        }
    }
}
