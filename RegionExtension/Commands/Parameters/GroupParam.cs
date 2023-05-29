using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;
using TShockAPI;

namespace RegionExtension.Commands.Parameters
{
    public class GroupParam : CommandParam<Group>
    {
        public GroupParam(string name, string description, bool optional = false, Group defaultValue = null)
            : base(name, description, optional, defaultValue)
        {
        }

        public override bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            var group = TShock.Groups.FirstOrDefault(g => g.Name == str);
            if (group == null)
            {
                args?.Player.SendErrorMessage("Invalid group name '{0}'!".SFormat(str));
                return false;
            }
            _value = group;
            return true;
        }
    }
}
