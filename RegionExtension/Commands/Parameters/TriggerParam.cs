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
    public class TriggerParam : CommandParam<ITriggerAction>
    {
        private ActionFormer _former;
        public TriggerParam(string name, string description, bool optional = false, ITriggerAction defaultValue = null)
            : base(name, description, optional, defaultValue)
        {
            Dynamic = true;
        }

        public override bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            var former = TriggerManager.GetFormer(str);
            if (former == null)
            {
                args.Player.SendErrorMessage("Failed found '{0}' trigger!".SFormat(str));
                return false;
            }
            if (!args.Player.HasPermission(former.Permission))
            {
                args.Player.SendErrorMessage("You don't have permission to use '{0}' trigger!".SFormat(str));
                return false;
            }
            _former = former;
            return true;
        }

        public override ICommandParam[] GetAdditionalParams() =>
            _former.Params;

        public override bool TrySetDynamicValue(CommandArgsExtension args)
        {
            var res = _former.Action(_former.Params, args);
            if (res == null)
                return false;
            _value = res;
            return true;
        }
    }
}
