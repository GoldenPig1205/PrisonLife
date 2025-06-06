﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
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
using MapEditorReborn.API.Features.Objects;
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
            HealingCooldown.Add(ev.Player, 0);

            ev.Player.Role.Set(RoleTypeId.Scientist);

            ev.Player.ClearInventory();

            ev.Player.EnableEffect(EffectType.Invisible);
            ev.Player.Position = Tools.GetObject("[SP] Lobby").position;

            ev.Player.RankName = "중립";
            ev.Player.RankColor = "white";

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

        public static void OnLeft(LeftEventArgs ev)
        {
            HealingCooldown.Remove(ev.Player);

            if (ChatCooldown.Contains(ev.Player))
                ChatCooldown.Remove(ev.Player);

            if (MeleeCooldown.Contains(ev.Player))
                MeleeCooldown.Remove(ev.Player);

            if (CrimePrisons.ContainsKey(ev.Player))
                CrimePrisons.Remove(ev.Player);

            if (CrimeJailors.ContainsKey(ev.Player))
                CrimeJailors.Remove(ev.Player);
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
            if (!MeleeCooldown.Contains(ev.Player) && !ev.Player.IsCuffed)
            {
                if (Tools.TryGetLookPlayer(ev.Player, 1.5f, out Player target))
                {
                    MeleeCooldown.Add(ev.Player);

                    target.Hurt(ev.Player, 12.05f, DamageType.Custom, null, "무지성으로 뚜드려 맞았습니다.");

                    Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 0.5f);

                    if (ev.Player.Role.Type == RoleTypeId.ClassD)
                    {
                        if (!CrimePrisons.ContainsKey(ev.Player))
                        {
                            CrimePrisons.Add(ev.Player, true);

                            ev.Player.ShowHint($"범죄를 저질렀습니다. 주의하세요.");
                        }
                    }

                    Timing.CallDelayed(1, () =>
                    {
                        if (MeleeCooldown.Contains(ev.Player))
                            MeleeCooldown.Remove(ev.Player);
                    });
                }
            }
        }

        public static IEnumerator<float> OnHurting(HurtingEventArgs ev)
        {
            HealingCooldown[ev.Player] = 15;

            if (ev.Attacker == null)
                yield break;

            if (ev.Attacker.LeadingTeam == ev.Player.LeadingTeam)
            {
                if (ev.Player.IsNTF)
                    ev.IsAllowed = false;
            }

            if (ev.DamageHandler.Type == DamageType.Jailbird)
                ev.DamageHandler.Damage = 20;

            if (ev.Attacker.CurrentItem.Type == ItemType.GunCOM18)
            {
                ev.IsAllowed = false;

                ev.Player.EnableEffect(EffectType.Ensnared, 5f);

                for (int i = 1; i < 51; i++)
                {
                    ev.Player.CurrentItem = null;

                    yield return Timing.WaitForSeconds(0.1f);
                }

                ev.Player.DisableEffect(EffectType.Ensnared);
            }
        }

        public static IEnumerator<float> OnDying(DyingEventArgs ev)
        {
            RoleTypeId roleTypeId = ev.Player.Role.Type;

            if (ev.Attacker != null && ev.Attacker.IsNTF)
            {
                if (ev.Player.Role.Type == RoleTypeId.Tutorial)
                    roleTypeId = RoleTypeId.ClassD;

                if ((ev.Player.Role.Type == RoleTypeId.ClassD && !CrimePrisons.ContainsKey(ev.Player)) || ev.Player.IsNTF)
                {
                    if (CrimeJailors.ContainsKey(ev.Attacker))
                    {
                        CrimeJailors[ev.Attacker] += 1;

                        if (CrimeJailors[ev.Attacker] >= 3)
                        {
                            ev.Attacker.Role.Set(RoleTypeId.ClassD);
                            ev.Attacker.Kill("교도관 행동 지침을 너무 많이 위반하였습니다.");

                            JailorBans.Add(ev.Attacker.UserId);
                        }
                        else
                            ev.Attacker.ShowHint($"교도관 행동 지침을 {CrimeJailors[ev.Attacker]}번이나 위반했습니다.\n한번 더 위반하면 수감자로 투옥됩니다.");
                    }
                    else
                    {
                        CrimeJailors.Add(ev.Attacker, 1);

                        ev.Attacker.ShowHint($"교도관 행동 지침을 위반했습니다. 주의하세요.");
                    }
                }

                if (ev.Player.IsNTF) // 교도관
                {
                    if (CrimeJailors.ContainsKey(ev.Player))
                        CrimeJailors.Remove(ev.Player);
                }
                else if (ev.Player.Role.Type == RoleTypeId.ClassD) // 수감자
                {
                    if (CrimePrisons.ContainsKey(ev.Player))
                        CrimePrisons.Remove(ev.Player);
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
                case RoleTypeId.Scientist:
                    ev.Player.Role.Set(RoleTypeId.Scientist);

                    ev.Player.ClearInventory();

                    ev.Player.EnableEffect(EffectType.Invisible);
                    ev.Player.Position = Tools.GetObject("[SP] Lobby").position;
                    ev.Player.RankName = "중립";
                    ev.Player.RankColor = "white";

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
                    break;

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
            {
                ev.IsAllowed = false;
                return;
            }

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

            if (ev.Player.Role.Type == RoleTypeId.ClassD)
            {
                if (!CrimePrisons.ContainsKey(ev.Player))
                {
                    CrimePrisons.Add(ev.Player, true);

                    ev.Player.ShowHint($"범죄를 저질렀습니다. 주의하세요.");
                }
            }
        }

        public static void OnReloadingWeapon(ReloadingWeaponEventArgs ev)
        {
            if (ev.Item.Type == ItemType.GunCOM18)
                ev.IsAllowed = false;
        }

        public static IEnumerator<float> OnChangingItem(ChangingItemEventArgs ev)
        {
            if (ev.Item == null)
                yield break;

            switch (ev.Item.Type) 
            {
                case ItemType.GunCOM18:
                    if (ev.Item.As<Firearm>().Ammo > 2)
                        ev.Item.As<Firearm>().Ammo = 2;

                    Firearm gunCom18 = ev.Item.As<Firearm>();
                    break;

                case ItemType.KeycardJanitor:
                    SchematicObject plate = ObjectSpawner.SpawnSchematic("Plate", Vector3.zero, isStatic: true);

                    yield return Timing.WaitForSeconds(0.1f);

                    while (ev.Player.CurrentItem.Type == ItemType.KeycardJanitor)
                    {
                        Vector3 cameraPosition = ev.Player.CameraTransform.position;
                        Vector3 cameraForward = ev.Player.CameraTransform.forward;

                        Vector3 spawnPosition = cameraPosition + cameraForward * 0.5f;

                        plate.Position = spawnPosition;

                        yield return Timing.WaitForOneFrame;
                    }

                    plate.Destroy();

                    break;
            }
        }

        public static IEnumerator<float> OnHandcuffing(HandcuffingEventArgs ev)
        {
            if (ev.Player.IsNTF && (ev.Target.Role.Type == RoleTypeId.Tutorial || ev.Target.Role.Type == RoleTypeId.ClassD))
            {
                if (ev.Target.Role.Type == RoleTypeId.ClassD && !CrimePrisons.ContainsKey(ev.Target))
                {
                    if (CrimeJailors.ContainsKey(ev.Player))
                    {
                        CrimeJailors[ev.Player] += 1;

                        if (CrimeJailors[ev.Player] >= 3)
                        {
                            ev.Player.Role.Set(RoleTypeId.ClassD);
                            ev.Player.Kill("교도관 행동 지침을 너무 많이 위반하였습니다.");

                            JailorBans.Add(ev.Player.UserId);
                        }
                        else
                            ev.Player.ShowHint($"교도관 행동 지침을 {CrimeJailors[ev.Player]}번이나 위반했습니다.\n한번 더 위반하면 수감자로 투옥됩니다.");
                    }
                    else
                    {
                        CrimeJailors.Add(ev.Player, 1);

                        ev.Player.ShowHint($"교도관 행동 지침을 위반했습니다. 주의하세요.");
                    }
                }

                ev.Target.EnableEffect(EffectType.Ensnared);

                for (int i = 1; i < 6; i++)
                {
                    ev.Target.ShowHint($"{6 - i}초 후 감옥으로 이송됩니다.", 1.2f);

                    yield return Timing.WaitForSeconds(1f);
                }

                ev.Target.RemoveHandcuffs();
                ev.Target.ClearInventory();
                PrisonLife.Instance.SpawnPrison(ev.Target);
            }
        }
    }
}
