using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Extensions.EnumerationExtensions;
using TShockAPI;

namespace RegionExtension.Commands.Parameters
{
    public class StringParam : CommandParam<string>
    {
        public StringParam(string name, string description, bool optional = false, string defaultValue = default) :
            base(name, description, optional, defaultValue)
        {
        }

        public override bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            _value = str;
            return true;
        }
    }
}
