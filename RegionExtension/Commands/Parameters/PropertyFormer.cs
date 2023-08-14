using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension.Commands.Parameters
{
    public class PropertyFormer
    {
        public PropertyFormer(string name, ICommandParam[] @params)
        {
            Name = name;
            Params = @params;
        }

        public string Name { get; set; }
        public ICommandParam[] Params { get; set; }
    }
}
