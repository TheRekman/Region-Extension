using RegionExtension.Database.EventsArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Database.Actions
{
    public class Allow : IAction
    {
        private string _regionName;
        private string _userName;

        public string Name => ActionFactory.AllowName;

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

        public Allow(string regionName, string userName)
        {
            _regionName = regionName;
            _userName = userName;
        }

        public Allow(AllowArgs args)
        {
            _regionName = args.Region.Name;
            _userName = args.User.Name;
        }

        public Allow(string fromArgs)
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
            new Remove(undoString);

        public string GetUndoArgsString() =>
            GetArgsString();

        public IEnumerable<string> GetInfoString() =>
            new string[]
            {
                Name + ":",
                _regionName,
                _userName
            };
    }
}
