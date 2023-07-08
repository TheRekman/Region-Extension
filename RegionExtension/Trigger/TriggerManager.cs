using OTAPI;
using RegionExtension.Trigger.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace RegionExtension.Trigger
{
    public class TriggerManager
    {
        private DateTime _lastUpdate = DateTime.UtcNow;
        private Dictionary<TSPlayer, Region> _lastRegions = new Dictionary<TSPlayer, Region>();

        Dictionary<Region, List<Trigger>> _triggers = new Dictionary<Region, List<Trigger>>();

        public void OnPlayerEnter(GreetPlayerEventArgs args)
        {
            if (!_lastRegions.ContainsKey(TShock.Players[args.Who]))
                _lastRegions.Add(TShock.Players[args.Who], null);
        }

        public void OnUpdate()
        {
            if (DateTime.UtcNow < _lastUpdate.AddMilliseconds(500))
                return;
            foreach(var player in TShock.Players.Where(p => p.Active))
            {
                var region = _lastRegions[player];
                if(region != player.CurrentRegion)
                {
                    if (_triggers.ContainsKey(region))
                        foreach (var trigger in _triggers[region].Where(t => t.Event == RegionEvents.OnEnter))
                            trigger.Action.Execute(new TriggerActionArgs(player, region));
                    if (_triggers.ContainsKey(player.CurrentRegion))
                        foreach (var trigger in _triggers[region].Where(t => t.Event == RegionEvents.OnLeave))
                            trigger.Action.Execute(new TriggerActionArgs(player, region));
                    _lastRegions[player] = region;
                }
                if(_triggers.ContainsKey(region))
                    foreach (var trigger in _triggers[region].Where(t => t.Event == RegionEvents.OnIn))
                        trigger.Action.Execute(new TriggerActionArgs(player, region));
            }
        }
    }

    public enum RegionEvents
    {
        None,
        OnEnter,
        OnLeave,
        OnIn
    }
}
