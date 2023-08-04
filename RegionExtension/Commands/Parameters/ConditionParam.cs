using RegionExtension.RegionTriggers.Actions;
using RegionExtension.RegionTriggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using RegionExtension.RegionTriggers.Conditions;

namespace RegionExtension.Commands.Parameters
{
    internal class ConditionParam : CommandParam<Condition>
    {
        private ICommandParam[] _additionalParams;
        public ConditionParam(string name, string description, bool optional = false, Condition defaultValue = null)
            : base(name, description, optional, defaultValue)
        {
            Dynamic = true;
        }

        public override bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            var param = ConditionManager.GetParams(str);
            if (param == null)
            {
                args.Player.SendErrorMessage("Failed found '{0}' condition!".SFormat(str));
                return false;
            }
            _additionalParams = param;
            return true;
        }

        public override ICommandParam[] GetAdditionalParams() =>
            _additionalParams;

        public override bool TrySetDynamicValue(CommandArgsExtension args)
        {
            var res = ConditionManager.GetCondition(Name, _additionalParams);
            if (res == null)
                return false;
            _value = res;
            return true;
        }
    }
}
