using RegionExtension.RegionTriggers.Actions;
using RegionExtension.RegionTriggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Commands.Parameters
{
    internal class TriggerFormerParam : CommandParam<ActionFormer>
    {
        public TriggerFormerParam(string name, string description, bool optional = false, ActionFormer defaultValue = null)
            : base(name, description, optional, defaultValue)
        {
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
            _value = former;
            return true;
        }

    }
}
