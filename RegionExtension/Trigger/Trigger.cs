using RegionExtension.Trigger.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension.Trigger
{
    public class Trigger
    {
        public RegionEvents Event { get; set; }
        public ITriggerAction Action { get; set; }
    }
}
