using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;

namespace RegionExtension.Commands.Parameters
{
    internal class ProjectileParam : CommandParam<Projectile>
    {
        public ProjectileParam(string name, string description, bool optional = false, Projectile defaultValue = null) :
    base(name, description, optional, defaultValue)
        {
        }

        public override bool TrySetValue(string str, CommandArgsExtension args = null)
        {
            Projectile projectile = null;
            var id = 0;
            if(int.TryParse(str, out id))
            {
                projectile = new Projectile();
                projectile.SetDefaults(id);
            }
            else
            {
                List<(int, string)> contains = new List<(int, string)>();
                List<(int, string)> startwith = new List<(int, string)>();
                for(int i = 0; i < str.Length; i++)
                {
                    if (Lang._projectileNameCache[i].Value.Contains(str, StringComparison.OrdinalIgnoreCase))
                        contains.Add(new(i, Lang._projectileNameCache[i].Value));
                    else if (Lang._projectileNameCache[i].Value.StartsWith(str, StringComparison.OrdinalIgnoreCase))
                        startwith.Add(new(i, Lang._projectileNameCache[i].Value));
                }
                if (startwith.Count != 1)
                    startwith.AddRange(contains);
                else if(startwith.Count == 1)
                {
                    projectile = new Projectile();
                    projectile.SetDefaults(id);
                }
                else if (startwith.Count > 1)
                {
                    args.Player.SendInfoMessage("Found more than one projectile! {0}".SFormat(startwith.Count));
                    args.Player.SendInfoMessage(string.Join(", ", startwith.Select(n => "({0}) {1}".SFormat(n.Item1.ToString(), n.Item2)).Take(20)));
                    return false;
                }
            }
            if (projectile == null)
            {
                args.Player.SendErrorMessage("Failed found projectile '{0}'!".SFormat(str));
                return false;
            }
            _value = projectile;
            return true;
        }

        
    }
}
