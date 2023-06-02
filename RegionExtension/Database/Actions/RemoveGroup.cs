using RegionExtension.Database.EventsArgs;
using System.Collections.Generic;
using TShockAPI;

namespace RegionExtension.Database.Actions
{
    public class RemoveGroup : IAction
    {
        private string _regionName;
        private string _groupName;

        public string Name => ActionFactory.RemoveGroupName;

        public object[] Params
        {
            get
            {
                return new object[]
                {
                    _regionName, _groupName
                };
            }
        }

        public RemoveGroup(string regionName, string groupName)
        {
            _regionName = regionName;
            _groupName = groupName;
        }

        public RemoveGroup(RemoveGroupArgs args)
        {
            _regionName = args.Region.Name;
            _groupName = args.Group.Name;
        }

        public RemoveGroup(string fromArgs)
        {
            var args = fromArgs.Split(' ');
            _regionName = args[0];
            _groupName = args[1];
        }

        public void Do() =>
            TShock.Regions.RemoveGroup(_regionName, _groupName);

        public string GetArgsString() =>
            string.Join(' ', Params);

        public IAction GetUndoAction(string undoString) =>
            new AllowGroup(undoString);

        public string GetUndoArgsString() =>
            GetArgsString();

        public IEnumerable<string> GetInfoString() =>
            new string[]
            {
                Name + ":",
                _regionName,
                _groupName
            };
    }
}
