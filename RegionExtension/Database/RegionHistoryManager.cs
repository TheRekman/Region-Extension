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

namespace RegionExtension.Database
{
    public class RegionHistoryManager
    {
        private IDbConnection _database;
        private Dictionary<int, Stack<ActionInfo>> _redoActions;

        private SqlTable _regionActionsTable =
            new SqlTable("RegionHistory",
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
            creator.EnsureTableStructure(_regionActionsTable);
        }

        public void SaveAction(IAction action, Region region, UserAccount user)
        {
            var name = action.Name;
            var args = action.GetArgsString();
            var undoArgs = action.GetUndoArgsString();
            var regionId = region.ID;
            var userId = user.ID;
            var dateTime = DateTime.Now;
            try
            {
                var variablesString = string.Join(' ', _regionActionsTable.Columns.Select(c => c.Name));
                var values = "{0} {1} {2} {3} {4} {5}".SFormat(
                             regionId, userId, name, args, undoArgs, dateTime);
                _database.Query("INSERT INTO @0 (@1) VALUES (@2);", _regionActionsTable.Name, variablesString, values);
            }
            catch (Exception ex)
            {
                TShock.Log.Error(ex.Message);
            }
        }

        public void Undo(int count, int regionId)
        {
            try
            {
                using (var reader = _database.QueryReader("SELECT @0 FROM @1 WHERE RegionId=@2", count, _regionActionsTable.Name, regionId))
                {
                    while (reader.Read())
                    {
                        regionId = reader.Get<int>(_regionActionsTable.Columns[0].Name);
                        var userId = reader.Get<int>(_regionActionsTable.Columns[1].Name);
                        var actionName = reader.Get<string>(_regionActionsTable.Columns[2].Name);
                        var args = reader.Get<string>(_regionActionsTable.Columns[3].Name);
                        var undoArgs = reader.Get<string>(_regionActionsTable.Columns[4].Name);
                        var dateTime = reader.Get<DateTime>(_regionActionsTable.Columns[5].Name);
                        var action = ActionFactory.GetActionByName(actionName, args);
                        var undoAction = action.GetUndoAction(undoArgs);
                        if (!_redoActions.ContainsKey(regionId))
                            _redoActions.Add(regionId, new Stack<ActionInfo>(10));
                        _redoActions[regionId].Push(new ActionInfo(action, regionId, userId));
                        undoAction.Do();
                    }
                }
            }
            catch (Exception e)
            {
                TShock.Log.Error(e.Message);
            }
        }

        public List<string> GetActionsInfo(int count, int regionId)
        {
            try
            {
                using (var reader = _database.QueryReader("SELECT @0 FROM @1 WHERE RegionId=@2", count, _regionActionsTable.Name, regionId))
                {
                    var info = new List<string>();
                    while (reader.Read())
                    {
                        regionId = reader.Get<int>(_regionActionsTable.Columns[0].Name);
                        var userId = reader.Get<int>(_regionActionsTable.Columns[1].Name);
                        var actionName = reader.Get<string>(_regionActionsTable.Columns[2].Name);
                        var args = reader.Get<string>(_regionActionsTable.Columns[3].Name);
                        var undoArgs = reader.Get<string>(_regionActionsTable.Columns[4].Name);
                        var dateTime = reader.Get<DateTime>(_regionActionsTable.Columns[5].Name);
                        var action = ActionFactory.GetActionByName(actionName, args);
                        info.Add(string.Join(' ',
                            TShock.UserAccounts.GetUserAccountByID(userId).Name,
                            dateTime.ToString(),
                            action.GetInfoString()));
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
                    SaveAction(undoAction, TShock.Regions.GetRegionByID(regionId), TShock.UserAccounts.GetUserAccountByID(actionInfo.UserId));
                    actionInfo.Action.Do();
                }
            }
        }

        private enum TableHistoryInfo
        {
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
        public ActionInfo(IAction action, int regionId, int userId)
        {
            Action = action;
            RegionId = regionId;
            UserId = userId;
        }

        public IAction Action { get; }
        public int RegionId { get; }
        public int UserId { get; }
    }
}
