using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;

namespace RegionExtension.Commands.Parameters
{
    public class ItemParam : CommandParam<Item>
    {
        public ItemParam(string name, string description, bool optional = false, Item defaultValue = null) :
    base(name, description, optional, defaultValue)
        {
        }

        public override bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            var item = TShock.Utils.GetItemFromTag(str);
            if(item == null)
            {
                item = TShock.Utils.GetItemByIdOrName(str).FirstOrDefault();
                if(item == null)
                    args.Player.SendErrorMessage("Failed found item {0}!".SFormat(str));
            }
            _value = item;
            return true;
        }
    }
}
