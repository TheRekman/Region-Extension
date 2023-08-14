using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Commands.Parameters
{
    internal class TimeParam : CommandParam<StringTime>
    {
        public TimeParam(string name, string description, bool optional = false, StringTime defaultValue = null)
            : base(name, description, optional, defaultValue)
        {
        }

        public override bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            var time = StringTime.FromString(str);
            if (time.IsZero() && str != "0") 
            {
                args?.Player.SendErrorMessage("Invalid group name '{0}'!".SFormat(str));
                return false;
            }
            _value = time;
            return true;
        }
    }
}
