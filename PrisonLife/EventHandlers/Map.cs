using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Map;
using Interactables.Interobjects.DoorUtils;
using InventorySystem;
using InventorySystem.Items.Firearms.Ammo;
using MapEditorReborn.API.Features;
using MEC;
using PlayerRoles;
using PrisonLife.API.Features;

namespace PrisonLife.EventHandlers
{
    public static class MapEvent
    {
        public static void OnPlacingBulletHole(PlacingBulletHoleEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        public static void OnPlacingBlood(PlacingBloodEventArgs ev)
        {
            ev.IsAllowed = false;
        }

        public static void OnPickupAdded(PickupAddedEventArgs ev)
        {
            if (ev.Pickup.Base.name.Contains($"[P]") || ev.Pickup.Base.name.Contains($"[O]"))
                return;

            Timing.CallDelayed(10, () =>
            {
                ev.Pickup.Destroy();
            });
        }
    }
}
