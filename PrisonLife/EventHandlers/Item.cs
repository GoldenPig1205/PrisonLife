using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Item;
using MEC;
using PrisonLife.API.Features;
using UnityEngine;

namespace PrisonLife.EventHandlers
{
    public static class ItemEvent
    {
        public static void OnSwinging(SwingingEventArgs ev)
        {
            if (Tools.TryGetLookHit(ev.Player, 1.2f, out RaycastHit hit))
            {
                if (hit.collider.transform.parent != null)
                {
                    if (hit.collider.transform.parent.name == "toilet")
                    {
                        Transform toilet = hit.transform.parent;

                        Hitmarker.SendHitmarkerDirectly(ev.Player.ReferenceHub, 1);

                        if (UnityEngine.Random.Range(1, 21) == 1)
                        {
                            Vector3 pos = toilet.position;

                            toilet.position = Vector3.zero;

                            Timing.CallDelayed(15, () =>
                            {
                                toilet.position = pos;
                            });
                        }
                    }
                }
            }
        }
    }
}
