using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension.Trigger.Actions
{
    public interface ITriggerAction
    {
        public string Name { get; }
        public string Description { get; }
        public void Execute(TriggerActionArgs args);
    }
}
