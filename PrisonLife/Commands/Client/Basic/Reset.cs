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

using PrisonLife.Discord;
using CustomPlayerEffects;
using Exiled.API.Features.Items;

using static PrisonLife.Variables.Server;
using MEC;

namespace PrisonLife.Commands.Client.Basic
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Reset : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            player.Role.Set(RoleTypeId.Scientist);
            player.Kill($"직업 선택 로비로 돌아갑니다..");

            response = "직업 선택 로비로 돌아갑니다..";
            return true;
        }

        public string Command { get; } = "reset";
        public string[] Aliases { get; } = new string[] { "초기화", "리셋", "재설정" };
        public string Description { get; } = "[프리즌 라이프] 직업 선택 로비로 돌아갑니다.";
    }
}
