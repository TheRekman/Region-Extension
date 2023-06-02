using System;
using TShockAPI;

namespace RegionExtension.Commands.Parameters
{
    public interface ICommandParam
    {
        string Name { get; }
        string Description { get; }
        object Value { get; }
        bool Optional { get; }
        public string GetBracketName();
        public string GetColoredBracketName();
        bool TrySetValue(string str, CommandArgsExtension args = null);
        bool TrySetDefaultValue(CommandArgs args = null);
    }
}