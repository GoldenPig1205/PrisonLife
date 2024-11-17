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

using DiscordInteraction.Discord;
using CustomPlayerEffects;
using Exiled.API.Features.Items;

using static PrisonLife.Variables.Server;
using MEC;

namespace PrisonLife.Commands.Client.Basic
{
    // [CommandHandler(typeof(ClientCommandHandler))]
    public class Chat : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (ChatCooldown.Contains(player))
            {
                response = "너무 빠른 간격으로 입력을 보내고 있습니다!";
                return false;
            }
            else if (player.IsMuted)
            {
                response = "뮤트된 상태입니다.";
                return false;
            }
            else
            {
                ChatCooldown.Add(player);

                string ColorFormat(string cn)
                {
                    if (ColorUtility.TryParseHtmlString(cn, out Color color))
                        return color.ToHex();

                    else
                    {
                        var cd = Datas.Colors;

                        if (cd.ContainsKey(cn))
                            return cd[cn];

                        else
                            return "#FFFFFF";
                    }
                }

                string BadgeFormat(Player p)
                {
                    if (p.Group != null && !p.BadgeHidden)
                        return $"[<color={ColorFormat(p.Group.BadgeColor)}>{p.Group.BadgeText}</color>] ";

                    else
                        return "";
                }

                string ChatFormat(string chatType)
                {
                    string text = Trans.Role[player.Role.Type];
                    string text2 = string.Concat(new string[]
                    {
                        $"<size=25>{BadgeFormat(player)}<color={player.Role.Color.ToHex()}>",
                        text,
                        $"</color> ({player.DisplayNickname}) <b> | </b>",
                        string.Join(" ", arguments).Replace("=", "❤️"),
                        "</size>"
                    });

                    bool Check(Player p)
                    {
                        if (player.VoiceChannel == VoiceChat.VoiceChatChannel.Intercom)
                            return true;

                        if (p.CurrentItem is Scp1576 scp1576)
                        {
                            if (scp1576.IsUsing)
                                return true;
                        }

                        if (chatType == "SCP 채팅")
                            return p.IsDead || p.IsScp;

                        else if (chatType == "관전자 채팅")
                            return p.IsDead;

                        else if (chatType == "근거리 채팅")
                            return p.IsDead || Vector3.Distance(p.Position, player.Position) <= 10;

                        else
                            return false;
                    }

                    foreach (Player ply in Player.List)
                    {
                        if (Check(ply))
                            ply.AddBroadcast(6, text2);
                    }

                    Webhook.Send($"**{chatType}**ㅣ`{player.DisplayNickname}`[{player.IPAddress}, {player.UserId}]({Trans.Role[player.Role.Type]}) - {string.Join(" ", arguments)}");

                    return $"'{text2}'";
                }

                if (arguments.Count == 0)
                {
                    response = "보낼 메세지를 입력해주세요.";
                    return false;
                }

                if (player.IsScp)
                {
                    response = ChatFormat("SCP 채팅");
                    return true;
                }
                else if (player.IsDead)
                {
                    response = ChatFormat("관전자 채팅");
                    return true;
                }
                else
                {
                    response = ChatFormat("근거리 채팅");
                    return true;
                }
            }
        }

        public string Command { get; } = "chat";
        public string[] Aliases { get; } = new string[] { "챗", "채팅", "ㅊ", "c" };
        public string Description { get; } = "[프리즌 라이프] 텍스트 채팅을 사용할 수 있습니다.";
    }
}
