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
                try
                {
                    foreach (var player in Player.List.Where(x => x.IsAlive))
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
                                        if (Player.List.Where(x => x.IsNTF).Count() >= Server.PlayerCount / 2)
                                        {
                                            player.ShowHint($"너무 많은 유저가 교도관을 선택했습니다. 다른 직업을 선택해주세요.");
                                        }
                                        else
                                        {
                                            player.Role.Set(RoleTypeId.FacilityGuard, RoleSpawnFlags.None);
                                            player.Kill($"교도관을 선택하셨습니다. 행운을 빕니다.");
                                        }
                                        break;
                                }
                            }

                            if (player.Role.Type != RoleTypeId.Tutorial)
                            {
                                if (hit.transform.name == "[SP] Free")
                                    PrisonLife.Instance.SpawnFree(player);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                yield return Timing.WaitForOneFrame;
            }
        }

        public static IEnumerator<float> GodModeManager()
        {
            while (true)
            {
                try
                {
                    foreach (var player in Player.List.Where(x => x.IsAlive && x.IsGodModeEnabled))
                    {
                        if (player.Position != null)
                        {
                            SchematicObject shield = ObjectSpawner.SpawnSchematic("Shield", player.Position, new Quaternion(0, 0, 0, 0), new Vector3(1, 1, 1), null, false);

                            Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                            {
                                shield.Destroy();
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                yield return Timing.WaitForOneFrame;
            }
        }
    }
}
