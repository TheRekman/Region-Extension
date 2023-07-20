using RegionExtension.RegionTriggers;
using RegionExtension.RegionTriggers.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Commands.Parameters
{
    internal class RegionEventParam : CommandParam<RegionEvents>
    {
        public RegionEventParam(string name, string description, bool optional = false, RegionEvents defaultValue = RegionEvents.None)
            : base(name, description, optional, defaultValue)
        {
        }

        public override bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            RegionEvents regionEvent;
            if (!Enum.TryParse(str, true, out regionEvent) || regionEvent == RegionEvents.None)
            {
                args.Player.SendErrorMessage("Failed found '{0}' event!".SFormat(str));
                return false;
            }
            _value = regionEvent;
            return true;
        }
    }
}
