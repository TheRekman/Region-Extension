using TShockAPI;

namespace RegionExtension.Commands.Parameters
{
    public class BoolParam : CommandParam<bool>
    {
        public BoolParam(string name, string description, bool optional = false, bool defaultValue = false)
            : base(name, description, optional, defaultValue)
        {
        }

        public override bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            var value = false;
            if(!bool.TryParse(str, out value))
            {
                args?.Player.SendErrorMessage("Invalid '{0}' = '{1}' value! true/false only.".SFormat(Name, str));
                return false;
            }
            _value = value;
            return true;
        }
    }
}