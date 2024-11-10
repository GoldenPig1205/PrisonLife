using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonLife.Variables
{
    public static class Server
    {
        public static List<Player> ChatCooldown = new List<Player>();
        public static List<Player> MeleeCooldown = new List<Player>();
        public static Dictionary<Player, int> HealingCooldown = new Dictionary<Player, int>();
    }
}
