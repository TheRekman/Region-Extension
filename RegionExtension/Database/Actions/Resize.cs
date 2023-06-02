using RegionExtension.Commands.Parameters;
using RegionExtension.Database.EventsArgs;
using System.Collections.Generic;
using TShockAPI;

namespace RegionExtension.Database.Actions
{
    public class Resize : IAction
    {
        private string _regionName;
        private int _amount;
        private int _direction;

        public string Name => ActionFactory.ResizeName;

        public object[] Params
        {
            get
            {
                return new object[]
                {
                    _regionName, _amount, _direction
                };
            }
        }

        public Resize(string regionName, int amount, int direction)
        {
            _regionName = regionName;
            _amount = amount;
            _direction = direction;
        }

        public Resize(ResizeArgs args)
        {
            _regionName = args.Region.Name;
            _amount = args.Amount;
            _direction = args.Amount;
        }

        public Resize(string fromArgs)
        {
            var args = fromArgs.Split(' ');
            _regionName = args[0];
            _amount = int.Parse(args[1]);
            _direction = int.Parse(args[2]);
        }

        public void Do() =>
            TShock.Regions.ResizeRegion(_regionName, _amount, _direction);

        public string GetArgsString() =>
            string.Join(' ', Params);

        public IAction GetUndoAction(string undoString) =>
            new Resize(undoString);

        public string GetUndoArgsString()
        {
            int amount = -_amount;
            int direction = _direction;
            return string.Join(' ', _regionName, amount, direction);
        }

        public IEnumerable<string> GetInfoString() =>
            new string[]
            {
                Name + ":",
                _regionName,
                _amount.ToString(),
                Direction.GetFromTshockDirection(_direction).ToString()
            };
    }
}
