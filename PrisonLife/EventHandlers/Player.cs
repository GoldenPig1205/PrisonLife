using System;
using System.Collections.Generic;
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
using PrisonLife.API.Features;

using static PrisonLife.Variables.Server;

namespace PrisonLife.EventHandlers
{
    public static class PlayerEvent
    {
        public static void OnVerified(VerifiedEventArgs ev)
        {
            ev.Player.Role.Set(RoleTypeId.ClassD);
            ev.Player.Position = Tools.GetObject("[SP] Lobby").position;
        }

        public static void OnSpawned(SpawnedEventArgs ev)
        {
            ev.Player.EnableEffect(EffectType.SoundtrackMute);
            ev.Player.EnableEffect(EffectType.FogControl);
        }

        public static void OnDroppedItem(DroppedItemEventArgs ev)
        {
            Timing.CallDelayed(10, () =>
            {
                ev.Pickup.UnSpawn();
            });
        }

        public static void OnDroppingAmmo(DroppingAmmoEventArgs ev)
        {
            ev.IsAllowed = false;

            List<AmmoPickup> ammos = ev.Player.Inventory.ServerDropAmmo(ev.AmmoType.GetItemType(), ev.Amount, false);

            Timing.CallDelayed(10, () =>
            {
                foreach (AmmoPickup ammo in ammos)
                    ammo.DestroySelf();
            });
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
                if (Tools.TryGetLookPlayer(ev.Player, 1.2f, out Player target))
                {
                    MeleeCooldown.Add(ev.Player);

                    CustomDamageHandler cDH = new CustomDamageHandler(target, ev.Player, 12.05f, DamageType.Custom, "무지성으로 뚜드려 맞았습니다.");
                    target.Hurt(cDH);

                    Timing.CallDelayed(1, () =>
                    {
                        if (MeleeCooldown.Contains(ev.Player))
                            MeleeCooldown.Remove(ev.Player);
                    });
                }
            }
        }
    }
}
