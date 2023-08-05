using OTAPI;
using RegionExtension.Database;
using RegionExtension.RegionTriggers.Actions;
using RegionExtension.RegionTriggers.Conditions;
using RegionExtension.RegionTriggers.RegionProperties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;
    
namespace RegionExtension.RegionTriggers
{
    public class TriggerManager
    {
        private DateTime _lastUpdate = DateTime.UtcNow;
        private DatabaseTable<TriggerDBUnit> _database;
        private Region[] _lastRegions = new Region[TShock.Players.Length];
        Dictionary<Region, List<Trigger>> _triggers = new Dictionary<Region, List<Trigger>>();

        public TriggerManager(IDbConnection dbConnection)
        {
            _database = new DatabaseTable<TriggerDBUnit>("RegionTrigger", dbConnection);
            Initialize();
        }

        public static ActionFormer[] Formers { get => new ActionFormer[]
        {
            CommandAction.Former,
            PushAction.Former,
            SendPacketAction.Former,
            SendMessageAction.Former
        };}

        public static ActionFormer GetFormer(string name)
        {
            var res = Formers.FirstOrDefault(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            var iss = Formers.Any(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return res;
        }

        public void Initialize()
        {
            _database.InitializeTable();
            var triggers = new List<Trigger>();
            foreach (var region in TShock.Regions.Regions)
            {
                var list = _database.GetValues(TriggerDBUnit.Reader, new[] { (nameof(TriggerDBUnit.RegionId), (object)region.ID) }).Select(t => t.ParseToTrigger()).ToList();
                if(list.Count != 0)
                {
                    _triggers.Add(region, list);
                    for (int i = 0; i < _triggers[region].Count; i++)
                    {
                        if(_triggers[region][i].LocalId != i)
                        {
                            _triggers[region][i].LocalId = i;
                            _database.UpdateByColumn(nameof(TriggerDBUnit.LocalId), i, new[] { (nameof(TriggerDBUnit.Id), (object)_triggers[region][i].Id) });
                        }
                    }
                }    
            }
        }

        public void OnPlayerEnter(GreetPlayerEventArgs args)
        {
            _lastRegions[args.Who] = null;
        }

        public IEnumerable<Trigger> GetTriggers(Region region) =>
            _triggers.ContainsKey(region) ? _triggers[region] : Enumerable.Empty<Trigger>();

        public bool CreateTrigger(Region region, RegionEvents regionEvent, ITriggerAction triggerAction)
        {
            if (!_triggers.ContainsKey(region))
                _triggers.Add(region, new List<Trigger>());
            if (!_database.SaveValue(new TriggerDBUnit(region.ID, _triggers[region].Count, triggerAction.Name, regionEvent.ToString(), triggerAction.GetArgsString())))
                return false;
            var triggerUnit = _database.GetValues(TriggerDBUnit.Reader, new[] { (nameof(TriggerDBUnit.RegionId), (object)region.ID), (nameof(TriggerDBUnit.LocalId), _triggers[region].Count) }).First();
            var trigger = new Trigger(triggerUnit.Id, _triggers[region].Count, region, regionEvent, triggerAction);
            _triggers[region].Add(trigger);
            return true;
        }

        public bool RemoveTrigger(Region region, int id)
        {
            if (!_triggers.ContainsKey(region) || _triggers[region].Count <= id)
                return false;
            return RemoveTrigger(region, _triggers[region][id]);
        }

        public bool RemoveTrigger(Region region, Trigger trigger)
        {
            if (!_database.RemoveByObject(new TriggerDBUnit(trigger)))
                return false;
            _triggers[region].Remove(trigger);
            for(int i = 0; i < _triggers[region].Count; i++)
            {
                _triggers[region][i].LocalId = i;
                _database.UpdateByColumn(nameof(TriggerDBUnit.LocalId), i, new[] { (nameof(TriggerDBUnit.Id), (object)trigger.Id) });
            }
            return true;
        }

        public void OnUpdate()
        {
            var lasUpd = _lastUpdate;
            Task.Run(() =>
            {
                if (DateTime.UtcNow > lasUpd.AddMilliseconds(500))
                    return;
                for(int i = 0; i < TShock.Players.Length; i++)
                    if (TShock.Players[i] != null && TShock.Players[i].Active)
                    {
                        var lastRegion = _lastRegions[i];
                        var player = TShock.Players[i];
                        if (lastRegion != player.CurrentRegion)
                        {
                            TriggerEvent(RegionEvents.OnEnter, player, player.CurrentRegion);
                            TriggerEvent(RegionEvents.OnLeave, player, lastRegion);
                            _lastRegions[i] = player.CurrentRegion;
                        }
                        TriggerEvent(RegionEvents.OnIn, player, player.CurrentRegion);
                    }
            });
            _lastUpdate = DateTime.Now;
        }

        private void TriggerEvent(RegionEvents events, TSPlayer player, Region region)
        {
            if (region == null || !_triggers.ContainsKey(region))
                return;
            foreach (var trigger in _triggers[region].Where(t => t.Event == events && t.Conditions.CheckConditions(player, region)))
                trigger.Action.Execute(new TriggerActionArgs(player, region));
        }

        public bool AddCondition(Region region, IRegionCondition condition, IEnumerable<int> localIds)
        {
            var triggers = _triggers[region];
            localIds ??= triggers.Select(t => t.LocalId);
            var res = true;
            foreach (var i in localIds.Intersect(triggers.Select(t => t.LocalId)))
            {
                triggers[i].Conditions = triggers[i].Conditions.Where(c => !c.GetNames()[0].Equals(condition.GetNames()[0]))
                                                               .Append(condition);
                res = true && _database.UpdateByColumn(nameof(TriggerDBUnit.Conditions), ConditionManager.GenerateConditionsString(triggers[i].Conditions), new[] { (nameof(TriggerDBUnit.Id), (object)triggers[i].Id) });
            }
            return res;
        }

        public bool RemoveCondition(Region region, IRegionCondition condition, IEnumerable<int> localIds)
        {
            var triggers = _triggers[region];
            localIds ??= triggers.Select(t => t.LocalId);
            var res = true;
            foreach (var i in localIds.Intersect(triggers.Select(t => t.Id)))
            {
                triggers[i].Conditions = triggers[i].Conditions.Where(c => !c.GetNames()[0].Equals(condition.GetNames()[0]));
                res = true && _database.UpdateByColumn(nameof(TriggerDBUnit.Conditions), ConditionManager.GenerateConditionsString(triggers[i].Conditions), new[] { (nameof(TriggerDBUnit.Id), (object)triggers[i].Id) });
            }
            return res;
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
