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
        private static IEnumerable<ConditionFormer> _formers = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => typeof(IRegionCondition).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                                                                                                                  .Select(t => (ConditionFormer)t.GetProperties(BindingFlags.Static).Where(p => p.Name == nameof(IRegionCondition.Former)).First().GetValue(null)));

        public static IEnumerable<ConditionFormer>  Formers { get { return _formers; } }

        public static ICommandParam[] GetParams(string name)
        {
            var nameCheck = name.StartsWith('!') ? name.Substring(1) : name;
            return _formers.FirstOrDefault(f => f.Names.Contains(nameCheck))?.CommandParams;
        }

        public static IRegionCondition GetCondition(string name, ICommandParam[] commandParams = null)
        {
            var nameCheck = name.StartsWith('!') ? name.Substring(1) : name;
            return _formers.FirstOrDefault(f => f.Names.Contains(nameCheck))?.Former(commandParams, name.StartsWith('!'));
        }
    }
}
