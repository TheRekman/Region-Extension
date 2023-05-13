using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.Commands.Parameters
{
    public class UserAccountParam : CommandParam<UserAccount>
    {
        public UserAccountParam(string name, string description, bool optional = false, UserAccount defaultValue = null)
            : base(name, description, optional, defaultValue)
        {
        }

        public override bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            UserAccount account = null;
            account = TShock.UserAccounts.GetUserAccountByName(str);
            if (account == null)
            {
                if (!TryGetUserAccountFromNameStart(str, args, out account))
                    return false;
            }
            _value = account;
            return true;
        }

        public bool TryGetUserAccountFromNameStart(string str, CommandArgsExtension args, out UserAccount account)
        {
            args?.Player.SendInfoMessage("Failed find user account with \"{0}\" name. Trying find from active players with name start.");
            var players = TShock.Players.Where(p => p.Name.StartsWith(str) && p.IsLoggedIn);
            if(players.Count() == 0)
            {
                args.Player.SendInfoMessage("Failed find player with \"{0}\" name start.");
                args.Player.SendInfoMessage("Make sure if this player was registered and logged.");
                account = null;
                return false;
            }
            if (players.Count() > 1)
            {
                args.Player.SendInfoMessage("More than one player was found:");
                args.Player.SendInfoMessage(string.Join(", ", players));
                account = null;
                return false;
            }
            account = players.Select(p => p.Account).First();
            return true;
        }

        public override bool TrySetDefaultValue(CommandArgs args = null)
        {
            _defaultValue = args.Player.Account;
            return base.TrySetDefaultValue();
        }
    }
}
