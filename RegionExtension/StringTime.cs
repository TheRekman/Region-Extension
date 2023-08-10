using NuGet.Packaging.Licenses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension
{
    public class StringTime
    {
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }

        public StringTime(int days, int hours, int minutes, int seconds)
        {
            Days = days;
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
        }

        public static StringTime FromString(string str)
        {
            int days, hours, minutes, seconds;
            days = GetNumAndRemove('d', ref str);
            hours = GetNumAndRemove('h', ref str);
            minutes = GetNumAndRemove('m', ref str);
            seconds = GetNumAndRemove('s', ref str);
            return new StringTime(days, hours, minutes, seconds);
        }

        public override string ToString()
        {
            string res = "";
            if (Days != 0)
                res += Days + "d";
            if (Hours != 0)
                res += Hours + "h";
            if (Minutes != 0)
                res += Minutes + "m";
            if (Seconds != 0)
                res += Seconds + "s";
            if (res == "")
                res = "0";
            return res;
        }

        public static int GetNumAndRemove(char chr, ref string str)
        {
            int res = 0;
            if (str.Contains(chr))
            {
                res = int.Parse(str.Substring(0, str.IndexOf(chr)));
                str = str.Substring(str.IndexOf(chr) + 1);
            }
            return res;
        }

        public static DateTime operator +(DateTime dateTime, StringTime stringTime)
        {
            dateTime = dateTime.AddSeconds(stringTime.Seconds);
            dateTime = dateTime.AddMinutes(stringTime.Minutes);
            dateTime = dateTime.AddHours(stringTime.Hours);
            dateTime = dateTime.AddDays(stringTime.Days);
            return dateTime;
        }

        public static DateTime operator -(DateTime dateTime, StringTime stringTime)
        {
            dateTime = dateTime.AddSeconds(-stringTime.Seconds);
            dateTime = dateTime.AddMinutes(-stringTime.Minutes);
            dateTime = dateTime.AddHours(-stringTime.Hours);
            dateTime = dateTime.AddDays(-stringTime.Days);
            return dateTime;
        }

        public bool IsZero() => 
            Days + Hours + Minutes + Seconds == 0;
    }
}
