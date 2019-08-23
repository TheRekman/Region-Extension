using System;
using TShockAPI.Hooks;

namespace RegionExtension
{
    class ContextCommand
    {
        public string Context { get; private set; }
        Action<PlayerCommandEventArgs, int> action;

        public ContextCommand(string context, Action<PlayerCommandEventArgs, int> action)
        {
            Context = context;
            this.action = action;
        }

        public void Initialize(PlayerCommandEventArgs args, int paramID)
        {
            action(args, paramID);
        }
    }
}
