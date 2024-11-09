using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CustomRendering;
using Discord;
using Exiled.API.Features;
using Exiled.Events.Commands.Reload;
using MapEditorReborn.API.Features.Objects;
using MEC;
using MultiBroadcast.API;
using PlayerRoles;
using UnityEngine;

namespace PrisonLife.API.Features
{
    public class Tools
    {
        public static T GetRandomValue<T>(List<T> list)
        {
            System.Random random = new System.Random();
            int index = random.Next(0, list.Count);
            return list[index];
        }

        public static List<T> EnumToList<T>()
        {
            Array items = Enum.GetValues(typeof(T));
            List<T> itemList = new List<T>();

            foreach (T item in items)
            {
                if (!item.ToString().Contains("None"))
                    itemList.Add(item);
            }

            return itemList;
        }

        public static List<Transform> GetObjectList(string Name)
        {
            return GameObject.FindObjectsOfType<Transform>().Where(t => t.name == Name).ToList();
        }

        public static Transform GetObject(string Name)
        {
            return GetObjectList(Name).FirstOrDefault();
        }

        public static Color GetRandomColor(bool Transparency = false)
        {
            if (!Transparency)
                return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);

            else
                return new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        }

        public static bool TryGetNearestPlayer(Player player, out Player nearestPlayer, out float radius)
        {
            nearestPlayer = null;
            radius = 99999;

            foreach (var near in Player.List.Where(x => x.IsAlive && x != player))
            {
                float Distance = Vector3.Distance(near.Position, player.Position);

                if (Distance < radius)
                {
                    nearestPlayer = near;
                    radius = Distance;
                }
            }

            if (nearestPlayer != null)
                return true;

            else
                return false;
        }

        public static bool TryGetLookPlayer(Player player, float Distance, out Player target)
        {
            target = null;

            if (Physics.Raycast(player.ReferenceHub.PlayerCameraReference.position + player.ReferenceHub.PlayerCameraReference.forward * 0.2f, player.ReferenceHub.PlayerCameraReference.forward, out RaycastHit hit, Distance, InventorySystem.Items.Firearms.Modules.StandardHitregBase.HitregMask) &&
                    hit.collider.TryGetComponent<IDestructible>(out IDestructible destructible))
            {
                if (Player.TryGet(hit.collider.GetComponentInParent<ReferenceHub>(), out Player t) && player != t)
                {
                    target = t;

                    return true;
                }
            }

            return false;
        }

        public static Player SpawnDJ(string name, RoleTypeId roleTypeId, Vector3 position, string sn = null)
        {
            ReferenceHub dj = GGUtils.Gtool.Spawn(roleTypeId, position);

            if (sn == null)
                sn = $"{UnityEngine.Random.value}";

            Dictionary<ReferenceHub, string> register = new Dictionary<ReferenceHub, string>()
            {
                { dj, sn }
            };

            foreach (var reg in register)
            {
                try
                {
                    GGUtils.Gtool.Register(reg.Key, reg.Value);
                }
                catch
                {
                }
            }

            GGUtils.Gtool.PlayerGet(sn).DisplayNickname = name;

            return Player.Get(dj);
        }


        public static List<Vector3> GetCirclePoints(Vector3 center, float radius, int pointCount)
        {
            List<Vector3> points = new List<Vector3>();
            float angleStep = 2 * Mathf.PI / pointCount;

            for (int i = 0; i < pointCount; i++)
            {
                float angle = i * angleStep;
                float x = center.x + radius * Mathf.Cos(angle);
                float z = center.z + radius * Mathf.Sin(angle);
                points.Add(new Vector3(x, center.y, z));
            }

            return points;
        }

        public static void ChangeBackground(float intensity, string color)
        {
            Tools.GetObjectList("[L] Base").ForEach(x => x.GetComponent<Light>().intensity = intensity);

            Color formattedColor = ColorUtility.TryParseHtmlString(color, out Color parsedColor) ? parsedColor : Color.white;
            Tools.GetObjectList("[BG] SkyBlock").ForEach(x => x.GetComponent<PrimitiveObject>().Primitive.Color = formattedColor);
        }
    }
}
