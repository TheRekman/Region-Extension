using RegionExtension.RegionTriggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;
using TShockAPI;

namespace RegionExtension.Database
{
    public class RegionPropertyDBUnit
    {
        [SQLSetting(SqlColumnSettings.Primary, SqlColumnSettings.AutoIncrement)]
        public int Id { get; private set; }
        public int RegionId { get; private set; }
        public string PropertyName { get; private set; }
        public string Conditions { get; private set; }
        public string Args { get; private set; }
        public static Func<QueryResult, RegionPropertyDBUnit> Reader { get; } = (r) => ReadFromDb(r);

        public RegionPropertyDBUnit(int regionId, string propertyName, string args) :
            this(-1, regionId, propertyName, args)
        {
        }

        public RegionPropertyDBUnit(int id, int regionId,string propertyName, string args)
        {
            Id = id;
            RegionId = regionId;
            PropertyName = propertyName;
            Args = args;
        }

        private RegionPropertyDBUnit(QueryResult reader)
        {
            var types = new Dictionary<Type, Func<string, QueryResult, object>>()
            {
                { typeof(string), (s, r) => r.Get<string>(s)},
                { typeof(int), (s, r) => r.Get<int>(s) },
                { typeof(DateTime), (s, r) => DateTime.Parse(r.Get<string>(s)) }
            };
            var properties = typeof(RegionPropertyDBUnit).GetProperties().Where(p => types.ContainsKey(p.PropertyType));
            foreach (var property in properties)
                property.SetValue(this, types[property.PropertyType](property.Name, reader));
        }

        public static RegionPropertyDBUnit ReadFromDb(QueryResult reader) =>
            new RegionPropertyDBUnit(reader);
    }
}
