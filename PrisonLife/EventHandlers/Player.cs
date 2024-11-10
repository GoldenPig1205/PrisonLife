using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.DamageHandlers;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Interactables.Interobjects.DoorUtils;
using InventorySystem;
using InventorySystem.Items.Firearms.Ammo;
using MapEditorReborn.API.Features;
using MEC;
using PlayerRoles;
using PrisonLife.API.DataBases;
using PrisonLife.API.Features;
using UnityEngine;

using static PrisonLife.Variables.Server;

namespace PrisonLife.EventHandlers
{
    public static class PlayerEvent
    {
        public static IEnumerator<float> OnVerified(VerifiedEventArgs ev)
        {
            ev.Player.Role.Set(RoleTypeId.Scientist);

            ev.Player.ClearInventory();

            ev.Player.EnableEffect(EffectType.Invisible);
            ev.Player.Position = Tools.GetObject("[SP] Lobby").position;
            ev.Player.Group.BadgeText = "중립";
            ev.Player.Group.BadgeColor = "white";

            ev.Player.ShowHint($"<b><size=40><size=50>[<color=#A4A4A4>교도관</color>]</size>\n<mark=#A4A4A4aa>수감자들을 잘 감시하세요. 불법 반입 물품 압수, 폭동 진압, 무엇보다도 탈옥 시도를 저지해야 합니다. 하지만 감옥을 위협하는 것이 죄수뿐만은 아니라는 것, 명심하세요.</mark>\n\n" +
                $"<size=50>[<color=#FF8000>수감자</color>]</size>\n<mark=#FF8000aa>가석방이 없는 종신형을 받은 무고한 시민인 당신, 어떤 희망도 미래도 보이지 않습니다. 지금 당신은 갈림길에 서있습니다. 평생 추운 감옥에 갇혀 의미 없는 나날을 보낼 것인가, 아니면 탈옥할 것인가...</mark></size></b>\n\n\n" +
                $"<color=#A4A4A4>교도관</color>으로 플레이하려면 <mark=#0080FFaa><color=#000000>파란색 발판</color></mark>을,\n<color=#FF8000>수감자</color>로 플레이하려면 <mark=#FF8000aa><color=#000000>주황색 발판</color></mark>을 밟으십시오.\n\n\n\n\n\n\n\n\n\n", 10000);

            while (true)
            {
                if (ev.Player.Role.Type != RoleTypeId.Scientist)
                {
                    ev.Player.ShowHint("", 1);
                    break;
                }

                yield return Timing.WaitForOneFrame;
            }
        }

        public static void OnSpawned(SpawnedEventArgs ev)
        {
            ev.Player.EnableEffect(EffectType.SoundtrackMute);
            ev.Player.EnableEffect(EffectType.FogControl);
        }

        public static void OnSpawnedRagdoll(SpawnedRagdollEventArgs ev)
        {
            Timing.CallDelayed(10, () => 
            {
                ev.Ragdoll.UnSpawn();
            });
        }

        public static void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (!ev.Door.IsOpen)
            {
                Timing.CallDelayed(3, () =>
                {
                    if (ev.Door.IsOpen)
                    {
                        ev.Player.IsBypassModeEnabled = true;

                        DoorVariant.AllDoors.Where(x => x.DoorId == ev.Door.Base.DoorId).FirstOrDefault().ServerInteract(ev.Player.ReferenceHub, 1);

                        if (!ev.Player.IsNTF)
                            ev.Player.IsBypassModeEnabled = false;
                    }
                });
            } 
        }

        public static void OnDamagingDoor(DamagingDoorEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        public static void OnTogglingNoClip(TogglingNoClipEventArgs ev)
        {
            if (!MeleeCooldown.Contains(ev.Player))
            {
                if (Tools.TryGetLookPlayer(ev.Player, 1.5f, out Player target))
                {
                    MeleeCooldown.Add(ev.Player);

                    target.Hurt(ev.Player, 12.05f, DamageType.Custom, null, "무지성으로 뚜드려 맞았습니다.");

                    Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 0.5f);

                    Timing.CallDelayed(1, () =>
                    {
                        if (MeleeCooldown.Contains(ev.Player))
                            MeleeCooldown.Remove(ev.Player);
                    });
                }
            }
        }

        public static void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == null)
                return;

            if (ev.Player.Role.Team == ev.Attacker.Role.Team)
            {
                if (ev.Player.IsNTF)
                    ev.IsAllowed = false;
            }

            if (ev.DamageHandler.Type == DamageType.Jailbird)
                ev.DamageHandler.Damage = 20;
        }

        public static IEnumerator<float> OnDying(DyingEventArgs ev)
        {
            RoleTypeId roleTypeId = ev.Player.Role.Type;

            if (ev.Attacker != null && ev.Attacker.IsNTF)
            {
                if (ev.Player.Role.Type == RoleTypeId.Tutorial)
                    roleTypeId = RoleTypeId.ClassD;

                try
                {
                    if (ev.Player.Items.Count == 0)
                    {
                        if (ev.Attacker.CustomInfo == null)
                            ev.Attacker.CustomInfo = "무고한 수감자 사살 횟수 : 1";

                        else
                        {
                            int count = int.Parse(ev.Attacker.CustomInfo.Split(':')[1].Trim());
                            ev.Attacker.CustomInfo = $"무고한 수감자 사살 횟수 : {count + 1}";

                            if (count > 2)
                            {
                                ev.Attacker.CustomInfo = null;
                                ev.Attacker.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.None);
                                ev.Attacker.Kill("무고한 죄수를 너무 많이 죽여버렸습니다.");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }

            if (ev.Player.IsNTF)
            {
                if (UnityEngine.Random.Range(1, 4) == 1)
                {
                    Item keycard = ev.Player.AddItem(ItemType.KeycardMTFPrivate);
                    ev.Player.DropItem(keycard);
                }
            }

            for (int i = 1; i < 6; i++)
            {
                ev.Player.ShowHint($"{6 - i}초 후 부활합니다.", 1.2f);

                yield return Timing.WaitForSeconds(1f);
            }

            switch (roleTypeId)
            {
                case RoleTypeId.ClassD:
                    PrisonLife.Instance.SpawnPrison(ev.Player);
                    break;

                case RoleTypeId.FacilityGuard:
                    PrisonLife.Instance.SpawnJailor(ev.Player);
                    break;

                case RoleTypeId.Tutorial:
                    PrisonLife.Instance.SpawnFree(ev.Player);
                    break;
            }
        }

        public static void OnSearchingPickup(SearchingPickupEventArgs ev)
        {
            if (ev.Pickup.Base.name.Contains("[O]"))
                return;

            if (ev.Pickup.Type.ToString().Contains($"Ammo"))
            {
                if (ev.Player.CountItem(ev.Pickup.Type) > 11)
                    return;
            }

            if (ev.Pickup.Type.ToString().Contains($"Armor"))
            {
                if (ev.Player.CountItem(ev.Pickup.Type) >= 1)
                    return;
            }

            ev.Player.AddItem(ev.Pickup.Type);

            if (!ev.Pickup.Base.name.Contains("[P]"))
                ev.Pickup.Destroy();
        }

        public static IEnumerator<float> OnHandcuffing(HandcuffingEventArgs ev)
        {
            if (ev.Player.IsNTF && (ev.Target.Role.Type == RoleTypeId.Tutorial || ev.Target.Role.Type == RoleTypeId.ClassD))
            {
                ev.Target.EnableEffect(EffectType.Ensnared);

                for (int i = 1; i < 6; i++)
                {
                    ev.Target.ShowHint($"{6 - i}초 후 감옥으로 이송됩니다.", 1.2f);

                    yield return Timing.WaitForSeconds(1f);
                }

                ev.Target.RemoveHandcuffs();
                PrisonLife.Instance.SpawnPrison(ev.Target);
            }
        }
    }
}
