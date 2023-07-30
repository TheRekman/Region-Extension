using System;
using TShockAPI;

namespace RegionExtension.Commands.Parameters
{
    public abstract class CommandParam<T> : ICommandParam
    {
        public delegate bool Parser(string str, out T value);

        private string _name;
        private string _description;
        private bool _optional;
        protected T _value;
        protected T _defaultValue;

        public string Name => _name;
        public string Description => _description;
        public object Value { get => _value; }
        public T TValue { get => _value; }
        public bool Optional => _optional;
        public virtual bool Dynamic { get; protected set; } = false;
        public int Count { get; } = 1;
        public bool Parametrical { get; protected set; } = false;

        public CommandParam() :
            this("", "", true, default)
        {
        }

        public CommandParam(string name, string description, bool optional = false, T defaultValue = default(T))
        {
            _name = name;
            _description = description;
            _optional = optional;
            _defaultValue = defaultValue;
        }

        public virtual string GetColoredBracketName() =>
            Optional ? $"[[c/6ce0b0:{Name}]]" : $"<[c/e88484:{Name}]>";

        public virtual string GetBracketName() =>
            Optional ? $"[{Name}]" : $"<{Name}>";

        public virtual bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            throw new NotImplementedException();
        }

        public virtual bool TrySetDefaultValue(CommandArgsExtension args = null)
        {
            if (!_optional)
                return false;
            _value = _defaultValue;
            return true;
        }

        public virtual bool TrySetDynamicValue(CommandArgsExtension args = null)
        {
            throw new NotImplementedException();
        }

        public virtual ICommandParam[] GetAdditionalParams() =>
            new ICommandParam[0];
    }
}
