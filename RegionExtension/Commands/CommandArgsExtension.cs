using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Commands
{
    public class CommandArgsExtension : CommandArgs
    {
        public Plugin Plugin { get; set; }
        public CommandArgsExtension(CommandArgs args, Plugin plugin) :
            base(args.Message, args.Silent, args.Player, args.Parameters)
        {
            Plugin = plugin;
        }
    }
}
