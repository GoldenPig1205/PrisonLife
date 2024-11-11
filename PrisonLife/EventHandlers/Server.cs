using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.Events.EventArgs.Server;
using MapEditorReborn.API.Features;
using MEC;
using PrisonLife.API.Features;
using RoundRestarting;
using UnityEngine;

using static PrisonLife.Variables.Server;

namespace PrisonLife.EventHandlers
{
    public static class ServerEvent
    {
        public static void OnWaitingForPlayers()
        {
            Server.FriendlyFire = true;
            Round.Start();
            Round.IsLocked = true;
            Map.IsDecontaminationEnabled = false;

            var webhook = new Discord.Webhook();
            webhook.OnEnabled();

            var command = new Discord.Command();
            command.OnEnabled();
        }

        public static void OnRoundStarted()
        {
            MapUtils.LoadMap("PL");

            Lobby = Tools.GetObject("[SP] Lobby");
            BaseLights = Tools.GetObjectList("[L] Base");
            SkyBlocks = Tools.GetObjectList("[BG] SkyBlock");

            GameObject gameobject = GameObject.Instantiate(new GameObject());
            PrisonLife.ShowTime = gameobject.AddComponent<ShowTime>();
        }

        public static void OnRoundEnded(RoundEndedEventArgs ev)
        {
            Timing.CallDelayed(5f, () =>
            {
                Server.ExecuteCommand($"sr");
            });
        }
    }
}
