using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension.Database.Actions
{
    public static class ActionFactory
    {
        public static IAction GetActionByName(string name, string args)
        {
            IAction action = null;
            switch(name)
            {
                case ("Allow"):
                    action = new Allow(args);
                    break;
                case ("Remove"):
                    action = new Remove(args);
                    break;
                case ("SetZ"):
                    action = new SetZ(args);
                    break;
                case ("AllowG"):
                    action = new 
            }
            return action;
        }
    }
}
