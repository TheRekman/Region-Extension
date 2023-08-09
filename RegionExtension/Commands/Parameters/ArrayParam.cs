using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using Terraria;

namespace RegionExtension.Commands.Parameters
{
    internal class ArrayParam<T> : CommandParam<T[]>
    {
        List<ICommandParam> _tempParams = new List<ICommandParam>();
        int _index = 0;

        Dictionary<Type, Func<ICommandParam>> _simpleParams = new Dictionary<Type, Func<ICommandParam>>
        {
            { typeof(string), () => new StringParam("", "") },
            { typeof(int), () => new IntParam("", "") },
            { typeof(Item), () => new ItemParam("", "") },
            { typeof(Projectile), () => new ProjectileParam("", "") }
        };

        public ArrayParam(string name, string description, int len = 0, bool optional = false, T[] defaultValue = default) :
            base(name, description, optional, defaultValue)
        {
            Dynamic = true;
            if (len == 0)
                Parametrical = true;
            while (_tempParams.Count < len)
                _tempParams.Add(_simpleParams[typeof(T)]());
        }

        public override bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            if (_tempParams.Count <= _index)
                _tempParams.Add(_simpleParams[typeof(T)]());
            var param = _tempParams[_index++];
            if (!param.TrySetValue(str, args))
                return false;
            _value = _tempParams.Select(p => (T)p.Value).ToArray();
            return true;
        }

        public override ICommandParam[] GetAdditionalParams()
        {
            return _tempParams.Skip(1).ToArray();
        }

        public override bool TrySetDynamicValue(CommandArgsExtension args = null)
        {
            _value = _tempParams.Select(p => (T)p.Value).ToArray();
            return true;
        }
    }
}
