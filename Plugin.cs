using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;

using static PrisonLife.Variables.Server;

using static PrisonLife.EventHandlers.ServerEvent;
using static PrisonLife.EventHandlers.PlayerEvent;
using static PrisonLife.EventHandlers.MapEvent;
using static PrisonLife.EventHandlers.ItemEvent;

using static PrisonLife.IEnumerators.ServerManagers;

using UnityEngine;
using PrisonLife.API.Features;
using PlayerRoles;
using MapEditorReborn.API.Features;
using Exiled.API.Enums;
using Interactables.Interobjects.DoorUtils;
using Exiled.API.Features.Doors;

namespace PrisonLife
{
    public class PrisonLife : Plugin<Config>
    {
        public override string Name => base.Name;
        public override string Author => "GoldenPig1205";
        public override Version Version => new Version(1, 0, 15);
        public override Version RequiredExiledVersion => new Version(1, 2, 0, 5);

        public static PrisonLife Instance;

        public static ShowTime ShowTime;

        CoroutineHandle _sendHeartBeat;
        CoroutineHandle _clearChatCooldown;
        CoroutineHandle _setRole;
        CoroutineHandle _godModeManager;
        CoroutineHandle _checkTryExit;
        CoroutineHandle _healManager;
        CoroutineHandle _syncSpectatedHint;
        CoroutineHandle _restartManager;

        public override void OnEnabled()
        {
            Instance = this;
            base.OnEnabled();

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
            Exiled.Events.Handlers.Player.ReloadingWeapon += OnReloadingWeapon;
            Exiled.Events.Handlers.Player.ChangingItem += OnChangingItem;
            Exiled.Events.Handlers.Player.Handcuffing += OnHandcuffing;

            Exiled.Events.Handlers.Map.PlacingBulletHole += OnPlacingBulletHole;
            Exiled.Events.Handlers.Map.PlacingBlood += OnPlacingBlood;
            Exiled.Events.Handlers.Map.PickupAdded += OnPickupAdded;

            Exiled.Events.Handlers.Item.Swinging += OnSwinging;

            _sendHeartBeat = Timing.RunCoroutine(SendHeartbeat());
            _clearChatCooldown = Timing.RunCoroutine(ClearChatCooldown());
            _setRole = Timing.RunCoroutine(SetRole());
            _godModeManager = Timing.RunCoroutine(GodModeManager());
            _checkTryExit = Timing.RunCoroutine(CheckTryExit());
            _healManager = Timing.RunCoroutine(HealManager());
            _syncSpectatedHint = Timing.RunCoroutine(SyncSpectatedHint());
            _restartManager = Timing.RunCoroutine(RestartManager());
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
            Exiled.Events.Handlers.Player.ReloadingWeapon -= OnReloadingWeapon;
            Exiled.Events.Handlers.Player.ChangingItem -= OnChangingItem;
            Exiled.Events.Handlers.Player.Handcuffing -= OnHandcuffing;

            Exiled.Events.Handlers.Map.PlacingBulletHole -= OnPlacingBulletHole;
            Exiled.Events.Handlers.Map.PlacingBlood -= OnPlacingBlood;
            Exiled.Events.Handlers.Map.PickupAdded -= OnPickupAdded;

            Exiled.Events.Handlers.Item.Swinging -= OnSwinging;

            Timing.KillCoroutines(_sendHeartBeat);
            Timing.KillCoroutines(_clearChatCooldown);
            Timing.KillCoroutines(_setRole);
            Timing.KillCoroutines(_godModeManager);
            Timing.KillCoroutines(_checkTryExit);
            Timing.KillCoroutines(_healManager);
            Timing.KillCoroutines(_syncSpectatedHint);
            Timing.KillCoroutines(_restartManager);
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
                            door.RequiredPermissions = new DoorPermissions { RequiredPermissions = Interactables.Interobjects.DoorUtils.KeycardPermissions.Checkpoints };
                    }

                    foreach (var door in Door.List)
                    {
                        if (door.Base.DoorId >= 189 && door.Base.DoorId <= 190)
                            door.RequiredPermissions = new DoorPermissions { RequiredPermissions = Interactables.Interobjects.DoorUtils.KeycardPermissions.Checkpoints };
                    }

                    notice("모든 수감자는 반드시 각자 방에 있어야 합니다.");
                    break;

                case Timestamp.breakfast:
                    foreach (var door in Door.List)
                    {
                        if (door.Base.DoorId >= 204 && door.Base.DoorId <= 219)
                            door.RequiredPermissions = new DoorPermissions { RequiredPermissions = Interactables.Interobjects.DoorUtils.KeycardPermissions.None };
                    }

                    notice("아침 식사 시간입니다. 급식소에서 아침 식사를 제공 받으십시오.");
                    break;

                case Timestamp.yardtime:
                    foreach (var door in Door.List)
                    {
                        if (door.Base.DoorId >= 189 && door.Base.DoorId <= 190)
                            door.RequiredPermissions = new DoorPermissions { RequiredPermissions = Interactables.Interobjects.DoorUtils.KeycardPermissions.None };
                    }

                    notice("여러분, 운동 시간입니다. 운동장으로 가세요.");
                    break;

                case Timestamp.lurnch:
                    foreach (var door in Door.List)
                    {
                        if (door.Base.DoorId >= 189 && door.Base.DoorId <= 190)
                            door.RequiredPermissions = new DoorPermissions { RequiredPermissions = Interactables.Interobjects.DoorUtils.KeycardPermissions.Checkpoints };
                    }

                    notice("점심 식사 시간입니다. 전원 식당으로 반드시 출석하세요.");
                    break;

                case Timestamp.freetime:
                    foreach (var door in Door.List)
                    {
                        if (door.Base.DoorId >= 189 && door.Base.DoorId <= 190)
                            door.RequiredPermissions = new DoorPermissions { RequiredPermissions = Interactables.Interobjects.DoorUtils.KeycardPermissions.None };
                    }

                    notice("수감자들을 위한 자유 시간입니다.");
                    break;

                case Timestamp.dinner:
                    foreach (var door in Door.List)
                    {
                        if (door.Base.DoorId >= 189 && door.Base.DoorId <= 190)
                            door.RequiredPermissions = new DoorPermissions { RequiredPermissions = Interactables.Interobjects.DoorUtils.KeycardPermissions.Checkpoints };
                    }

                    notice("모든 수감자는 급식소에서 저녁 식사를 해야 합니다.");
                    break;

                case Timestamp.lockdown:
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

            player.RankName = "수감자";
            player.RankColor = "orange";

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
            player.AddItem(ItemType.GunCOM18);

            Vector3 pos = Tools.GetRandomValue(Tools.GetObjectList("[SP] Jailor")).transform.position;
            player.Position = new Vector3(pos.x, pos.y + 2, pos.z);

            player.IsGodModeEnabled = true;
            player.IsBypassModeEnabled = true;

            player.RankName = "교도관";
            player.RankColor = "silver";

            Timing.CallDelayed(7, () =>
            {
                player.IsGodModeEnabled = false;
            });
        }

        public void SpawnFree(Player player)
        {
            player.Role.Set(RoleTypeId.Tutorial, RoleSpawnFlags.None);

            Vector3 pos = Tools.GetRandomValue(Tools.GetObjectList("[SP] Free")).transform.position;
            player.Position = new Vector3(pos.x, pos.y + 2, pos.z);

            player.IsGodModeEnabled = true;
            player.IsBypassModeEnabled = false;

            player.RankName = "범죄자";
            player.RankColor = "red";

            Timing.CallDelayed(7, () =>
            {
                player.IsGodModeEnabled = false;
            });
        }
    }
}
