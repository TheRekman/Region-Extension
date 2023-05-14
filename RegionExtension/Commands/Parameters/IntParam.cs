using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Commands.Parameters
{
    internal class IntParam : CommandParam<int>
    {
        public IntParam(string name, string description, bool optional = false, int defaultValue = default(int)) :
            base(name, description, optional, defaultValue)
        {
        }

        public override bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            var value = 0;
            if(!int.TryParse(str, out value))
            {
                args?.Player.SendErrorMessage("Invalid number '{0}' for '{1}'!".SFormat(str, Name));
                return false;
            }
            _value = value;
            return true;
        }
    }
}
