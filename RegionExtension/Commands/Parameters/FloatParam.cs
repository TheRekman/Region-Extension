using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Commands.Parameters
{
    internal class FloatParam : CommandParam<float>
    {
        public FloatParam(string name, string description, bool optional = false, float defaultValue = default(float)) :
            base(name, description, optional, defaultValue)
        {
        }

        public override bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            var value = 0f;
            if (!float.TryParse(str, out value))
            {
                args?.Player.SendErrorMessage("Invalid number '{0}' for '{1}'!".SFormat(str, Name));
                return false;
            }
            _value = value;
            return true;
        }
    }
}
