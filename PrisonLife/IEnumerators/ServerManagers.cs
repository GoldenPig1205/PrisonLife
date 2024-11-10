using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdminToys;
using Exiled.API.Features;
using MapEditorReborn.API.Features;
using MapEditorReborn.API.Features.Objects;
using MapEditorReborn.API.Features.Serializable;
using MEC;
using PlayerRoles;
using UnityEngine;
using static PrisonLife.Variables.Server;

namespace PrisonLife.IEnumerators
{
    public static class ServerManagers
    {
        public static IEnumerator<float> SendHeartbeat()
        {
            while (true)
            {
                Log.Info("heartbeat sent");

                yield return Timing.WaitForSeconds(30);
            }
        }

        public static IEnumerator<float> ClearChatCooldown()
        {
            while (true)
            {
                ChatCooldown.Clear();

                yield return Timing.WaitForSeconds(1);
            }
        }

        public static IEnumerator<float> SetRole()
        {
            while (true)
            {
                foreach (var player in Player.List)
                {
                    if (Physics.Raycast(player.Position, Vector3.down, out RaycastHit hit, 1, (LayerMask)1))
                    {
                        if (player.Role.Type == RoleTypeId.Scientist)
                        {
                            switch (hit.transform.name)
                            {
                                case "[SR] Prison":
                                    player.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.None);
                                    player.Kill($"수감자를 선택하셨습니다. 행운을 빕니다.");
                                    break;

                                case "[SR] Jailor":
                                    player.Role.Set(RoleTypeId.FacilityGuard, RoleSpawnFlags.None);
                                    player.Kill($"교도관을 선택하셨습니다. 행운을 빕니다.");
                                    break;
                            }
                        }

                        if (player.Role.Type == RoleTypeId.ClassD)
                        {
                            if (hit.transform.name == "[SP] Free")
                            {
                                PrisonLife.Instance.SpawnFree(player);
                            }
                        }
                    }
                }

                yield return Timing.WaitForOneFrame;
            }
        }

        public static IEnumerator<float> GodModeManager()
        {
            while (true)
            {
                foreach (var player in Player.List.Where(x => x.IsGodModeEnabled))
                {
                    PrimitiveSerializable sphere = new PrimitiveSerializable(PrimitiveType.Sphere, "58ACFAaa", PrimitiveFlags.Visible);
                    PrimitiveObject god = ObjectSpawner.SpawnPrimitive(sphere);
                    god.Position = player.Position;
                    god.Scale = new Vector3(3, 3, 3);

                    Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                    {
                        god.Destroy();
                    });
                }

                yield return Timing.WaitForOneFrame;
            }
        }
    }
}
