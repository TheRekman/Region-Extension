﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Database.Actions
{
    public class AllowGroup : IAction
    {
        private string _regionName;
        private string _groupName;

        public string Name => "AllowG";

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

        public AllowGroup(string regionName, string groupName)
        {
            _regionName = regionName;
            _groupName = groupName;
        }

        public AllowGroup(string fromArgs)
        {
            var args = fromArgs.Split(' ');
            _regionName = args[0];
            _groupName = args[1];
        }


        public void Do() =>
            TShock.Regions.AllowGroup(_regionName, _groupName);

        public string GetArgsString() =>
            string.Join(' ', Params);

        public IAction GetUndoAction(string undoString) =>
            new RemoveGroup(undoString);

        public string GetUndoArgsString() =>
            GetArgsString();
    }
}
