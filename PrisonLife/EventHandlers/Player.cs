using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MapEditorReborn.API.Features;
using MEC;
using PlayerRoles;
using PrisonLife.API.Features;

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
            Server.ExecuteCommand($"/effect FogControl 1 0 {ev.Player.Id}");
        }
    }
}
