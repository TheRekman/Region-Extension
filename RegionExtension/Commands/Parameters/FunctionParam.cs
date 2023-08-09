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
        string _defaultString = null;

        public FunctionParam(string name, string description, bool optional = false, FunctionParamDefault paramDefault = FunctionParamDefault.None)
            : base(name, description, optional, null)
        {
            _default = paramDefault;
        }

        public FunctionParam(string name, string description, bool optional = false, string paramDefault = null)
            : base(name, description, optional, null)
        {
            _defaultString = paramDefault;
        }

        public override bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            var value = Function.GetFunction(args.Player, RegionParam.LastUsedRegion[args.Player.Index], str);
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
                    value = Function.GetFunction(args.Player, RegionParam.LastUsedRegion[args.Player.Index], "ri%w+cx");
                    break;
                case FunctionParamDefault.InRegionY:
                    value = Function.GetFunction(args.Player, RegionParam.LastUsedRegion[args.Player.Index], "ri%h+cy");
                    break;
                case FunctionParamDefault.RandomDouble:
                    value = Function.GetFunction(args.Player, RegionParam.LastUsedRegion[args.Player.Index], "rd*10");
                    break;
                case FunctionParamDefault.PlayerX:
                    value = Function.GetFunction(args.Player, RegionParam.LastUsedRegion[args.Player.Index], "px");
                    break;
                case FunctionParamDefault.PlayerY:
                    value = Function.GetFunction(args.Player, RegionParam.LastUsedRegion[args.Player.Index], "py");
                    break;
                default:
                    if (!string.IsNullOrEmpty(_defaultString))
                        value = Function.GetFunction(args.Player, RegionParam.LastUsedRegion[args.Player.Index], _defaultString);
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
            RandomDouble = 3,
            PlayerX = 4, PlayerY = 5,
        }
    }
}
