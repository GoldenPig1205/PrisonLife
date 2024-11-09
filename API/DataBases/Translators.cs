using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonLife.API.DataBases
{
    public class Trans
    {
        public static Dictionary<RoleTypeId, string> Role = new Dictionary<RoleTypeId, string>
        {
            {RoleTypeId.Scp049, "SCP-049"},
            {RoleTypeId.Scp0492, "SCP-049-2"},
            {RoleTypeId.Scp079, "SCP-079"},
            {RoleTypeId.Scp096, "SCP-096"},
            {RoleTypeId.Scp106, "SCP-106"},
            {RoleTypeId.Scp173,  "SCP-173"},
            {RoleTypeId.Scp939, "SCP-939"},
            {RoleTypeId.Scp3114, "SCP-3114"},
            {RoleTypeId.ClassD, "D계급"},
            {RoleTypeId.Scientist, "과학자"},
            {RoleTypeId.FacilityGuard, "시설 경비"},
            {RoleTypeId.ChaosConscript, "반란 징집병"},
            {RoleTypeId.ChaosMarauder, "반란 약탈자"},
            {RoleTypeId.ChaosRepressor, "반란 압제자"},
            {RoleTypeId.ChaosRifleman, "반란 소총수"},
            {RoleTypeId.NtfCaptain, "구미호 대위"},
            {RoleTypeId.NtfPrivate, "구미호 이등병"},
            {RoleTypeId.NtfSergeant, "구미호 병장"},
            {RoleTypeId.NtfSpecialist, "구미호 상등병"},
            {RoleTypeId.Spectator, "관전자"},
            {RoleTypeId.None, "알 수 없음"},
            {RoleTypeId.Tutorial, "튜토리얼"},
            {RoleTypeId.Overwatch, "오버워치"},
            {RoleTypeId.Filmmaker, "필름메이커"},
        };

        public static Dictionary<ItemType, string> Item { get; set; } = new Dictionary<ItemType, string>
        {
            {ItemType.GunA7, "A7"},
            {ItemType.GunAK, "AK"},
            {ItemType.GunCOM18, "COM-18"},
            {ItemType.GunCOM15, "COM-15"},
            {ItemType.GunCom45, "COM-45"},
            {ItemType.GunCrossvec, "CROSSVEC"},
            {ItemType.GunE11SR, "MTF-E11-SR"},
            {ItemType.GunFRMG0, "FR-MG-0"},
            {ItemType.GunFSP9, "FPS-9"},
            {ItemType.GunLogicer, "LOGICER"},
            {ItemType.GunRevolver, ".44 REVOLVER"},
            {ItemType.GunShotgun, "SHOTGUN"},
            {ItemType.Adrenaline, "아드레날린"},
            {ItemType.Ammo12gauge, "탄약 12 게이지"},
            {ItemType.Ammo44cal, "탄약 44"},
            {ItemType.Ammo556x45, "탄약 556x45"},
            {ItemType.Ammo762x39, "탄약 762x39"},
            {ItemType.Ammo9x19, "탄약 9x19"},
            {ItemType.ArmorLight, "경량 방탄복"},
            {ItemType.ArmorCombat, "전투 방탄복"},
            {ItemType.ArmorHeavy, "고강도 방탄복"},
            {ItemType.Coin, "동전"},
            {ItemType.Flashlight, "손전등"},
            {ItemType.GrenadeFlash, "섬광탄"},
            {ItemType.GrenadeHE, "고폭 수류탄"},
            {ItemType.Jailbird, "제일버드"},
            {ItemType.KeycardChaosInsurgency, "혼돈의 반란 접속 장치"},
            {ItemType.KeycardContainmentEngineer, "격리 정비사 키카드"},
            {ItemType.KeycardFacilityManager, "시설 관리자 키카드"},
            {ItemType.KeycardGuard, "경비 키카드"},
            {ItemType.KeycardJanitor, "잡역부 키카드"},
            {ItemType.KeycardMTFCaptain, "구미호 대위 키카드"},
            {ItemType.KeycardMTFOperative, "구미호 대원 키카드"},
            {ItemType.KeycardMTFPrivate, "구미호 이등병 키카드"},
            {ItemType.KeycardO5, "O5-등급 키카드"},
            {ItemType.KeycardResearchCoordinator, "연구 감독관 키카드"},
            {ItemType.KeycardScientist, "과학자 키카드"},
            {ItemType.KeycardZoneManager, "구역 관리자 키카드"},
            {ItemType.Lantern, "고전 오일 랜턴"},
            {ItemType.Medkit, "구급 상자"},
            {ItemType.MicroHID, "마이크로 H.I.D"},
            {ItemType.Painkillers, "진통제"},
            {ItemType.ParticleDisruptor, "3-X 입자 분열기"},
            {ItemType.Radio, "무전기"},
            {ItemType.SCP244b, "SCP-244b"},
            {ItemType.SCP018, "SCP-018"},
            {ItemType.SCP1576, "SCP-1576"},
            {ItemType.SCP1853, "SCP-1853"},
            {ItemType.SCP207, "SCP-207"},
            {ItemType.SCP2176, "SCP-2176"},
            {ItemType.SCP244a, "SCP-244a"},
            {ItemType.SCP268, "SCP-268"},
            {ItemType.SCP330, "SCP-330"},
            {ItemType.SCP500, "SCP-500"},
            {ItemType.AntiSCP207, "SCP-207?"},
            {ItemType.None, "알 수 없음"}
        };
    }
}
