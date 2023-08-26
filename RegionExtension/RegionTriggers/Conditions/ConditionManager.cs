using Mono.Cecil.Cil;
using RegionExtension.Commands.Parameters;
using RegionExtension.RegionTriggers.RegionProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.RegionTriggers.Conditions
{
    public static class ConditionManager
    {
        private static IEnumerable<ConditionFormer> _formers = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(IRegionCondition).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                                                                                                         .Select(t => (ConditionFormer)t.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(p => p.Name == nameof(IRegionCondition.ConditionFormer)).First().GetValue(null));

        public static IEnumerable<ConditionFormer>  Formers { get { return _formers; } }

        public static ICommandParam[] GetParams(string name) =>
            _formers.FirstOrDefault(f => f.Names.Contains(GetName(name).ToLower()))?.CommandParams;

        public static IRegionCondition GetCondition(string name, ICommandParam[] commandParams = null) =>
            _formers.FirstOrDefault(f => f.Names.Contains(GetName(name).ToLower()))?.Former(commandParams, name.StartsWith('!'));

        public static string GenerateConditionsString(this IEnumerable<IRegionCondition> conditions) =>
            string.Join(", ", conditions.Select(c => c.GetString()));

        public static IEnumerable<IRegionCondition> GetRegionConditionsFromString(string str) =>
            str.Split(", ").Select(s => _formers.FirstOrDefault(f => f.Names[0].Equals(GetName(s.Split(' ')[0]), StringComparison.OrdinalIgnoreCase))?.FormerFromString(str)).Where(c => c != null);


        private static string GetName(string str) =>
            str.StartsWith('!') ? str.Substring(1) : str;

        public static bool CheckConditions(this IEnumerable<IRegionCondition> conditions, TSPlayer player, Region region, Trigger trigger = null) =>
            conditions == null || conditions.Count() < 1 || conditions.All(c => c.Check(player, region, trigger));


        public static bool EqualsCondition(this IEnumerable<IRegionCondition> conditions1, IEnumerable<IRegionCondition> conditions2) =>
            conditions1.Count() == conditions2.Count() && conditions1.All(c => conditions2.Any(c2 => c.GetNames()[0].Equals(c2.GetNames()[0])));
    }
}
