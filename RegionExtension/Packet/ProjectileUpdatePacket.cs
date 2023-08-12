using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension.Packet
{
    public class ProjectileUpdatePacket : IPacket
    {
        public byte Id { get; } = 27;
        public short ProjectileId { get; set; } = 0;
        public float PositionX { get; set; } = 0;
        public float PositionY { get; set; } = 0;
        public float VelocityX { get; set; } = 0;
        public float VelocityY { get; set; } = 0;
        public byte Owner { get; set; } = 255;
        public short Type { get; set; } = 0;
        public byte flags { get; set; } = 0;

        public ProjectileUpdatePacket(short id, float positionX, float positionY, float velocityX, float velocityY, byte owner, short type)
        {
            ProjectileId = id;
            PositionX = positionX;
            PositionY = positionY;
            VelocityX = velocityX;
            VelocityY = velocityY;
            Owner = owner;
            Type = type;
        }
    }
}
