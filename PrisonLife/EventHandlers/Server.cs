using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using MapEditorReborn.API.Features;
using MEC;
using RoundRestarting;

namespace PrisonLife.EventHandlers
{
    public static class ServerEvent
    {
        public static void OnWaitingForPlayers()
        {
            Round.Start();
            Round.IsLocked = true;
            Map.IsDecontaminationEnabled = false;

            MapUtils.LoadMap("PL");

            var webhook = new Discord.Webhook();
            webhook.OnEnabled();

            var command = new Discord.Command();
            command.OnEnabled();
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
