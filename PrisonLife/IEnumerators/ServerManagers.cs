using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Exiled.API.Features;
using MEC;

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
    }
}
