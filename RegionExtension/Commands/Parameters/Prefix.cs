using Terraria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension.Commands.Parameters
{
    public class Prefix
    {
        public Prefix(int iD)
        {
            ID = iD;
        }

        public int ID { get; set; }
        public string Name { get => Lang.prefix[ID].Value;  }
    }
}
