using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;

namespace RegionExtension.Packet
{
    public class PacketConstructor
    {
        public static Dictionary<Type, Func<object, byte[]>> bytesConverts = new Dictionary<Type, Func<object, byte[]>>()
            {
                { typeof(short), o => BitConverter.GetBytes((short)o) },
                { typeof(float), o => BitConverter.GetBytes((float)o) },
                { typeof(byte), o => new byte[]{ (byte)o} },
                { typeof(int), o => BitConverter.GetBytes((int)o) }
            };

        public static byte[] GetBytes(IPacket packet)
        {
            List<byte> bytes = new List<byte>() { 0, 0 };
            short len = 2;
            foreach (var field in packet.GetType().GetProperties())
            {
                var tempBytes = bytesConverts[field.PropertyType](field.GetValue(packet));
                len += (short)tempBytes.Length;
                bytes.AddRange(tempBytes);
            }
            bytes[0] = (byte)((len >> 0) & 255);
            bytes[1] = (byte)((len >> 8) & 255);
            return bytes.ToArray();
        }

        public static void SendPacket(int target, IPacket packet)
        {
            byte[] bytes = GetBytes(packet);
            foreach(var plr in TShock.Players.Where(p => p != null && p.TPlayer.active && (target < 0 || p.TPlayer.whoAmI == target) && Netplay.Clients[p.TPlayer.whoAmI].IsActive))
                plr.SendRawData(bytes);
        }
    }
}
