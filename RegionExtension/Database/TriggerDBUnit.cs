using MySqlX.XDevAPI.Relational;
using Org.BouncyCastle.Asn1.X509.Qualified;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using TShockAPI.DB;
using RegionExtension.RegionTriggers;
using System.Diagnostics.CodeAnalysis;
using NuGet.Protocol;
using System.Data.Common;
using System.Linq;

namespace RegionExtension.Database
{
    public class TriggerDBUnit
    {
        [SQLSetting(SqlColumnSettings.Primary, SqlColumnSettings.AutoIncrement)]
        public int Id { get; private set; }
        public int LocalId { get; private set; }
        public int RegionId { get; private set; }
        public string ActionName { get; private set; }
        public string RegionEvent { get; private set; }
        public string Args { get; private set; }
        public static Func<QueryResult, TriggerDBUnit> Reader { get; } = (r) => ReadFromDb(r);

        public TriggerDBUnit(int regionId, int localId, string actionName, string regionEvent, string args) :
            this(-1, regionId, localId, actionName, regionEvent, args)
        {
        }

        public TriggerDBUnit(int id, int regionId, int localId, string actionName,string regionEvent, string args)
        {
            Id = id;
            RegionId = regionId;
            LocalId = localId;
            ActionName = actionName;
            RegionEvent = regionEvent;
            Args = args;
        }

        private TriggerDBUnit(QueryResult reader)
        {
            var types = new Dictionary<Type, Func<string, QueryResult, object>>()
            {
                { typeof(string), (s, r) => r.Get<string>(s)},
                { typeof(int), (s, r) => r.Get<int>(s) },
                { typeof(DateTime), (s, r) => DateTime.Parse(r.Get<string>(s)) }
            };
            var properties = typeof(TriggerDBUnit).GetProperties().Where(p => types.ContainsKey(p.PropertyType));
            foreach (var property in properties)
                property.SetValue(this, types[property.PropertyType](property.Name, reader));
        }

        public TriggerDBUnit(Trigger trigger)
        {
            Id = trigger.Id;
            RegionId = trigger.Region.ID;
            ActionName = trigger.Action.Name;
            RegionEvent = trigger.Event.ToString();
            Args = trigger.Action.GetArgsString();
        }

        public static TriggerDBUnit ReadFromDb(QueryResult reader) =>
            new TriggerDBUnit(reader);

        public Trigger ParseToTrigger()
        {
            var reg = TShock.Regions.GetRegionByID(RegionId);
            if (reg == null)
                return null;
            var former = TriggerManager.GetFormer(ActionName);
            var action = former.FromString(Args);
            var trigger = new Trigger(Id, LocalId, reg, Enum.Parse<RegionEvents>(RegionEvent, true), action);
            return trigger;
        }
    }
}
