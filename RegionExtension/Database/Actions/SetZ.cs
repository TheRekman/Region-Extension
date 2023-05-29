using RegionExtension.Database.EventsArgs;
using TShockAPI;

namespace RegionExtension.Database.Actions
{
    public class SetZ : IAction
    {
        private string _regionName;
        private int _z;

        public string Name => ActionFactory.SetZName;

        public object[] Params
        {
            get
            {
                return new object[]
                {
                    _regionName, _z
                };
            }
        }

        public SetZ(string regionName, int z)
        {
            _regionName = regionName;
            _z = z;
        }

        public SetZ(SetZArgs args)
        {
            _regionName = args.Region.Name;
            _z = args.Amount;
        }

        public SetZ(string fromArgs)
        {
            var args = fromArgs.Split(' ');
            _regionName = args[0];
            _z = int.Parse(args[1]);
        }

        public void Do() =>
            TShock.Regions.SetZ(_regionName, _z);

        public string GetArgsString() =>
            string.Join(' ', Params);

        public IAction GetUndoAction(string undoString) =>
            new SetZ(undoString);

        public string GetUndoArgsString()
        {
            int z = TShock.Regions.GetRegionByName(_regionName).Z;
            return string.Join(' ', _regionName, z);
        }
    }
}
