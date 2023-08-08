using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.RegionTriggers
{
    public class Function
    {
        string _func;
        static Random _random = new Random();

        public string FunctionString { get => _func; }

        public static Dictionary<string, Func<TSPlayer, Region, int>> ReplacesOnCount = new Dictionary<string, Func<TSPlayer, Region, int>>
        {
            {"@ax", (p, r) => p.TileX },
            {"@aY", (p, r) => p.TileY },
            {"@cx", (p, r) => r.Area.X },
            {"@cy", (p, r) => r.Area.Y },
            {"@w", (p, r) => r.Area.Width },
            {"@h", (p, r) => r.Area.Height },
            {"@r", (p, r) => _random.Next()},
        };

        public static Dictionary<string, Func<TSPlayer, Region, int>> ReplacesOnCreation = new Dictionary<string, Func<TSPlayer, Region, int>>
        {
            {"@lx", (p, r) =>  p.TileX - r.Area.X },
            {"@ly", (p, r) => p.TileY - r.Area.Y },
            {"@gx", (p, r) => p.TileX },
            {"@gy", (p, r) => p.TileY }
        };
        
        private static Dictionary<char, Func<int, int, int>> _priortyActions = new Dictionary<char, Func<int, int, int>>
        {
            {'*', (n1, n2) => n1 * n2 },
            {'/', (n1, n2) => n1 / n2 },
            {'%', (n1, n2) => n1 % n2 },
        };
        
        private static Dictionary<char, Func<int, int, int>> _actions = new Dictionary<char, Func<int, int, int>>
        {
            {'+', (n1, n2) => n1 + n2 },
            {'-', (n1, n2) => n1 - n2 }
        };

        public Function(string function)
        {
            _func = function;
        }

        public static Function GetFunction(TSPlayer player, Region region, string function)
        {
            try 
            {
                foreach (var r in ReplacesOnCreation)
                    function = function.Replace(r.Key, r.Value(player, region).ToString());
                var res = new Function(function);
                res.Count(player, region);
                return res;
            }
            catch
            {
                return null;
            }
        }

        public int Count(TSPlayer player, Region region)
        {
            var function = _func;
            if (function.StartsWith('-'))
                function = "0" + function;
            foreach(var r in ReplacesOnCount)
                function = function.Replace(r.Key, r.Value(player, region).ToString());

            Stack<int> OpenedBracket = new Stack<int>();
            for (int i = 0; i < function.Length; i++)
            {
                if (function[i] == '(')
                    OpenedBracket.Push(i);
                else if (function[i] == ')')
                {
                    var opened = OpenedBracket.Pop();
                    function = function.Take(opened + 1).ToString() + CountWithCheckSkip(player, region, new StringBuilder(function.Substring(i, i - opened))).ToString() + function.Skip(i + 1);
                }
            }
            return CountWithCheckSkip(player, region, new StringBuilder(function));
        }

        private static int CountWithCheckSkip(TSPlayer player, Region region, StringBuilder function)
        {
            for(int i = 0; i < function.Length; i++)
                if (_priortyActions.ContainsKey(function[i]))
                {
                    var leftid = FindFirstLeftKeySym(i, function.ToString());
                    var rightid = FindFirstRightKeySym(i, function.ToString());
                    var leftCorner = function.ToString().Substring(leftid + 1, i - leftid - 1);
                    var rightCorner = function.ToString().Substring(i + 1, rightid - i - 1);
                    var res = _priortyActions[function[i]](int.Parse(leftCorner), int.Parse(rightCorner));
                    function = function.Replace(string.Join("", leftCorner, function[i], rightCorner), res.ToString());
                }
            for (int i = 0; i < function.Length; i++)
                if (_actions.ContainsKey(function[i]))
                {
                    var leftid = FindFirstLeftKeySym(i, function.ToString());
                    var rightid = FindFirstRightKeySym(i, function.ToString());
                    var leftCorner = function.ToString().Substring(leftid + 1, i - leftid - 1);
                    var rightCorner = function.ToString().Substring(i + 1, rightid - i - 1);
                    var res = _actions[function[i]](int.Parse(leftCorner), int.Parse(rightCorner));
                    function = function.Replace(string.Join("", leftCorner, function[i], rightCorner), res.ToString());
                }
            return int.Parse(function.ToString());
        }

        private static int FindFirstLeftKeySym(int i, string function)
        {
            for (int j = i - 1; j >= 0; j--)
                if (_priortyActions.ContainsKey(function[j]) || _actions.ContainsKey(function[j]))
                    return j;
            return -1;
        }

        private static int FindFirstRightKeySym(int i, string function)
        {
            for (int j = i + 1; j < function.Length; j++)
                if (_priortyActions.ContainsKey(function[j]) || _actions.ContainsKey(function[j]))
                    return j;
            return function.Length;
        }
    }
}
