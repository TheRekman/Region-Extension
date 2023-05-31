using RegionExtension.Database.EventsArgs;
using System.Collections.Generic;
using TShockAPI;

namespace RegionExtension.Database.Actions
{
    public class ChangeOwner : IAction
    {
        private string _regionName;
        private string _userName;

        public string Name => ActionFactory.ChangeOwnerName;

        public object[] Params
        {
            get
            {
                return new object[]
                {
                    _regionName, _userName
                };
            }
        }

        public ChangeOwner(string regionName, string userName)
        {
            _regionName = regionName;
            _userName = userName;
        }

        public ChangeOwner(ChangeOwnerArgs args)
        {
            _regionName = args.Region.Name;
            _userName = args.User.Name;
        }

        public ChangeOwner(string fromArgs)
        {
            var args = fromArgs.Split(' ');
            _regionName = args[0];
            _userName = args[1];
        }

        public void Do() =>
            TShock.Regions.AddNewUser(_regionName, _userName);

        public string GetArgsString() =>
            string.Join(' ', Params);

        public IAction GetUndoAction(string undoString) =>
            new ChangeOwner(undoString);

        public string GetUndoArgsString() =>
            string.Join(' ', _regionName, TShock.Regions.GetRegionByName(_regionName).Owner);

        public IEnumerable<string> GetInfoString() =>
            new string[]
            {
                Name + ": ",
                _regionName,
                _userName
            };
    }
}
