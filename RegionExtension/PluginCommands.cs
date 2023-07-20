using RegionExtension.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace RegionExtension
{
    public static class PluginCommands
    {
        public static Plugin Plugin { get { return _plugin; } }
        private static Plugin _plugin;

        public static void Initialize(Plugin plugin)
        {
            _plugin = plugin;

            InitializeCommands(plugin,
                new RegionExtensionCommand(),
                new RegionOwnCommand(),
                new RegionHistoryCommand(),
                new RegionTriggerCommand()
                );
        }

        public static void InitializeCommands(this Plugin plugin, params CommandExtension[] commands)
        {
            CommandsInitializer.InitializeCommands(plugin, commands);
        }
    }
}
