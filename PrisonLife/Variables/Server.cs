using Exiled.API.Features;
using PrisonLife.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PrisonLife.Variables
{
    public static class Server 
    {
        public static Transform Lobby;

        public static List<Transform> BaseLights;
        public static List<Transform> SkyBlocks;

        public static List<Player> ChatCooldown = new List<Player>();
        public static List<Player> MeleeCooldown = new List<Player>();

        public static List<Player> JailorBans = new List<Player>();

        public static Dictionary<Player, int> HealingCooldown = new Dictionary<Player, int>();
        public static Dictionary<Player, bool> CrimePrisons = new Dictionary<Player, bool>();
        public static Dictionary<Player, int> CrimeJailors = new Dictionary<Player, int>();
    }
}
