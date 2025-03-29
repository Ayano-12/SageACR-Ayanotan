using AEAssist.CombatRoutine;
using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using Ayanotan.WhiteMage.Setting;
using Ayanotan.WhiteMage.Data;
using Ayanotan.Sage.Data;
using Ayanotan.Sage;
using AEAssist.Avoid;
using AEAssist.CombatRoutine.Module.Target;
using Dalamud.Game.ClientState.Objects.Types;
using System.Numerics;

namespace Ayanotan.Sage.SloResolvers.GCD;

public class SlotResolver_GCD_魂灵风息 : ISlotResolver
{
    // 摘要:
    //     指定距离的矩形范围内敌人的数量，包含跟自身目标圈重合的敌人
    //
    // 参数:
    //   me:
    //
    //   target:
    //
    //   length:
    //     长度
    //
    //   width:
    //     宽度
    private IBattleChara target;
    public static int GetEnemyCountInsideRect(IBattleChara me, IBattleChara target, float length, float width)
    {
        Dictionary<uint, IBattleChara> enemysIn = TargetMgr.Instance.EnemysIn25;
        int num = 0;
        Vector3 vector = Vector3.Normalize(target.Position - me.Position);
        foreach (KeyValuePair<uint, IBattleChara> item in enemysIn)
        {
            RectShape rectShape = RectShape.Create(Core.Me.Position - vector * target.HitboxRadius, Core.Me.Position + vector * (target.HitboxRadius + length + Core.Me.HitboxRadius), width + 2f * item.Value.HitboxRadius);
            if (rectShape.IsInShape(item.Value.Position))
            {
                num++;
            }
        }

        return num;
    }
    public static bool IsBoss(IBattleChara target)
    {
        if (target.IsDummy())
        {
            return true;
        }

        if (Core.Resolve<MemApiTarget>().IsBoss(target))
        {
            return true;
        }

        return false;
    }
    
    // 返回>=0表示检测通过 即将调用Build方法
    public int Check_Before_Use()
    {   //使用TargetMgr.Instance.EnemysIn25.Values获取范围25m内的所有敌人，利用GetEnemyCountInsideRect方法对技能能够打到的敌人做倒序排序，取第一个则为最多的
        target = (from r in TargetMgr.Instance.EnemysIn25.Values 
                  orderby GetEnemyCountInsideRect(Core.Me, r, 25.0f, 4.0f) descending
                  select r).FirstOrDefault();

        //检查开关
        if (!SageRotationEntry.QT.GetQt(QTKey.魂灵风息))
            return -2;
        //检查CD
        Spell spell_魂灵风息 = new Spell(SageSpell.魂灵风息, target);
        Spell spell_寄生清汁 = new Spell(SageSpell.寄生清汁, Core.Me);
        if (!SpellExtension.IsReadyWithCanCast(spell_魂灵风息))
            return -3;
        if (IsBoss(Core.Me.GetCurrTarget()) && SpellExtension.IsReadyWithCanCast(spell_寄生清汁))
            return -1;
        if (SageSpell.寄生清汁.RecentlyUsed(2000) || SageSpell.整体论.RecentlyUsed(2000))
            return -6;
        if (!Core.Me.GetCurrTarget().HasAura(SageData.EukrasianDosis) &&
            !Core.Me.GetCurrTarget().HasAura(SageData.EukrasianDosisIi) &&
            !Core.Me.GetCurrTarget().HasAura(SageData.EukrasianDosisIii) &&
            !Core.Me.GetCurrTarget().HasAura(SageData.EukrasianDyskrasia))
            return -7;
        
        //检查满足条件人数
        var skillTarget = PartyHelper.CastableAlliesWithin20.Count(r =>
            r.CurrentHp > 0 && GameObjectExtension.CurrentHpPercent(r) <= SageSettings.Instance.群抬血阈值);
        var aoeCount = TargetHelper.GetNearbyEnemyCount(8);
        if (skillTarget < SageSettings.Instance.群奶人数阈值 && GetEnemyCountInsideRect(Core.Me, target, 25.0f, 4.0f) < 2)
            return -4;
        if (Core.Me.IsMoving())
            return -5;

        return 0;
   
    }
    public int Check()
    {
        var result = Check_Before_Use();
        if (result < 0) return result;
        if (SageBattleData.Healing_CD.TryGetValue("LastPartyHeal", out var lastUseTime_0))
        {
            if (Helpers.GetTimeStamps() - lastUseTime_0 < 1000)
                return -11;
        }

        return result;
    }

    // 将指定技能加入技能队列中
    public void Build(Slot slot)
    {
        if (Helpers.二十米视线内血量低于设定的队员数量(0.6f) >= SageSettings.Instance.群奶人数阈值 && SageSpell.活化.IsReady())
        {
            slot.Add(new Spell(SageSpell.活化, Core.Me)); 
        }
        slot.Add(new Spell(SageSpell.魂灵风息, target));

        if (SageBattleData.Healing_CD.ContainsKey("LastPartyHeal"))
        {
            SageBattleData.Healing_CD["LastPartyHeal"] = Helpers.GetTimeStamps();
        }
        else
        {
            SageBattleData.Healing_CD.TryAdd("LastPartyHeal", Helpers.GetTimeStamps());
        }
    }
}