using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.RegionTriggers.Conditions
{
    public interface IRegionCondition
    {
        static ConditionFormer ConditionFormer { get; }
        string[] GetNames();
        bool IsReversed();
        bool Check(TSPlayer player, Region region, Trigger trigger = null);
        string GetString();
    }
}
