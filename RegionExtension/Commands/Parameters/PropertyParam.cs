using RegionExtension.RegionTriggers.Actions;
using RegionExtension.RegionTriggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using RegionExtension.RegionTriggers.RegionProperties;

namespace RegionExtension.Commands.Parameters
{
    public class PropertyParam : CommandParam<PropertyFormer>
    {
        PropertyFormer _property;
        public PropertyParam(string name, string description, bool optional = false, PropertyFormer defaultValue = null)
            : base(name, description, optional, defaultValue)
        {
            Dynamic = true;
        }

        public override bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            var prop = Plugin.RegionExtensionManager.PropertyManager.GetProperty(str);
            if (prop == null)
            {
                args.Player.SendErrorMessage("Failed found '{0}' property!".SFormat(str));
                return false;
            }
            _property = new PropertyFormer(prop.Names[0], prop.CommandParams);
            return true;
        }

        public override ICommandParam[] GetAdditionalParams() =>
            _property.Params;

        public override bool TrySetDynamicValue(CommandArgsExtension args)
        {
            _value = _property;
            return true;
        }
    }
}
