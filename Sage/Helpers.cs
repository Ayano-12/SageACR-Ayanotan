using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.Module.Target;
using AEAssist.Define;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Ayanotan.WhiteMage.Setting;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Plugin.Services;
using FFXIVClientStructs;
using FFXIVClientStructs.FFXIV.Client.Game;
using static Ayanotan.Sage.Himitu.HackingXIV;


namespace Ayanotan.Sage;

public static class Helpers
{
    public static IPlayerCharacter 自身 => Core.Me;

    public static uint 自身血量 => Core.Me.CurrentHp;

    public static uint 自身蓝量 => Core.Me.CurrentMp;

    public static float 自身血量百分比 => Core.Me.CurrentHpPercent();

    public static float 自身蓝量百分比 => Core.Me.CurrentMpPercent();

    public static int 队伍成员数量 => PartyHelper.Party.Count;

    public static uint 自身当前等级 => Core.Me.Level;
    public static bool 自生是否使用过(this uint spellId, int time = 60000)
    {
        return spellId.GetSpell().RecentlyUsed(time);
    }

    public static bool 是否在副本中()
    {
        return Core.Resolve<MemApiCondition>().IsBoundByDuty();
    }

    public static uint 当前地图id => Core.Resolve<MemApiZoneInfo>().GetCurrTerrId();

    public static int 副本人数()
    {
        return Core.Resolve<MemApiDuty>().DutyMembersNumber();
    }

    public static bool 自身是否在移动()
    {
        return Core.Resolve<MemApiMove>().IsMoving();
    }

    public static bool 自身是否在读条()
    {
        return Core.Me.IsCasting;
    }

    public static int 自身周围单位数量(int range)
    {
        return TargetHelper.GetNearbyEnemyCount(range);
    }

    public static bool 自身存在Buff(uint id)
    {
        return Core.Me.HasAura(id);
    }

    public static bool 自身存在其中Buff(List<uint> auras, int msLeft = 0)
    {
        return Core.Me.HasAnyAura(auras, msLeft);
    }

    public static uint 自身命中其中Buff(List<uint> auras, int msLeft = 0)
    {
        return Core.Me.HitAnyAura(auras, msLeft);
    }

    public static bool 自身存在Buff大于时间(uint id, int time)
    {
        return Core.Me.HasMyAuraWithTimeleft(id, time);
    }
    public static IBattleChara 获取拥有buff队员(uint buff)
    {
        return PartyHelper.CastableAlliesWithin30.LastOrDefault(agent => agent.HasAura(buff));
    }
    public static float 目标血量百分比 => Core.Me.GetCurrTarget().CurrentHpPercent();

    public static bool 目标战斗状态(IBattleChara target)
    {
        return target.InCombat();
    }

    public static IBattleChara 获取血量最低成员()
    {
        if (PartyHelper.CastableAlliesWithin30.Count == 0)
            return Core.Me;
        return PartyHelper.CastableAlliesWithin30
            .Where(r => r.CurrentHp > 0).MinBy(r => r.CurrentHpPercent());
    }

    public static bool 目标是否可见或在技能范围内(uint actionId)
    {
        return Core.Resolve<MemApiSpell>().GetActionInRangeOrLoS(actionId) is not (566 or 562);
    }

    public static bool 队员是否拥有可驱散状态()
    {
        return PartyHelper.CastableAlliesWithin30.Any(
            agent => agent.HasCanDispel() && agent.Distance(Core.Me) <= 30);
    }

    public static IBattleChara 获取可驱散队员()
    {
        return PartyHelper.CastableAlliesWithin30.LastOrDefault(agent =>
            agent.HasCanDispel() && agent.Distance(Core.Me) <= 30);
    }
    public static int 二十米视线内血量低于设定的队员数量(float hp)
    {
        return PartyHelper.CastableAlliesWithin20.Count(
            r => r.CurrentHp != 0 && r.CurrentHpPercent() <= hp
        );
    }
    public static int 二十五米视线内血量低于设定的队员数量(float hp)
    {
        return PartyHelper.CastableAlliesWithin25.Count(
            r => r.CurrentHp != 0 && r.CurrentHpPercent() <= hp
        );
    }

    public static int 三十米视线内血量低于设定的队员数量(float hp)
    {
        return PartyHelper.CastableAlliesWithin30.Count(
            r => r.CurrentHp != 0 && r.CurrentHpPercent() <= hp
        );
    }

    public static int 十米视线内血量低于设定的队员数量(float hp)
    {
        return PartyHelper.CastableAlliesWithin10.Count(
            r => r.CurrentHp != 0 && r.CurrentHpPercent() <= hp
        );
    }
    public static int 十五米视线内血量低于设定的队员数量(float hp)
    {
        return PartyHelper.CastableAlliesWithin15.Count(
            r => r.CurrentHp != 0 && r.CurrentHpPercent() <= hp
        );
    }
    public static bool 队员是否拥有BUFF(uint buff)
    {
        return PartyHelper.CastableAlliesWithin30.Any(agent => agent.HasAura(buff));
    }

    public static bool 队伍里是否有T()
    {
        if (PartyHelper.CastableTanks != null)
        {
            return true;
        }
        return false;
    }
    public static int 队伍里有几个T()
    {
        return PartyHelper.CastableTanks.Count;
        
    }
    public static bool 队伍里是否为轻锐小队()
    {
        if (PartyHelper.Party.Count <=4)
        {
            return true;
        }
        return false;
    }
    public static int GetNearbyEnemyCount(IBattleChara target, int spellCastRange, int damageRange)
    {
        if (target == null)
        {
            return 0;
        }

        if (target.Distance(Core.Me) >= (float)spellCastRange)
        {
            return 0;
        }

        Dictionary<uint, IBattleChara> enemysIn = TargetMgr.Instance.EnemysIn25;
        int num = 0;
        foreach (KeyValuePair<uint, IBattleChara> item in enemysIn)
        {
            if (item.Value.Distance(target, DistanceMode.IgnoreHeight) - target.HitboxRadius <= (float)damageRange)
            {
                num++;
            }
        }

        return num;
    }
    public static long GetTimeStamps()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalMilliseconds);
    }

    public static bool TargetHasAura(IBattleChara target, List<uint> auras, int timeLeft = 0)
    {
        return target.HasAnyAura(auras, timeLeft);
    }

    public static IBattleChara? SmartTargetCircleAOE(uint skill, int count, IBattleChara currentTarget, int spellCastRange,
         int damageRange) //技能，可攻击目标数
    {
        var canTargetObjects = TargetHelper.GetMostCanTargetObjects(skill, count); //可被该技能命中的最大目标数
        if (canTargetObjects != null && canTargetObjects.IsValid() && canTargetObjects.DistanceToPlayer() <= spellCastRange)
        {
            return canTargetObjects;
        }
        if (currentTarget != null &&
                 TargetHelper.GetNearbyEnemyCount(currentTarget, spellCastRange, damageRange) >= count)
        {
            return currentTarget;
        }

        return currentTarget;

    }

    public static bool IsTargetVisibleOrInRange(uint actionId, IBattleChara? target)
    {
        unsafe
        {
            if(Core.Me != null && target != null && target.IsTargetable)
            {
                var skilltarget = (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)target.Address;
                var me = (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)Core.Me.Address;
                if (ActionManager.GetActionInRangeOrLoS == null)
                {
                    return false;
                }
                return ActionManager.GetActionInRangeOrLoS(actionId, me, skilltarget) is not (566 or 562);
            }
            return false;
        }
    }
    public static void ActionRange()
    {
        if (SageSettings.Instance != null && SageSettings.Instance.ActionRange)
            Hook.EnablePatch(PatchType.ActionRange);
        else
            Hook.DisablePatch(PatchType.ActionRange);
    }

    public static void MovePermission()
    {
        if (SageSettings.Instance != null && SageSettings.Instance.MovePermission)
            Hook.EnablePatch(PatchType.MovePermission);
        else
            Hook.DisablePatch(PatchType.MovePermission);
    }

    public static void SkillPostActionMove()
    {
        if (SageSettings.Instance != null && SageSettings.Instance.SkillPostActionMove)
            Hook.EnablePatch(PatchType.SkillPostActionMove);
        else
            Hook.DisablePatch(PatchType.SkillPostActionMove);
    }
    public static void SpeedUP()
    {
        if (SageSettings.Instance != null && SageSettings.Instance.SpeedUP)
        {
            Hook.EnablePatch(PatchType.SpeedUP);
            LogHelper.Print("加速功能" + "已开启，当前速度:" + (SageSettings.Instance.加速量+1));
        }

        else
        {
            Hook.DisablePatch(PatchType.SpeedUP);
        }
            
    }
    public static void Hack()
    {
        Ayanotan.Sage.Helpers.SkillPostActionMove();
        Ayanotan.Sage.Helpers.MovePermission();
        Ayanotan.Sage.Helpers.ActionRange();
        Ayanotan.Sage.Helpers.SpeedUP();
    }
    public static void UpdateHack()
    {
        if (SageSettings.Instance == null)
            return;
        SageSettings instance = SageSettings.Instance;
        bool movePermission = instance.MovePermission;
        bool actionRange = instance.ActionRange;
        bool skillPostActionMove = instance.SkillPostActionMove;
        bool speedUP = instance.SpeedUP;
        if (instance.MovePermission != movePermission)
            Ayanotan.Sage.Helpers.MovePermission();
        if (instance.ActionRange != actionRange)
            Ayanotan.Sage.Helpers.ActionRange();
        if (instance.SpeedUP != speedUP)
            Ayanotan.Sage.Helpers.SpeedUP();
        if (instance.SkillPostActionMove == skillPostActionMove)
            return;
        Ayanotan.Sage.Helpers.SkillPostActionMove();
    }



    public static class Map
    {

        public const uint 领航明灯天狼星灯塔 = 160u;
        public const uint 武装圣域放浪神古神殿 = 188u;
        public const uint 乐欲之所瓯博讷修道院 = 826u;


        public const uint 欧米茄绝境验证战_时空狭缝 = 1122u;
        public const uint 幻想龙诗绝境战_诗想空间 = 968u;
        public const uint 亚历山大绝境战_差分闭合宇宙 = 887u;
        public const uint 究极神兵绝境战_禁绝幻想 = 777u;
        public const uint 巴哈姆特绝境战_巴哈姆特大迷宫 = 733u;
        public const uint 泽罗姆斯歼殛战_红月深处 = 1169u;
        public const uint 圆桌骑士幻巧战_奇点反应堆 = 1175u;
        public const uint 零式万魔殿荒天之狱4_施恩神座 = 1154u;
        public const uint 零式万魔殿荒天之狱3_十四席大堂 = 1152u;
        public const uint 零式万魔殿荒天之狱2_万魔的产房 = 1150u;
        public const uint 零式万魔殿荒天之狱1_滞淀海域 = 1148u;
        public const uint 异闻阿罗阿罗岛 = 1179u;
        public const uint 零式阿罗阿罗岛 = 1180u;

        public static readonly List<uint> 高难地图 =
        [
            欧米茄绝境验证战_时空狭缝,
            幻想龙诗绝境战_诗想空间,
            亚历山大绝境战_差分闭合宇宙,
            究极神兵绝境战_禁绝幻想,
            巴哈姆特绝境战_巴哈姆特大迷宫,
            泽罗姆斯歼殛战_红月深处,
            圆桌骑士幻巧战_奇点反应堆,
            零式万魔殿荒天之狱4_施恩神座,
            零式万魔殿荒天之狱3_十四席大堂,
            零式万魔殿荒天之狱2_万魔的产房,
            零式万魔殿荒天之狱1_滞淀海域,
            异闻阿罗阿罗岛,
            零式阿罗阿罗岛
        ];

        public static readonly List<uint> 不拉人地图 =
        [
            异闻阿罗阿罗岛,
            零式阿罗阿罗岛
        ];

        public const uint 优雷卡丰水之地 = 827u;
        public const uint 优雷卡常风之地 = 732u;
        public const uint 优雷卡恒冰之地 = 763u;
        public const uint 优雷卡涌火之地 = 795u;
        public const uint 南方博兹雅战线 = 920u;
        public const uint 扎杜诺尔高原 = 975u;

        public static readonly List<uint> 不挂再生地图 =
        [
            优雷卡丰水之地,
            优雷卡常风之地,
            优雷卡恒冰之地,
            优雷卡涌火之地,
            南方博兹雅战线,
            扎杜诺尔高原
        ];
    }
}
