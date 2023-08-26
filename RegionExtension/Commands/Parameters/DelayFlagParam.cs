using RegionExtension.RegionTriggers;
using RegionExtension.RegionTriggers.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Commands.Parameters
{
    public class DelayFlagParam : CommandParam<string>
    {
        public DelayFlagParam(string name, string description, bool optional = false, string defaultValue = null)
            : base(name, description, optional, defaultValue)
        {
        }

        public override bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            var regionEvent = DelayManager.GetType(str);
            if (regionEvent == DelayType.None)
            {
                args.Player.SendErrorMessage("Failed found '{0}' flag!".SFormat(str));
                return false;
            }
            _value = str;
            return true;
        }
    }
}
