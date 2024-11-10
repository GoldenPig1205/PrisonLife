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
using static PrisonLife.EventHandlers.ItemEvent;

using static PrisonLife.IEnumerators.ServerManagers;

using UnityEngine;
using PrisonLife.API.Features;
using PlayerRoles;
using MapEditorReborn.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Enums;

namespace PrisonLife
{
    public class PrisonLife : Plugin<Config>
    {
        public override string Name => base.Name;
        public override string Author => "GoldenPig1205";
        public override Version Version => new Version(1, 0, 1);
        public override Version RequiredExiledVersion => new Version(1, 2, 0, 5);

        public static PrisonLife Instance;

        public static ShowTime ShowTime;

        CoroutineHandle _sendHeartBeat;
        CoroutineHandle _clearChatCooldown;
        CoroutineHandle _setRole;
        CoroutineHandle _godModeManager;

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
            Exiled.Events.Handlers.Player.SpawnedRagdoll += OnSpawnedRagdoll;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Player.DamagingDoor += OnDamagingDoor;
            Exiled.Events.Handlers.Player.TogglingNoClip += OnTogglingNoClip;
            Exiled.Events.Handlers.Player.Hurting += OnHurting;
            Exiled.Events.Handlers.Player.Dying += OnDying;
            Exiled.Events.Handlers.Player.SearchingPickup += OnSearchingPickup;
            Exiled.Events.Handlers.Player.Handcuffing += OnHandcuffing;

            Exiled.Events.Handlers.Map.PlacingBulletHole += OnPlacingBulletHole;
            Exiled.Events.Handlers.Map.PlacingBlood += OnPlacingBlood;
            Exiled.Events.Handlers.Map.PickupAdded += OnPickupAdded;

            Exiled.Events.Handlers.Item.Swinging += OnSwinging;

            _sendHeartBeat = Timing.RunCoroutine(SendHeartbeat());
            _clearChatCooldown = Timing.RunCoroutine(ClearChatCooldown());
            _setRole = Timing.RunCoroutine(SetRole());
            _godModeManager = Timing.RunCoroutine(GodModeManager());
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
            Exiled.Events.Handlers.Player.SpawnedRagdoll -= OnSpawnedRagdoll;
            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
            Exiled.Events.Handlers.Player.DamagingDoor -= OnDamagingDoor;
            Exiled.Events.Handlers.Player.TogglingNoClip -= OnTogglingNoClip;
            Exiled.Events.Handlers.Player.Hurting -= OnHurting;
            Exiled.Events.Handlers.Player.Dying -= OnDying;
            Exiled.Events.Handlers.Player.SearchingPickup -= OnSearchingPickup;
            Exiled.Events.Handlers.Player.Handcuffing -= OnHandcuffing;

            Exiled.Events.Handlers.Map.PlacingBulletHole -= OnPlacingBulletHole;
            Exiled.Events.Handlers.Map.PlacingBlood -= OnPlacingBlood;
            Exiled.Events.Handlers.Map.PickupAdded -= OnPickupAdded;

            Exiled.Events.Handlers.Item.Swinging -= OnSwinging;

            Timing.KillCoroutines(_sendHeartBeat);
            Timing.KillCoroutines(_clearChatCooldown);
            Timing.KillCoroutines(_setRole);
            Timing.KillCoroutines(_godModeManager);
        }

        public void OnTimeChanged(Timestamp NewTimestamp)
        {
            void notice(string note)
            {
                foreach (var player in Player.List.Where(x => x.Role.Type != RoleTypeId.Scientist))
                    player.ShowHint($"<size=40><mark=#000000aa><b>감옥 방송</b>\n{note}</mark>\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n", 5);
            }

            switch (NewTimestamp)
            {
                case Timestamp.lights_out:
                    foreach (var door in Door.List)
                    {
                        if (door.Base.DoorId >= 204 && door.Base.DoorId <= 219)
                            door.Lock(99999, DoorLockType.SpecialDoorFeature);
                    }

                    foreach (var door in Door.List)
                    {
                        if (door.Base.DoorId >= 189 && door.Base.DoorId <= 190)
                            door.Lock(99999, DoorLockType.SpecialDoorFeature);
                    }

                    notice("모든 수감자는 반드시 각자 방에 있어야 합니다.");
                    break;

                case Timestamp.breakfast:
                    foreach (var door in Door.List)
                    {
                        if (door.Base.DoorId >= 204 && door.Base.DoorId <= 219)
                            door.Unlock();
                    }

                    notice("아침 식사 시간입니다. 급식소에서 아침 식사를 제공 받으십시오.");
                    break;

                case Timestamp.yardtime:
                    foreach (var door in Door.List)
                    {
                        if (door.Base.DoorId >= 189 && door.Base.DoorId <= 190)
                            door.Unlock();
                    }

                    notice("여러분, 운동 시간입니다. 운동장으로 가세요.");
                    break;

                case Timestamp.lurnch:
                    notice("점심 식사 시간입니다. 전원 식당으로 반드시 출석하세요.");
                    break;

                case Timestamp.freetime:
                    foreach (var door in Door.List)
                    {
                        if (door.Base.DoorId >= 189 && door.Base.DoorId <= 190)
                            door.Unlock();
                    }

                    notice("수감자들을 위한 자유 시간입니다.");
                    break;

                case Timestamp.dinner:

                    notice("모든 수감자는 급식소에서 저녁 식사를 해야 합니다.");
                    break;

                case Timestamp.lockdown:
                    foreach (var door in Door.List)
                    {
                        if (door.Base.DoorId >= 189 && door.Base.DoorId <= 190)
                            door.Lock(99999, DoorLockType.SpecialDoorFeature);
                    }

                    notice("수감자는 문을 잠그기 위해 각자 방으로 돌아가야 합니다.");
                    break;
            }
        }

        public void SpawnPrison(Player player)
        {
            player.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.None);

            Vector3 pos = Tools.GetRandomValue(Tools.GetObjectList("[SP] Prison")).transform.position;
            player.Position = new Vector3(pos.x, pos.y + 2, pos.z);

            player.IsGodModeEnabled = true;
            player.IsBypassModeEnabled = false;

            if (player.Group == null)
            {
                player.Group = new UserGroup { BadgeText = "수감자", BadgeColor = "orange" };
            }
            else
            {
                player.Group.BadgeText = "수감자";
                player.Group.BadgeColor = "orange";
            }

            Timing.CallDelayed(7, () =>
            {
                player.IsGodModeEnabled = false;
            });
        }

        public void SpawnJailor(Player player)
        {
            player.Role.Set(RoleTypeId.FacilityGuard, RoleSpawnFlags.None);

            player.ClearInventory();
            player.AddItem(ItemType.GunCOM15);
            player.AddItem(ItemType.Ammo9x19);

            Vector3 pos = Tools.GetRandomValue(Tools.GetObjectList("[SP] Jailor")).transform.position;
            player.Position = new Vector3(pos.x, pos.y + 2, pos.z);

            player.IsGodModeEnabled = true;
            player.IsBypassModeEnabled = true;

            if (player.Group == null)
            {
                player.Group = new UserGroup { BadgeText = "교도관", BadgeColor = "silver" };
            }
            else
            {
                player.Group.BadgeText = "교도관";
                player.Group.BadgeColor = "silver";
            }

            Timing.CallDelayed(7, () =>
            {
                player.IsGodModeEnabled = false;
            });
        }

        public void SpawnFree(Player player)
        {
            player.Role.Set(RoleTypeId.Tutorial, RoleSpawnFlags.None);

            player.ClearInventory();

            Vector3 pos = Tools.GetRandomValue(Tools.GetObjectList("[SP] Free")).transform.position;
            player.Position = new Vector3(pos.x, pos.y + 2, pos.z);

            player.IsGodModeEnabled = true;
            player.IsBypassModeEnabled = false;

            if (player.Group == null)
            {
                player.Group = new UserGroup { BadgeText = "범죄자", BadgeColor = "red" };
            }
            else
            {
                player.Group.BadgeText = "범죄자";
                player.Group.BadgeColor = "red";
            }

            Timing.CallDelayed(7, () =>
            {
                player.IsGodModeEnabled = false;
            });
        }
    }
}
