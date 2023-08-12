using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension.Packet
{
    public class ProjectileDestroyPacket : IPacket
    {
        public byte Id { get; } = 29;
        public short ProjectileId { get; set; } = 0;
        public byte Owner { get; set; } = 255;

        public ProjectileDestroyPacket(short id, byte owner)
        {
            ProjectileId = id;
            Owner = owner;
        }

    }
}
