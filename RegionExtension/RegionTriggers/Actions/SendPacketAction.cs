using RegionExtension.Commands.Parameters;
using RegionExtension.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using NuGet.Packaging;
using Microsoft.VisualBasic;

namespace RegionExtension.RegionTriggers.Actions
{
    public class SendPacketAction : ITriggerAction
    {
        public string Name => "packet";
        public string Description => "Send packet to the player.";

        public static ActionFormer Former { get; } = new ActionFormer("packet", "Send packet to the player.",
                                                                      new ICommandParam[]
                                                                      {
                                                                          new IntParam("int", "send data"),
                                                                          new StringParam("text", "package text", true, ""),
                                                                          new ArrayParam<int>("data...", "data of package. Max: 7 int", 0, true, new int[7])
                                                                      },
                                                                      (param, args) => CreateTriggerAction(param, args),
                                                                      s => FromString(s))
                                                                      { Permission = Permissions.TriggerSendPacket };
        private int _msgType;
        private string _text;
        private int[] _data = new int[7];

        public SendPacketAction(int msgType, string text, int[] data)
        {
            _msgType = msgType;
            _text = text;
            _data = data;
        }

        public static ITriggerAction CreateTriggerAction(ICommandParam[] param, CommandArgsExtension args)
        {
            var msgType = (int)param[0].Value;
            var text = (string)param[1].Value;
            var data = (int[])param[2].Value;
            data.Take(7);
            data.AddRange(new int[7 - data.Length]);
            return new SendPacketAction(msgType, text, data);
        } 

        public static ITriggerAction FromString(string str)
        {
            var splitedString = str.Split('\"');
            var msgType = int.Parse(splitedString[0]);
            var _text = string.Join("\"", splitedString.Skip(1).Take(splitedString.Length - 7 - 1));
            var data = splitedString.Skip(splitedString.Length - 7).Select(s => int.Parse(s)).ToArray();
            return new SendPacketAction(msgType, _text, data);
        }

        public void Execute(TriggerActionArgs args)
        {
            try 
            {
                NetMessage.SendData(_msgType, args.Player.Index, -1, string.IsNullOrEmpty(_text) ? null : new Terraria.Localization.NetworkText(_text, Terraria.Localization.NetworkText.Mode.Literal),
                    _data[0], _data[1], _data[2], _data[3], _data[4], _data[5], _data[6]);
            }
            catch (Exception ex)
            {
                args.Player.SendErrorMessage(ex.Message);

            }
        }

        public string GetArgsString() =>
            string.Join(" ", _msgType.ToString(), _text, string.Join("\"", _data));
    }
}
