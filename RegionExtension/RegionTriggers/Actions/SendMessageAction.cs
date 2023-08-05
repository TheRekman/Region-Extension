using RegionExtension.Commands.Parameters;
using RegionExtension.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;
using IL.Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace RegionExtension.RegionTriggers.Actions
{
    internal class SendMessageAction : ITriggerAction
    {
        private string _text = string.Empty;
        public string Name => "msg";
        public string Description => "Send message.";

        public static ActionFormer Former { get; } = new ActionFormer("msg", "Send message to the player.",
                                                                      new ICommandParam[] {
                                                                          new ArrayParam<string>("text...", "which text")
                                                                      },
                                                                      (param, args) => CreateTriggerAction(param, args),
                                                                      s => new SendMessageAction(s))
                                                                      { Permission = Permissions.TriggerMessage };

        private SendMessageAction(string text)
        {
            _text = text;
        }

        public static ITriggerAction CreateTriggerAction(ICommandParam[] param, CommandArgsExtension args)
        {
            var text = string.Join(" ", (string[])param[0].Value);
            return new SendMessageAction(text);
        }

        public void Execute(TriggerActionArgs args)
        {
            args.Player.SendMessage(_text, Color.White);
        }

        public string GetArgsString() =>
            _text;
    }
}
