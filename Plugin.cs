using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;

using static PrisonLife.Variables.Protocol;

using static PrisonLife.EventHandlers.ServerEvent;
using static PrisonLife.EventHandlers.PlayerEvent;

using static PrisonLife.IEnumerators.ServerManagers;

namespace PrisonLife
{
    public class PrisonLife : Plugin<Config>
    {
        public override string Name => base.Name;
        public override string Author => "GoldenPig1205";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion => new Version(1, 2, 0, 5);

        public static PrisonLife Instance;

        CoroutineHandle _sendHeartBeat;

        public override void OnEnabled()
        {
            Instance = this;
            base.OnEnabled();

            WebhookURL = Config.WebhookURL;
            BotAPIServer = Config.BotAPIServer;

            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;

            _sendHeartBeat = Timing.RunCoroutine(SendHeartbeat());
        }

        public override void OnDisabled()
        {
            Instance = null;
            base.OnDisabled();

            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;

            Timing.KillCoroutines(_sendHeartBeat);
        }
    }
}
