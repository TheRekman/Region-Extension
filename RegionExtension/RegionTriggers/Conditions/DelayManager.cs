using Google.Protobuf.WellKnownTypes;
using NuGet.Protocol.Plugins;
using RegionExtension.RegionTriggers.Actions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerrariaApi.Server;
using TShockAPI;

namespace RegionExtension.RegionTriggers.Conditions
{
    public static class DelayManager
    {
        private static SortedList<DateTime, DelayInfo> ForcedDelayedTriggers { get; set; } = new SortedList<DateTime, DelayInfo>();
        private static SortedList<DateTime, DelayInfo> InRegionDelayedTriggers { get; set; } = new SortedList<DateTime, DelayInfo>();
        private static SortedList<DateTime, DelayInfo> AlwaysInRegionDelayedTriggers { get; set; } = new SortedList<DateTime, DelayInfo>();

        private static DateTime _lastUpdate;

        public static Dictionary<string, DelayType> DelayTypes { get; private set; } = new Dictionary<string, DelayType>
        {
            {"-f", DelayType.Forced },
            {"-i", DelayType.InRegionOnActive },
            {"-a", DelayType.InRegion }
        };

        public static void Initialize(Plugin plugin)
        {
            ServerApi.Hooks.GameUpdate.Register(plugin, OnUpdate);
            TriggerManager.OnLeave += OnLeave;
        }

        private static void OnLeave(TriggerActionArgs args)
        {
            foreach (var item in AlwaysInRegionDelayedTriggers.Where(i => i.Value.Region == args.Region).ToArray())
                if (item.Value.RecheckPlayer)
                    AlwaysInRegionDelayedTriggers.Remove(item.Key);
                else if (!TShock.Players.Any(p => p != null && p.Active && p.CurrentRegion == item.Value.Region))
                    AlwaysInRegionDelayedTriggers.Remove(item.Key);

        }

        private static void OnUpdate(EventArgs args)
        {
            if (DateTime.Now < _lastUpdate.AddMilliseconds(1000))
                return;
            Task.Run(() =>
            {
                while (ForcedDelayedTriggers.Count > 0 && ForcedDelayedTriggers.First().Key < DateTime.Now)
                {
                     var value = ForcedDelayedTriggers.First().Value;
                    ForcedDelayedTriggers.RemoveAt(0);
                    value.Trigger.Action.Execute(new TriggerActionArgs(value.Player, value.Region));
                }
            });
            Task.Run(() =>
            {
                while (InRegionDelayedTriggers.Count > 0 && InRegionDelayedTriggers.First().Key < DateTime.Now)
                {
                    var value = InRegionDelayedTriggers.First().Value;
                    InRegionDelayedTriggers.RemoveAt(0);
                    if ((value.RecheckPlayer && value.Player.CurrentRegion == value.Region) ||
                        (!value.RecheckPlayer && TShock.Players.Any(p => p != null && p.Active && p.CurrentRegion == value.Region)))
                        value.Trigger.Action.Execute(new TriggerActionArgs(value.Player, value.Region));
                }
            });
            Task.Run(() =>
            {
                while (AlwaysInRegionDelayedTriggers.Count > 0 && AlwaysInRegionDelayedTriggers.First().Key < DateTime.Now)
                {
                    var value = AlwaysInRegionDelayedTriggers.First().Value;
                    AlwaysInRegionDelayedTriggers.RemoveAt(0);
                    value.Trigger.Action.Execute(new TriggerActionArgs(value.Player, value.Region));
                }
            });
            _lastUpdate = DateTime.Now;
        }

        public static void Reload(Plugin plugin)
        {
            ForcedDelayedTriggers.Clear();
            InRegionDelayedTriggers.Clear();
            AlwaysInRegionDelayedTriggers.Clear();
        }

        public static void RegisterDelay(DelayInfo delay, string delayFlag, DateTime activation)
        {
            RegisterDelay(delay, DelayTypes[delayFlag], activation);
        }

        public static DelayType GetType(string delayFlag)
        {
            if (!DelayTypes.ContainsKey(delayFlag.ToLower()))
                return DelayType.None;
            return DelayTypes[delayFlag];
        }

        public static void RegisterDelay(DelayInfo delay, DelayType delayType, DateTime activation)
        {
            if (delay.Trigger == null || !delay.Trigger.Conditions.Where(c => c.GetNames()[0] != Delay.Names[0] && c.GetNames()[0] != PlayerDelay.Names[0]).CheckConditions(delay.Player, delay.Region))
                return;
            var targetList = ForcedDelayedTriggers;
            switch(delayType)
            {
                case DelayType.None:
                    break;
                case DelayType.Forced:
                    targetList = ForcedDelayedTriggers;
                    break;
                case DelayType.InRegionOnActive:
                    targetList = InRegionDelayedTriggers;
                    break;
                case DelayType.InRegion:
                    targetList = AlwaysInRegionDelayedTriggers;
                    break;
            }

            if ((delay.RecheckPlayer && targetList.Any(p => p.Value.Player == delay.Player && p.Value.Trigger == delay.Trigger)) ||
               (!delay.RecheckPlayer && targetList.Any(p => p.Value.Region == delay.Region && p.Value.Trigger == delay.Trigger)))
                return;
            targetList.Add(activation, delay);
        }

        public static void Dispose(Plugin plugin)
        {
            ServerApi.Hooks.GameUpdate.Deregister(plugin, OnUpdate);
            TriggerManager.OnLeave -= OnLeave;
        }
    }

    public enum DelayType
    {
        None,
        Forced,
        InRegionOnActive,
        InRegion
    }
}
