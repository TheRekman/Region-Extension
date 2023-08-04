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
        static ConditionFormer Former { get; }
        string[] GetNames();
        bool IsReversed();
        bool Check(TSPlayer player, Region region);
        string GetString();
    }
}
