using RegionExtension.Database.EventsArgs;
using TShockAPI;

namespace RegionExtension.Database.Actions
{
    public class Rename : IAction
    {
        private string _regionName;
        private string _newName;

        public string Name => ActionFactory.RenameName;

        public object[] Params
        {
            get
            {
                return new object[]
                {
                    _regionName, _newName
                };
            }
        }

        public Rename(string regionName, string userName)
        {
            _regionName = regionName;
            _newName = userName;
        }

        public Rename(RenameArgs args)
        {
            _regionName = args.Region.Name;
            _newName = args.NewName;
        }

        public Rename(string fromArgs)
        {
            var args = fromArgs.Split(' ');
            _regionName = args[0];
            _newName = args[1];
        }

        public void Do() =>
            TShock.Regions.RenameRegion(_regionName, _newName);

        public string GetArgsString() =>
            string.Join(' ', Params);

        public IAction GetUndoAction(string undoString) =>
            new Rename(undoString);

        public string GetUndoArgsString() =>
            string.Join(' ', _newName, _regionName);
    }
}
