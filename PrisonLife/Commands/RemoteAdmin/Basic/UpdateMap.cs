using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MultiBroadcast.API;
using PlayerRoles;
using PrisonLife.API.Features;
using PrisonLife.API.DataBases;
using UnityEngine;

using static PrisonLife.Variables.Server;
using static MapEditorReborn.API.API;
using MapEditorReborn.API.Features;
using MEC;

namespace PrisonLife.Commands.RemoteAdmin.Basic
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class UpdateMap : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            MapUtils.LoadMap("");
            Timing.CallDelayed(Timing.WaitForOneFrame, () => MapUtils.LoadMap("PL"));

            response = "Update map successfully!";

            return true;
        }

        public string Command { get; } = "updatemap";
        public string[] Aliases { get; } = new string[] { "um", "update" };
        public string Description { get; } = "[프리즌 라이프] 맵을 업데이트합니다.";
    }
}
