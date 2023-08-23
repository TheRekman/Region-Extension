using RegionExtension.Commands.Parameters;
using RegionExtension.RegionTriggers.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaApi.Server;
using TShockAPI.DB;

namespace RegionExtension.RegionTriggers.RegionProperties
{
    public interface IRegionProperty
    {
        string[] Names { get; }
        string Description { get; }
        string Permission { get; }
        ICommandParam[] CommandParams { get; }
        Region[] DefinedRegions { get; }
        void InitializeEventHandler(TerrariaPlugin plugin);
        void RemoveRegionProperties(Region region, ICommandParam[] commandParams);
        void AddRegionProperties(Region region, ICommandParam[] commandParams);
        void ClearProperties(Region region);
        ConditionStringPair GetStringArgs(Region region);
        void SetFromString(Region region, ConditionStringPair pair);
        void AddCondition(Region region, ICommandParam[] commandParams, IRegionCondition condition);
        void RemoveCondition(Region region, ICommandParam[] commandParams, IRegionCondition condition);
        void Dispose(Plugin plugin);

    }
}
