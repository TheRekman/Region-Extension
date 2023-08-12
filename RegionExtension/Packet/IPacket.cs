using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension.Packet
{
    public interface IPacket
    {
        public byte Id { get; }
    }
}
