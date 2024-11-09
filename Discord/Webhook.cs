using MEC;
using Respawning;
using System;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using PrisonLife.API.DataBases;

using Exiled.API.Features;

using static PrisonLife.Variables.Protocol;

namespace PrisonLife.Discord
{
    public class Webhook
    {
        public static Webhook Instance;

        public static async void Send(string msg, string url = null)
        {
            if (url == null)
                url = WebhookURL;

            await _send(msg, url);
        }

        private static async Task _send(string message, string url)
        {
            WebClient client = new WebClient();
            client.Headers.Add("Content-Type", "application/json");
            string payload = "{\"content\": \"" + message + "\"}";
            await client.UploadDataTaskAsync(url, Encoding.UTF8.GetBytes(payload));
        }

        public string _time
        {
            get
            {
                return DateTime.Now.AddHours(0).ToString("hh:mm:ss tt").ToUpper();
            }
        }

        public void OnEnabled()
        {
            Send($"[{_time}] :white_check_mark: **라운드**: 라운드 준비 완료.");

            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
            Exiled.Events.Handlers.Server.SelectingRespawnTeam += OnSelectingRespawnTeam;

            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.Handcuffing += OnHandcuffing;
            Exiled.Events.Handlers.Player.Left += OnLeft;
            Exiled.Events.Handlers.Player.RemovingHandcuffs += OnRemovingHandcuffs;
            Exiled.Events.Handlers.Player.Verified += OnVerified;

            Exiled.Events.Handlers.Warhead.Detonating += OnWarheadDetonating;
            Exiled.Events.Handlers.Warhead.Starting += OnWarheadStarting;
            Exiled.Events.Handlers.Warhead.Stopping += OnWarheadStopping;
        }

        public void OnRoundStarted()
        {
            Timing.CallDelayed(0.5f, () => Send($"[{_time}] :play_pause: **라운드**: 라운드 시작"));
        }

        public void OnRoundEnded(Exiled.Events.EventArgs.Server.RoundEndedEventArgs ev)
        {
            Send($"[{_time}] :end: **라운드**: 라운드 종료. - {ev.LeadingTeam}의 승리.");
        }

        public void OnDying(Exiled.Events.EventArgs.Player.DyingEventArgs ev)
        {
            if (ev.Player == null) return;
            if (ev.Player.ReferenceHub.isLocalPlayer) return;

            if (ev.Attacker == null || ev.Player == ev.Attacker)
            {
                Send($"[{_time}] :skull: **자살**: `{ev.Player.Nickname}` ({Trans.Role[ev.Player.Role.Type]}) | {ev.DamageHandler.Type}");
            }
            else
            {
                if (ev.Player.IsCuffed && !ev.Player.IsScp)
                {
                    Send($"[{_time}] :exclamation: **체포킬**: `{ev.Attacker.Nickname}` ({Trans.Role[ev.Attacker.Role.Type]}) -> `{ev.Player.Nickname}` ({Trans.Role[ev.Player.Role.Type]}) | {ev.DamageHandler.Type}\n체포자: `{ev.Player.Cuffer.Nickname}`");
                }
                else
                {
                    Send($"[{_time}] :skull: **사살**: `{ev.Attacker.Nickname}` ({Trans.Role[ev.Attacker.Role.Type]}) -> `{ev.Player.Nickname}` ({Trans.Role[ev.Player.Role.Type]}) | {ev.DamageHandler.Type}");
                }
            }
        }

        public void OnHandcuffing(Exiled.Events.EventArgs.Player.HandcuffingEventArgs ev)
        {
            if (ev.Target.ReferenceHub.isLocalPlayer) return;

            Send($"[{_time}] :lock: **체포**: `{ev.Player.Nickname}` ({Trans.Role[ev.Player.Role.Type]}) -> `{ev.Target.Nickname}` ({Trans.Role[ev.Target.Role.Type]})");
        }

        public void OnLeft(Exiled.Events.EventArgs.Player.LeftEventArgs ev)
        {
            if (ev.Player == null) return;
            if (ev.Player.ReferenceHub.isLocalPlayer) return;

            Send($"[{_time}] :busts_in_silhouette: **퇴장**: `{ev.Player.Nickname}` ({ev.Player.UserId} | {ev.Player.IPAddress})");
        }

        public void OnRemovingHandcuffs(Exiled.Events.EventArgs.Player.RemovingHandcuffsEventArgs ev)
        {
            if (ev.Target.ReferenceHub.isLocalPlayer) return;

            Send($"[{_time}] :unlock: **체포**: `{ev.Player.Nickname}` ({Trans.Role[ev.Player.Role.Type]}) -> `{ev.Target.Nickname}` ({Trans.Role[ev.Target.Role.Type]})");
        }

        public void OnVerified(Exiled.Events.EventArgs.Player.VerifiedEventArgs ev)
        {
            if (ev.Player.ReferenceHub.isLocalPlayer) return;
            Send($"[{_time}] :busts_in_silhouette: **접속**: `{ev.Player.Nickname}` ({ev.Player.UserId} | {ev.Player.IPAddress})");
        }

        public void OnSelectingRespawnTeam(Exiled.Events.EventArgs.Server.SelectingRespawnTeamEventArgs ev)
        {
            Send($"[{_time}] {(ev.Team == SpawnableTeamType.ChaosInsurgency ? ":red_car:" : ":helicopter:")} **지원**: {ev.Team}");
        }

        public void OnWarheadDetonating(Exiled.Events.EventArgs.Warhead.DetonatingEventArgs ev)
        {
            Send($"[{_time}] :boom: **핵**: 폭발");
        }

        public void OnWarheadStarting(Exiled.Events.EventArgs.Warhead.StartingEventArgs ev)
        {
            Send($"[{_time}] :boom: **핵**: 가동 시작 {(ev.Player != null ? $"- {ev.Player.Nickname}에 의해." : string.Empty)}");
        }

        public void OnWarheadStopping(Exiled.Events.EventArgs.Warhead.StoppingEventArgs ev)
        {
            if (Warhead.IsLocked) return;

            Send($"[{_time}] :boom: **핵**: 가동 중지 {(ev.Player != null ? $"- {ev.Player.Nickname}에 의해." : string.Empty)}");
        }
    }
}