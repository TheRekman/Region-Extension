using RegionExtension.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.RegionTriggers.Conditions
{
    public class ConditionFormer
    {
        public ConditionFormer(string[] names, string description, ICommandParam[] commandParams, Func<ICommandParam[], bool, IRegionCondition> former, Func<string, IRegionCondition> formerFromString)
        {
            Names = names;
            Description = description;
            CommandParams = commandParams;
            Former = former;
            FormerFromString = formerFromString;
        }

        public string[] Names { get; }
        public string Description { get; }
        public ICommandParam[] CommandParams { get; }
        public Func<ICommandParam[], bool, IRegionCondition> Former { get; }
        public Func<string, IRegionCondition> FormerFromString { get; }
    }
}
