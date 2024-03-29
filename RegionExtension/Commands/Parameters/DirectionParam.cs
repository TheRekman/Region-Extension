﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace RegionExtension.Commands.Parameters
{
    internal class DirectionParam : CommandParam<Direction>
    {
        public DirectionParam(string name, string description, bool optional = false, Direction defaultValue = null) :
            base(name, description, optional, defaultValue)
        {
        }

        public override bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            var types = Enum.GetValues<DirectionType>()
                            .Select(d => (str: d.ToString().Substring(0, 1).ToLower(), dir: d))
                            .Where(el => el.str == str || el.dir.ToString().ToLower() == str);
            var detectedType = types.Where(el => el.str == str || el.dir.ToString().ToLower() == str);
            if (detectedType.Count() == 0)
            {
                args?.Player.SendErrorMessage("Invalid direction '{0}'! Use {1}"
                                              .SFormat(Name, string.Join('/', types.Select(el => el.str), types.Select(el => el.dir.ToString().ToLower()))));
                return false;
            }
            _value = new Direction(detectedType.First().dir);
            return true;
        }
    }
}
