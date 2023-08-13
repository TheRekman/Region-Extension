using RegionExtension.Commands.Parameters;
using RegionExtension.Database;
using RegionExtension.Database.EventsArgs;
using RegionExtension.RegionTriggers.Conditions;
using RegionExtension.RegionTriggers.RegionProperties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace RegionExtension.RegionTriggers
{
    public class PropertyManager
    {
        IRegionProperty[] _regionProperties = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => typeof(IRegionProperty).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                                                                                                                  .Select(t => (IRegionProperty)t.GetConstructors().Where(c => c.GetParameters().Length == 0)
                                                                                                                                                                   .First().Invoke(null))).ToArray();
        DatabaseTable<RegionPropertyDBUnit> _database;

        public IRegionProperty[] RegionProperties { get { return _regionProperties; } }

        public PropertyManager(IDbConnection dbConnection, TerrariaPlugin plugin)
        {
            _database = new DatabaseTable<RegionPropertyDBUnit>("RegionProperties", dbConnection);
            Initialize(plugin);
        }

        public void Initialize(TerrariaPlugin plugin)
        {
            _database.InitializeTable();
            var triggers = new List<Trigger>();
            foreach (var prop in _regionProperties)
                prop.InitializeEventHandler(plugin);
            foreach (var region in TShock.Regions.Regions)
            {
                var list = _database.GetValues(RegionPropertyDBUnit.Reader, new[] { (nameof(RegionPropertyDBUnit.RegionId), (object)region.ID) }).Select(p => (p.PropertyName, p.Conditions, p.Args));
                foreach(var propInfo in list)
                {
                    if (string.IsNullOrEmpty(propInfo.Args))
                    {
                        _database.RemoveByColumn(new[] { (nameof(RegionPropertyDBUnit.RegionId), (object)region.ID), (nameof(RegionPropertyDBUnit.PropertyName), (object)propInfo.PropertyName) });
                        continue;
                    }
                    _regionProperties.FirstOrDefault(p => p.Names[0].Equals(propInfo.PropertyName)).SetFromString(region, new(propInfo.Conditions, propInfo.Args));
                }
            }
            Plugin.RegionExtensionManager.OnRegionDeleted += OnRegionDeleted;
        }

        private void OnRegionDeleted(BaseRegionArgs args)
        {
            var region = args.Region;
            RemoveAllProperties(region);
        }

        public IRegionProperty GetProperty(string name) =>
            _regionProperties.FirstOrDefault(p => p.Names.Contains(name.ToLower()));

        public ICommandParam[] GetPropertyParams(string propertyName) =>
            _regionProperties.FirstOrDefault(p => p.Names.Contains(propertyName.ToLower()))?.CommandParams;

        public bool AddRegionProps(Region region, string propertyName, ICommandParam[] commandParams)
        {
            var prop = _regionProperties.First(p => p.Names.Contains(propertyName.ToLower()));
            if (!prop.DefinedRegions.Contains(region))
                _database.SaveValue(new RegionPropertyDBUnit(region.ID, prop.Names[0], ""));
            prop.AddRegionProperties(region, commandParams);
            var pair = prop.GetStringArgs(region);
            return _database.UpdateByColumn(nameof(RegionPropertyDBUnit.Args), pair.Args, new[] { (nameof(RegionPropertyDBUnit.RegionId), (object)region.ID), (nameof(RegionPropertyDBUnit.PropertyName), prop.Names[0]) }) &&
                   _database.UpdateByColumn(nameof(RegionPropertyDBUnit.Conditions), pair.Conditions, new[] { (nameof(RegionPropertyDBUnit.RegionId), (object)region.ID), (nameof(RegionPropertyDBUnit.PropertyName), prop.Names[0]) });

        }

        public bool RemoveRegionProperties(Region region, string propertyName, ICommandParam[] commandParams)
        {
            var prop = _regionProperties.First(p => p.Names.Contains(propertyName.ToLower()));
            if (!prop.DefinedRegions.Contains(region))
                return false;
            prop.RemoveRegionProperties(region, commandParams);
            if (!prop.DefinedRegions.Contains(region))
                return _database.RemoveByColumn(new[] { (nameof(RegionPropertyDBUnit.RegionId), (object)region.ID), (nameof(RegionPropertyDBUnit.PropertyName), prop.Names[0]) });
            var pair = prop.GetStringArgs(region);
            return _database.UpdateByColumn(nameof(RegionPropertyDBUnit.Args), pair.Args, new[] { (nameof(RegionPropertyDBUnit.RegionId), (object)region.ID), (nameof(RegionPropertyDBUnit.PropertyName), prop.Names[0]) }) &&
                   _database.UpdateByColumn(nameof(RegionPropertyDBUnit.Conditions), pair.Conditions, new[] { (nameof(RegionPropertyDBUnit.RegionId), (object)region.ID), (nameof(RegionPropertyDBUnit.PropertyName), prop.Names[0]) });

        }

        public bool AddRegionCondition(Region region, string propertyName, ICommandParam[] commandParams, IRegionCondition regionCondition)
        {
            var prop = _regionProperties.First(p => p.Names.Contains(propertyName.ToLower()));
            if (!prop.DefinedRegions.Contains(region))
                _database.SaveValue(new RegionPropertyDBUnit(region.ID, prop.Names[0], ""));
            prop.AddCondition(region, commandParams, regionCondition);
            var pair = prop.GetStringArgs(region);
            return _database.UpdateByColumn(nameof(RegionPropertyDBUnit.Args), pair.Args, new[] { (nameof(RegionPropertyDBUnit.RegionId), (object)region.ID), (nameof(RegionPropertyDBUnit.PropertyName), prop.Names[0]) }) &&
                   _database.UpdateByColumn(nameof(RegionPropertyDBUnit.Conditions), pair.Conditions, new[] { (nameof(RegionPropertyDBUnit.RegionId), (object)region.ID), (nameof(RegionPropertyDBUnit.PropertyName), prop.Names[0]) });
        }

        public bool RemoveRegionCondition(Region region, string propertyName, ICommandParam[] commandParams, IRegionCondition regionCondition)
        {
            var prop = _regionProperties.First(p => p.Names.Contains(propertyName.ToLower()));
            if (!prop.DefinedRegions.Contains(region))
                _database.SaveValue(new RegionPropertyDBUnit(region.ID, prop.Names[0], ""));
            prop.RemoveCondition(region, commandParams, regionCondition);
            var pair = prop.GetStringArgs(region);
            return _database.UpdateByColumn(nameof(RegionPropertyDBUnit.Args), pair.Args, new[] { (nameof(RegionPropertyDBUnit.RegionId), (object)region.ID), (nameof(RegionPropertyDBUnit.PropertyName), prop.Names[0]) }) &&
                   _database.UpdateByColumn(nameof(RegionPropertyDBUnit.Conditions), pair.Conditions, new[] { (nameof(RegionPropertyDBUnit.RegionId), (object)region.ID), (nameof(RegionPropertyDBUnit.PropertyName), prop.Names[0]) });
        }

        public void ClearProperty(Region region, string propertyName)
        {
            var prop = _regionProperties.First(p => p.Names.Contains(propertyName.ToLower()));
            if (!prop.DefinedRegions.Contains(region))
                return;
            prop.ClearProperties(region);
            _database.RemoveByColumn(new[] { (nameof(RegionPropertyDBUnit.RegionId), (object)region.ID), (nameof(RegionPropertyDBUnit.PropertyName), prop.Names[0]) });
        }

        public bool RemoveAllProperties(Region region)
        {
            foreach (var item in _regionProperties.Where(p => p.DefinedRegions.Contains(region)))
                item.ClearProperties(region);
            return _database.RemoveByColumn(new[] { (nameof(RegionPropertyDBUnit.RegionId), (object)region.ID) });
        }
    }
}
