using RegionExtension.RegionTriggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Commands.Parameters
{
    internal class FunctionParam : CommandParam<Function>
    {
        FunctionParamDefault _default = FunctionParamDefault.None;

        public FunctionParam(string name, string description, bool optional = false, FunctionParamDefault paramDefault = FunctionParamDefault.None)
            : base(name, description, optional, null)
        {
            _default = paramDefault;
        }

        public override bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            var value = Function.GetFunction(args.Player, args.Player.CurrentRegion, str);
            if (value == null)
            {
                args?.Player.SendErrorMessage("Failed implement function: {0}".SFormat(str));
                return false;
            }
            _value = value;
            return true;
        }

        public override bool TrySetDefaultValue(CommandArgsExtension args = null)
        {
            Function value = null;
            switch (_default)
            {
                case FunctionParamDefault.InRegionX:
                    value = Function.GetFunction(args.Player, args.Player.CurrentRegion, "ri%w+cx");
                    break;
                case FunctionParamDefault.InRegionY:
                    value = Function.GetFunction(args.Player, args.Player.CurrentRegion, "ri%h+cy");
                    break;
                case FunctionParamDefault.RandomDouble:
                    value = Function.GetFunction(args.Player, args.Player.CurrentRegion, "rd*10");
                    break;
            }
            if (value == null)
                return false;
            _value = value;
            return true;
        }

        public enum FunctionParamDefault
        {
            None = 0,
            InRegionX = 1,
            InRegionY = 2,
            RandomDouble = 3
        }
    }
}
