using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension.RegionTriggers.RegionProperties
{
    public class ConditionStringPair
    {
        public ConditionStringPair(string conditions, string args)
        {
            Conditions = conditions;
            Args = args;
        }

        public string Conditions { get; }
        public string Args { get; }
    }
}
