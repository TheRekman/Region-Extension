using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension.Database.Actions
{
    public interface IAction
    {
        string Name { get; }
        object[] Params { get; }
        void Do();
        string GetArgsString();
        string GetUndoArgsString();
        IAction GetUndoAction(string undoString);
        IEnumerable<string> GetInfoString();
    }
}
