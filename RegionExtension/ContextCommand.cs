using System;
using TShockAPI.Hooks;

namespace RegionExtension
{
    public class ContextCommand
    {
        public string[] Names{ get; private set; }
        public string Description { get; set; }
        public Action<PlayerCommandEventArgs, int> Action { get; private set; }

        public ContextCommand(Action<PlayerCommandEventArgs, int> action, params string[] names)
        {
            Names = names;
            this.Action = action;
        }
    }
}
