using RegionExtension.RegionTriggers.Actions;
using RegionExtension.RegionTriggers.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;

namespace RegionExtension.RegionTriggers
{
    public class Trigger
    {
        public int Id { get; set; }
        public int LocalId { get; set; }
        public Region Region { get; set; }
        public RegionEvents Event { get; set; }
        public ITriggerAction Action { get; set; }
        public IEnumerable<IRegionCondition> Conditions { get; set; }  = new List<IRegionCondition>();

        public Trigger(int id, int localId, Region region, RegionEvents @event, ITriggerAction action)
        {
            Id = id;
            LocalId = localId;
            Region = region;
            Event = @event;
            Action = action;
        }
    }
}
