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
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;

namespace RegionExtension.Database
{
    public class RegionHistoryManager
    {
        private IDbConnection _database;
        private Dictionary<int, Stack<ActionInfo>> _redoActions = new Dictionary<int, Stack<ActionInfo>>();

        private SqlTable _table =
            new SqlTable("RegionHistory",
                         new SqlColumn(TableHistoryInfo.Id.ToString(), MySqlDbType.Int32) { Primary = true, AutoIncrement = true },
                         new SqlColumn(TableHistoryInfo.RegionId.ToString(), MySqlDbType.Int32),
                         new SqlColumn(TableHistoryInfo.UserId.ToString(), MySqlDbType.Int32),
                         new SqlColumn(TableHistoryInfo.ActionName.ToString(), MySqlDbType.Text),
                         new SqlColumn(TableHistoryInfo.Args.ToString(), MySqlDbType.Text),
                         new SqlColumn(TableHistoryInfo.UndoArgs.ToString(), MySqlDbType.Text),
                         new SqlColumn(TableHistoryInfo.DateTime.ToString(), MySqlDbType.Text)
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

        public void SaveAction(IAction action, Region region, UserAccount user, DateTime dateTime, bool clearRedo = true)
        {
            
            var name = action.Name;
            var args = action.GetArgsString();
            var undoArgs = action.GetUndoArgsString();
            var regionId = region.ID;
            var userId = user == null ? 0 : user.ID;
            if (_redoActions.ContainsKey(regionId) && clearRedo)
                _redoActions.Remove(regionId);
            try
            {
                
                var variablesString = string.Join(", ", _table.Columns.Select(c => c.Name).Where(s => s != TableHistoryInfo.Id.ToString()));
                var values = "'" + string.Join("', '",
                             regionId, userId, name, args, undoArgs, dateTime.ToString()) + "'";
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
            var actions = new List<ActionInfo>();
            try
            {
                using (var reader = _database.QueryReader($"SELECT * FROM {_table.Name} WHERE RegionId=@0", regionId))
                {
                    while (reader.Read())
                    {
                        var id = reader.Get<int>(_table.Columns[0].Name);
                        regionId = reader.Get<int>(_table.Columns[1].Name);
                        var userId = reader.Get<int>(_table.Columns[2].Name);
                        var actionName = reader.Get<string>(_table.Columns[3].Name);
                        var args = reader.Get<string>(_table.Columns[4].Name);
                        var undoArgs = reader.Get<string>(_table.Columns[5].Name);
                        var dateTime = DateTime.Parse(reader.Get<string>(_table.Columns[6].Name));
                        var action = ActionFactory.GetActionByName(actionName, args);
                        actions.Add(new ActionInfo(id, action, regionId, userId, dateTime, undoArgs));
                    }
                }
            }
            catch (Exception e)
            {
                TShock.Log.Error(e.Message);
                return false;
            }
            var sortedActions = actions.OrderBy(a => a.Date).Reverse();
            foreach(var action in sortedActions)
            {
                var undoAction = action.Action.GetUndoAction(action.UndoStr);
                if (!_redoActions.ContainsKey(action.RegionId))
                    _redoActions.Add(action.RegionId, new Stack<ActionInfo>(10));
                _redoActions[action.RegionId].Push(new ActionInfo(action.Id, action.Action, action.RegionId, action.UserId, action.Date, action.UndoStr));
                undoAction.Do();
                _database.Query($"DELETE FROM {_table.Name} WHERE Id=@0", action.Id);
                count--;
                if (count < 1)
                    break;
            }
            return true;
        }

        public List<string> GetActionsInfo(int count, int regionId)
        {
            var actions = new List<ActionInfo>();
            var info = new List<string>();
            try
            {
                using (var reader = _database.QueryReader($"SELECT * FROM {_table.Name} WHERE RegionId=@0", regionId))
                {
                    while (reader.Read())
                    {
                        var id = reader.Get<int>(_table.Columns[0].Name);
                        regionId = reader.Get<int>(_table.Columns[1].Name);
                        var userId = reader.Get<int>(_table.Columns[2].Name);
                        var actionName = reader.Get<string>(_table.Columns[3].Name);
                        var args = reader.Get<string>(_table.Columns[4].Name);
                        var undoArgs = reader.Get<string>(_table.Columns[5].Name);
                        var dateTime = DateTime.Parse(reader.Get<string>(_table.Columns[6].Name));
                        var action = ActionFactory.GetActionByName(actionName, args);
                        actions.Add(new ActionInfo(id, action, regionId, userId, dateTime, undoArgs));
                    }
                }
            }
            catch (Exception e)
            {
                TShock.Log.Error(e.Message);
                return null;
            }
            info = actions.OrderBy(a => a.Date)
                          .Reverse()
                          .Select(a => string.Join(' ',
                                       a.Date.ToString(Utils.DateFormat),
                                       a.UserId == 0 ? "Server" : TShock.UserAccounts.GetUserAccountByID(a.UserId).Name,
                                       string.Join(' ', a.Action.GetInfoString())))
                          .ToList();
            return info;
        }

        public void Redo(int count, int regionId)
        {
            if(_redoActions.ContainsKey(regionId))
            {
                while(count > 0 && _redoActions[regionId].Count > 0)
                {
                    count--;
                    var actionInfo = _redoActions[regionId].Pop();
                    SaveAction(actionInfo.Action, TShock.Regions.GetRegionByID(regionId), TShock.UserAccounts.GetUserAccountByID(actionInfo.UserId), actionInfo.Date, false);
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
        public ActionInfo(int id, IAction action, int regionId, int userId, DateTime date, string undoStr)
        {
            Id = id;
            Action = action;
            RegionId = regionId;
            UserId = userId;
            Date = date;
            UndoStr = undoStr;
        }

        public int Id { get; }
        public IAction Action { get; }
        public int RegionId { get; }
        public int UserId { get; }
        public string UndoStr { get; }
        public DateTime Date { get; }
    }
}
