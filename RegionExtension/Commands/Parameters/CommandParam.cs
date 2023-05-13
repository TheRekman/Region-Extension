using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Commands.Parameters
{
    public abstract class CommandParam<T> : ICommandParam
    {
        public delegate bool Parser(string str, out T value);

        private string _name;
        private string _description;
        private bool _optional;
        protected bool _defined;
        protected T _value;
        protected T _defaultValue;

        public string Name => _name;
        public string Description => _description;
        public object Value { get => _value; }
        public bool Optional => _optional;
        public Type Type => throw new NotImplementedException();

        public CommandParam(string name, string description, bool optional = false, T defaultValue = default(T))
        {
            _name = name;
            _description = description;
            _optional = optional;
            _defaultValue = defaultValue;
        }

        public string GetBracketName() =>
            Optional ? $"[{Name}]" : $"<{Name}>";

        public virtual bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            throw new NotImplementedException();
        }

        public virtual bool TrySetDefaultValue(CommandArgs args = null)
        {
            if (!_optional)
                return false;
            _value = _defaultValue;
            _defined = true;
            return true;
        }
    }
}
