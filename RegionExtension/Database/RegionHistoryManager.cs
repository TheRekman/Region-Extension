using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI.DB;
using TShockAPI;
using RegionExtension.Database.Actions;
using TShockAPI.Hooks;
using System.Data.SqlTypes;

namespace RegionExtension.Database
{
    public class RegionHistoryManager
    {
        private IDbConnection _database;
        private Dictionary<int, Stack<ActionInfo>> _redoActions = new Dictionary<int, Stack<ActionInfo>>();

        private SqlTable _table =
            new SqlTable("RegionHistory",
                         new SqlColumn(TableHistoryInfo.Id.ToString(), MySqlDbType.Int32) {Unique = true, Primary = true, AutoIncrement = true },
                         new SqlColumn(TableHistoryInfo.RegionId.ToString(), MySqlDbType.Int32) { NotNull = true },
                         new SqlColumn(TableHistoryInfo.UserId.ToString(), MySqlDbType.Int32) { NotNull = true },
                         new SqlColumn(TableHistoryInfo.ActionName.ToString(), MySqlDbType.Text),
                         new SqlColumn(TableHistoryInfo.Args.ToString(), MySqlDbType.Text),
                         new SqlColumn(TableHistoryInfo.UndoArgs.ToString(), MySqlDbType.Text),
                         new SqlColumn(TableHistoryInfo.DateTime.ToString(), MySqlDbType.DateTime) { DefaultCurrentTimestamp = true }
                         );

        public List<RegionExtensionInfo> RegionsInfo { get; private set; }

        public RegionHistoryManager(IDbConnection db)
        {
            _database = db;
            InitializeTable();
        }

        public void InitializeTable()
        {
            IQueryBuilder queryCreator = _database.GetSqlType() == SqlType.Sqlite ?
                                            new SqliteQueryCreator() : new MysqlQueryCreator();
            var creator = new SqlTableCreator(_database, queryCreator);
            creator.EnsureTableStructure(_table);
        }

        public void SaveAction(IAction action, Region region, UserAccount user, DateTime dateTime)
        {
            
            var name = action.Name;
            var args = action.GetArgsString();
            var undoArgs = action.GetUndoArgsString();
            var regionId = region.ID;
            var userId = user.ID;
            if (_redoActions.ContainsKey(regionId))
                _redoActions.Remove(regionId);
            try
            {
                
                var variablesString = string.Join(", ", _table.Columns.Select(c => c.Name).Where(s => s != TableHistoryInfo.Id.ToString()));
                var values = "'" + string.Join("', '",
                             regionId, userId, name, args, undoArgs, new SqlDateTime(dateTime).ToSqlString().Value) + "'";
                _database.Query($"INSERT INTO {_table.Name} ({variablesString}) VALUES ({values});");
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
            }
        }

        public void SaveAction(IAction action, Region region, UserAccount user)
        {
            SaveAction(action, region, user, DateTime.UtcNow);
        }

        public bool Undo(int count, int regionId)
        {
            try
            {
                var ids = new List<int>();
                using (var reader = _database.QueryReader($"SELECT * FROM {_table.Name} WHERE RegionId=@0 ORDER BY DateTime DESC", regionId))
                {
                    while (reader.Read() && count > 0)
                    {
                        regionId = reader.Get<int>(_table.Columns[1].Name);
                        var userId = reader.Get<int>(_table.Columns[2].Name);
                        var actionName = reader.Get<string>(_table.Columns[3].Name);
                        var args = reader.Get<string>(_table.Columns[4].Name);
                        var undoArgs = reader.Get<string>(_table.Columns[5].Name);
                        var dateTime = reader.Get<DateTime>(_table.Columns[6].Name);
                        var action = ActionFactory.GetActionByName(actionName, args);
                        var undoAction = action.GetUndoAction(undoArgs);
                        if (!_redoActions.ContainsKey(regionId))
                            _redoActions.Add(regionId, new Stack<ActionInfo>(10));
                        _redoActions[regionId].Push(new ActionInfo(action, regionId, userId, dateTime));
                        undoAction.Do();
                        ids.Add(reader.Get<int>(_table.Columns[0].Name));
                        count--;
                    }
                }
                foreach(var id in ids)
                    _database.Query($"DELETE FROM {_table.Name} WHERE Id=@0", id);
            }
            catch (Exception e)
            {
                TShock.Log.Error(e.Message);
                return false;
            }
            return true;
        }

        public List<string> GetActionsInfo(int count, int regionId)
        {
            try
            {
                using (var reader = _database.QueryReader($"SELECT * FROM {_table.Name} WHERE RegionId=@0 ORDER BY DateTime DESC", regionId))
                {
                    var info = new List<string>();
                    while (reader.Read())
                    {
                        regionId = reader.Get<int>(_table.Columns[1].Name);
                        var userId = reader.Get<int>(_table.Columns[2].Name);
                        var actionName = reader.Get<string>(_table.Columns[3].Name);
                        var args = reader.Get<string>(_table.Columns[4].Name);
                        var undoArgs = reader.Get<string>(_table.Columns[5].Name);
                        var dateTime = reader.Get<DateTime>(_table.Columns[6].Name);
                        var action = ActionFactory.GetActionByName(actionName, args);
                        info.Add(string.Join(' ',
                            dateTime.ToString(Utils.DateFormat),
                            userId == 0 ? "Server" : TShock.UserAccounts.GetUserAccountByID(userId).Name,
                            string.Join(' ', action.GetInfoString())));
                    }
                    return info;
                }
            }
            catch (Exception e)
            {
                TShock.Log.Error(e.Message);
                return null;
            }
        }

        public void Redo(int count, int regionId)
        {
            if(_redoActions.ContainsKey(regionId))
            {
                while(count > 0 && _redoActions[regionId].Count > 0)
                {
                    count--;
                    var actionInfo = _redoActions[regionId].Pop();
                    var undoAction = actionInfo.Action.GetUndoAction(actionInfo.Action.GetUndoArgsString());
                    SaveAction(undoAction, TShock.Regions.GetRegionByID(regionId), TShock.UserAccounts.GetUserAccountByID(actionInfo.UserId), actionInfo.Date);
                    actionInfo.Action.Do();
                }
            }
        }

        private enum TableHistoryInfo
        {
            Id,
            RegionId,
            UserId,
            ActionName,
            Args,
            UndoArgs,
            DateTime
        }
    }

    public class ActionDBInfo
    {
        public int RegionId { get; set; }
        public int UserId { get; set; }
        public string ActionName { get; set; }
        public string Args { get; set; }
        public string UndoArgs { get; set; }
        public DateTime DateTime { get; set; }
    }

    public class ActionInfo 
    {
        public ActionInfo(IAction action, int regionId, int userId, DateTime date)
        {
            Action = action;
            RegionId = regionId;
            UserId = userId;
            Date = date;
        }

        public IAction Action { get; }
        public int RegionId { get; }
        public int UserId { get; }
        public DateTime Date { get; }
    }
}
