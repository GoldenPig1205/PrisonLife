using System.Collections.Generic;
using System;
using UnityEngine;

using Exiled.API.Features;
using MultiBroadcast.API;
using System.Linq;
using PrisonLife;
using PrisonLife.API.Features;
using MapEditorReborn.API.Features.Objects;

public class ShowTime : MonoBehaviour
{
    float St = 0;
    string current = $"";

    void notice(string title, string description)
    {
        TimeSpan timeOfDay = TimeSpan.FromSeconds(St);
        string formattedTime = timeOfDay.ToString("mm\\:ss");
        string amPmDesignator = timeOfDay.Minutes < 12 ? "AM" : "PM";
        string timeset = amPmDesignator + " " + formattedTime;

        if (current != timeset)
        {
            foreach (var player in Player.List)
            {
                player.ClearBroadcasts();
                player.AddBroadcast(1, $"<size=25><mark=#FFD700aa><color=#000000><b>{title}</b></color></mark></size>\n<size=20><u><mark=#000000aa>{description}</mark></u></size>\n<size=15>[{timeset}]</size>", tag: "ShowTime");
            }
            
            current = timeset;
        }
    }

    void Update()
    {
        if (St < 480)
            St += Time.deltaTime * 10;

        else if ((480 < St && St <= 600) || (840 < St && St <= 960) || (1200 < St && St <= 1380))
            St += Time.deltaTime * 5;

        else if ((600 < St && St <= 840) || (960 < St && St <= 1200))
            St += Time.deltaTime * 3;

        else
            St += Time.deltaTime;

        if (St > 1440)
        {
            St = 0;
        }

        if (St < 480)
        {
            timestamp = Timestamp.lights_out;
            notice("소등", "모든 수감자는 반드시 각자 방에 있어야 합니다.");

            Tools.ChangeBackground(25000, "#000000");
        }
        else if (480 < St && St <= 600)
        {
            timestamp = Timestamp.breakfast;
            notice("아침 식사", "아침 식사 시간입니다. 급식소에서 아침 식사를 제공 받으십시오.");

            Tools.ChangeBackground(375000, "#000000");
        }
        else if (600 < St && St <= 840)
        {
            timestamp = Timestamp.yardtime;
            notice("운동 시간", "여러분, 운동 시간입니다. 운동장으로 가세요.");

            Tools.ChangeBackground(390000, "#CEF6F5");
        }
        else if (840 < St && St <= 960)
        {
            timestamp = Timestamp.lurnch;
            notice("점심 식사", "점심 식사 시간입니다. 전원 식당으로 반드시 출석하세요.");

            Tools.ChangeBackground(360000, "#F5ECCE");
        }
        else if (960 < St && St <= 1200)
        {
            timestamp = Timestamp.freetime;
            notice("자유 시간", "수감자들을 위한 자유 시간입니다.");

            Tools.ChangeBackground(350000, "#F5ECCE");
        }
        else if (1200 < St && St <= 1380)
        {
            timestamp = Timestamp.dinner;
            notice("저녁 식사", "모든 수감자는 급식소에서 저녁 식사를 해야 합니다.");

            Tools.ChangeBackground(250000, "#2A0A0A");
        }
        else if (1380 < St)
        {
            timestamp = Timestamp.lockdown;
            notice("폐방", "수감자는 문을 잠그기 위해 각자 방으로 돌아가야 합니다.");

            Tools.ChangeBackground(50000, "#000000");
        }

        if (timestamp != timestamp2)
        {
            timestamp2 = timestamp;
            PrisonLife.PrisonLife.Instance.OnTimeChanged(timestamp2);
        }
    }

    public Timestamp timestamp = Timestamp.lights_out;
    public Timestamp timestamp2 = Timestamp.lockdown;

}