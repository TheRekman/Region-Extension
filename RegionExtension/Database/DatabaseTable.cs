using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TShockAPI;
using TShockAPI.DB;
using System.Reflection;
using MySqlX.XDevAPI.Relational;
using RegionExtension.Database.Actions;
using System.Drawing;

namespace RegionExtension.Database
{
    public class DatabaseTable<T>
    {
        public string Name { get; private set; }
        public IDbConnection Connection { get; private set; }

        private static Dictionary<Type, MySqlDbType> _types = new Dictionary<Type, MySqlDbType>()
            {
                { typeof(string), MySqlDbType.Text },
                { typeof(int), MySqlDbType.Int32 },
                { typeof(DateTime), MySqlDbType.Text }
            };

        public DatabaseTable(string name, IDbConnection connection)
        {
            Name = name;
            Connection = connection;
        }

        public IEnumerable<T> GetValues(Func<QueryResult, T> unitReader, params (string columnName, object value)[] conditions)
        {
            var res = new List<T>();
            var query = $"SELECT * FROM {Name}";
            if (conditions != null && conditions.Length != 0)
                query += " WHERE " + string.Join(" AND ", conditions.Select(c => c.columnName + "='" + c.value.ToString() + "'"));
            try
            {
                using (var reader = Connection.QueryReader(query))
                {
                    while (reader.Read())
                        res.Add(unitReader(reader));
                }
            }
            catch (Exception ex) 
            {
                TShock.Log.Error(ex.Message);
            }
            return res;
        }

        public bool InitializeTable()
        {
            var properties = typeof(T).GetProperties();
            var columns = new List<SqlColumn>();
            foreach (var property in properties)
            {
                MySqlDbType type;
                if (!_types.TryGetValue(property.PropertyType, out type))
                    continue;
                var column = new SqlColumn(property.Name, _types[property.PropertyType]);
                SetColumnProperties(property, column);
                columns.Add(column);
            }
            try
            {
                IQueryBuilder queryCreator = Connection.GetSqlType() == SqlType.Sqlite ?
                                            new SqliteQueryCreator() : new MysqlQueryCreator();
                var creator = new SqlTableCreator(Connection, queryCreator);
                creator.EnsureTableStructure(new SqlTable(Name, columns.ToArray()));
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
                return false;
            }
            return true;
        }

        private static void SetColumnProperties(PropertyInfo property, SqlColumn column)
        {
            var attribute = property.GetCustomAttributes(true).FirstOrDefault(a => a.GetType() == typeof(SQLSettingAttribute));
            if (attribute != null)
            {
                var settings = (SQLSettingAttribute)attribute;
                var columnProperties = column.GetType().GetProperties().Where(p => p.PropertyType == typeof(bool) &&
                                                                              settings.Settings.Any(s => s.ToString().Equals(p.Name, StringComparison.OrdinalIgnoreCase)));
                foreach (var prop in columnProperties)
                    prop.SetValue(column, true);
            }
        }

        public bool SaveValue(T dBUnit)
        {
            var properties = dBUnit.GetType().GetProperties().Where(p => _types.ContainsKey(p.PropertyType) &&
                                                                   !p.GetCustomAttributes(true).Any(a => a.GetType() == typeof(SQLSettingAttribute) &&
                                                                                                    ((SQLSettingAttribute)a).Settings.Any(s => s == SqlColumnSettings.Primary ||
                                                                                                                                          s == SqlColumnSettings.AutoIncrement)));
            var names = string.Join(", ", properties.Select(p => p.Name).ToArray());
            var values = "'" + string.Join("', '", properties.Select(p => p.GetValue(dBUnit))) + "'";
            try
            {
                Connection.Query($"INSERT INTO {Name} ({names}) VALUES ({values});");
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
                return false;
            }
            return true;
        }

        public bool RemoveByObject(T dBunit)
        {
            var keyProperty = dBunit.GetType().GetProperties().FirstOrDefault(p => p.GetCustomAttributes(true).Any(a => a.GetType() == typeof(SQLSettingAttribute) &&
                                                                                                         ((SQLSettingAttribute)a).Settings.Any(s => s == SqlColumnSettings.Primary)));
            if (keyProperty == null)
                throw new ArgumentException("Failed find primary key column!");
            try
            {
                Connection.Query($"DELETE FROM {Name} WHERE {keyProperty.Name}={keyProperty.GetValue(dBunit)}");
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
                return false;
            }
            return true;
        }

        public bool RemoveByColumn(params (string columnName, object value)[] conditions)
        {
            var query = $"DELETE FROM {Name}";
            if (conditions != null && conditions.Length != 0)
                query += " WHERE " + string.Join(" AND ", conditions.Select(c => c.columnName + "='" + c.value + "'"));
            try
            {
                Connection.Query(query);
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
                return false;
            }
            return true;
        }

        public bool UpdateByColumn(string columnName, object value, params (string columnName, object value)[] conditions)
        {
            var query = $"UPDATE {Name} SET {columnName}='{value}'";
            if (conditions != null && conditions.Length != 0)
                query += " WHERE " + string.Join(" AND ", conditions.Select(c => c.columnName + "='" + c.value + "'"));
            try
            {
                Connection.Query(query);
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
                return false;
            }
            return true;
        }
    }
}
