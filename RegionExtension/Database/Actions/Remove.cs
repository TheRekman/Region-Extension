using RegionExtension.Database.EventsArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Database.Actions
{
    public class Remove : IAction
    {
        private string _regionName;
        private string _userName;

        public string Name => typeof(Remove).Name;

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

        public Remove(string regionName, string userName)
        {
            _regionName = regionName;
            _userName = userName;
        }

        public Remove(RemoveArgs args)
        {
            _regionName = args.Region.Name;
            _userName = args.User.Name;
        }

        public Remove(string fromArgs)
        {
            var args = fromArgs.Split(' ');
            _regionName = args[0];
            _userName = args[1];
        }

        public void Do() =>
            TShock.Regions.RemoveUser(_regionName, _userName);

        public string GetArgsString() =>
            string.Join(' ', Params);

        public IAction GetUndoAction(string undoString) =>
            new Allow(undoString);

        public string GetUndoArgsString() =>
            GetArgsString();
    }
}
