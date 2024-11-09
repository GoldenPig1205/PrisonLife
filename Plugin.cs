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
using static PrisonLife.EventHandlers.MapEvent;

using static PrisonLife.IEnumerators.ServerManagers;
using UnityEngine;

namespace PrisonLife
{
    public class PrisonLife : Plugin<Config>
    {
        public override string Name => base.Name;
        public override string Author => "GoldenPig1205";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion => new Version(1, 2, 0, 5);

        public static PrisonLife Instance;

        public static ShowTime ShowTime;

        CoroutineHandle _sendHeartBeat;
        CoroutineHandle _clearChatCooldown;

        public override void OnEnabled()
        {
            Instance = this;
            base.OnEnabled();

            WebhookURL = Config.WebhookURL;
            BotAPIServer = Config.BotAPIServer;

            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.DroppedItem += OnDroppedItem;
            Exiled.Events.Handlers.Player.DroppingAmmo += OnDroppingAmmo;
            Exiled.Events.Handlers.Player.SpawnedRagdoll += OnSpawnedRagdoll;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Player.DamagingDoor += OnDamagingDoor;
            Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;

            Exiled.Events.Handlers.Map.PlacingBulletHole += OnPlacingBulletHole;
            Exiled.Events.Handlers.Map.PlacingBlood += OnPlacingBlood;

            _sendHeartBeat = Timing.RunCoroutine(SendHeartbeat());
            _clearChatCooldown = Timing.RunCoroutine(ClearChatCooldown());
        }

        public override void OnDisabled()
        {
            Instance = null;
            base.OnDisabled();

            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            Exiled.Events.Handlers.Player.DroppedItem -= OnDroppedItem;
            Exiled.Events.Handlers.Player.DroppingAmmo -= OnDroppingAmmo;
            Exiled.Events.Handlers.Player.SpawnedRagdoll -= OnSpawnedRagdoll;
            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
            Exiled.Events.Handlers.Player.DamagingDoor -= OnDamagingDoor;
            Exiled.Events.Handlers.Player.TogglingNoClip -= OnTogglingNoClip;

            Exiled.Events.Handlers.Map.PlacingBulletHole -= OnPlacingBulletHole;
            Exiled.Events.Handlers.Map.PlacingBlood -= OnPlacingBlood;

            Timing.KillCoroutines(_sendHeartBeat);
            Timing.KillCoroutines(_clearChatCooldown);
        }

        public void OnTimeChanged(Timestamp NewTimestamp)
        {
            void notice(string note)
            {
                Player.List.ToList().ForEach(x => x.ShowHint($"<size=40><mark=#000000aa><b>감옥 방송</b>\n{note}</mark>\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n", 5));
            }

            switch (NewTimestamp)
            {
                case Timestamp.lights_out:
                    notice("모든 수감자는 반드시 각자 방에 있어야 합니다.");
                    break;

                case Timestamp.breakfast:
                    notice("아침 식사 시간입니다. 급식소에서 아침 식사를 제공 받으십시오.");
                    break;

                case Timestamp.yardtime:
                    notice("여러분, 운동 시간입니다. 운동장으로 가세요.");
                    break;

                case Timestamp.lurnch:
                    notice("점심 식사 시간입니다. 전원 식당으로 반드시 출석하세요.");
                    break;

                case Timestamp.freetime:
                    notice("수감자들을 위한 자유 시간입니다.");
                    break;

                case Timestamp.dinner:
                    notice("모든 수감자는 급식소에서 저녁 식사를 해야 합니다.");
                    break;

                case Timestamp.lockdown:
                    notice("수감자는 문을 잠그기 위해 각자 방으로 돌아가야 합니다.");
                    break;
            }
        }
    }
}
