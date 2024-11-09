using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Exiled.API.Features;
using MEC;

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
    }
}
