using AEAssist.CombatRoutine;
using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using Ayanotan.WhiteMage.Data;
using Ayanotan.Sage.Data;
using AEAssist.JobApi;
using Ayanotan.Sage;
using AEAssist.CombatRoutine.Module.Target;
using AEAssist.Define;
using Dalamud.Game.ClientState.Objects.Types;
using System.Numerics;
using Ayanotan.WhiteMage.Setting;

namespace Ayanotan.Sage.SloResolvers.GCD;

public class SlotResolver_GCD_AOE : ISlotResolver
{
    private IBattleChara target_for发炎;
    private IBattleChara target_for箭毒;
    Spell spell_失衡 = new Spell(SageSpell.失衡, SpellTargetType.Self);
    Spell spell_失衡II = new Spell(SageSpell.失衡II, SpellTargetType.Self);
    Spell spell_均衡注药 = new Spell(SageSpell.均衡注药, Core.Me.GetCurrTarget());
    Spell spell_均衡失衡 = new Spell(SageSpell.均衡失衡, SpellTargetType.Self);


    //
    // 摘要:
    //     获取某个单位附近敌人的数量
    //
    // 参数:
    //   target:
    //     以哪个单位为圆心
    //
    //   spellCastRange:
    //     施法距离，如果target离自己超出这个距离则返回0
    //
    //   damageRange:
    //     伤害范围
    //public static int GetNearbyEnemyCount(IBattleChara target, int spellCastRange, int damageRange)
    //{
    //    return 0;
    //}

    // 返回>=0表示检测通过 即将调用Build方法
    public int Check()
    {
        //发炎用 选择最优化目标（距离小于等于6.5）倒序排列
        target_for发炎 = (from r in TargetMgr.Instance.Enemys.Values
                        where r.DistanceToPlayer() <= 6.5
                        orderby TargetHelper.GetNearbyEnemyCount(r, 6, 5) descending
                        select r).FirstOrDefault();
        //箭毒用 选择最优化目标（距离小于等于25）倒序排列
        target_for箭毒 = (from r in TargetMgr.Instance.EnemysIn25.Values
                        where r.DistanceToPlayer() <= 25
                        orderby TargetHelper.GetNearbyEnemyCount(r, 25, 5) descending
                        select r).FirstOrDefault();
        int 保留蛇刺数量_forReal;
        if (SageSettings.Instance.保留蛇刺数量 > 3)
        {
            保留蛇刺数量_forReal = 3;
        }
        else if (SageSettings.Instance.保留蛇刺数量 < 0)
        {
            保留蛇刺数量_forReal = 0;
        }
        else
        {
            保留蛇刺数量_forReal = SageSettings.Instance.保留蛇刺数量;
        }
        var aoeCount_check = TargetHelper.GetNearbyEnemyCount(5);
        var aoeCount_check_6 = TargetHelper.GetNearbyEnemyCount(6);
        var aoeCount_check_20 = TargetHelper.GetNearbyEnemyCount(20);
        int aoeCount_for箭毒 = TargetHelper.GetNearbyEnemyCount(GameObjectExtension.GetCurrTarget((IBattleChara)Core.Me), 25, 5);
        if (target_for箭毒 == null) //解决NullReferenceException问题
        {
            if (aoeCount_check < 2)
                return -2;

        }
        else//附近的敌人小于2个不打AOE，或是箭毒解锁且有充能的情况下打不到两个目标
        {
            if (aoeCount_check < 2 && aoeCount_for箭毒 < 2)
                return -3;
        }
        if (!SageRotationEntry.QT.GetQt(QTKey.AOE))
            return -1;

        if (!Helpers.目标是否可见或在技能范围内(SageSpell.发炎) && !Helpers.目标是否可见或在技能范围内(SpellsDefineAlternative.Toxikon) && !Helpers.目标是否可见或在技能范围内(SageSpell.注药))
            return -7;
            //开了均衡准备要上DOT不打AOE
        if (SpellExtension.IsReadyWithCanCast(spell_均衡失衡) || SpellExtension.IsReadyWithCanCast(spell_均衡注药))
            return -4;
        //检测失衡是否解锁
        if (!SpellExtension.IsReadyWithCanCast(spell_失衡) && !SpellExtension.IsReadyWithCanCast(spell_失衡II))
            return -5;
        if (Core.Me.IsMoving() && !(SageRotationEntry.QT.GetQt(QTKey.发炎) && aoeCount_check_6 >= 3 && (SageSpell.发炎III.IsReady() || SageSpell.发炎II.IsReady() || SageSpell.发炎.IsReady()) || (aoeCount_check_6 < 3 && Core.Resolve<MemApiSpell>().CheckActionChange(SageSpell.发炎).GetSpell().Charges > 1.9) && target_for发炎 != null && target_for发炎.DistanceToPlayer() <= 6.5) && !(SpellExtension.IsUnlock(SageSpell.箭毒) && Core.Resolve<JobApi_Sage>().Addersting > 保留蛇刺数量_forReal && TargetHelper.GetNearbyEnemyCount(target_for箭毒, 25, 5) >= aoeCount_check)&& !(Core.Me.Level >= 46 && aoeCount_check >= 2))
            return -6;
        if (Helpers.TargetHasAura(Core.Me.GetCurrTarget(), SageData.Enemyinvulnerability))
            return -2;
        //DOT还有3秒以上的时候可以打AOE
        if (Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(SageData.EukrasianDosis, 3000) ||
            Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(SageData.EukrasianDosisIi, 3000) ||
            Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(SageData.EukrasianDosisIii, 3000) ||
            Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(SageData.EukrasianDyskrasia, 3000))
            return 0;


        return 0;
    }

    // 将指定技能加入技能队列中
    public void Build(Slot slot)
    {
        if (SageRotationEntry.QT.GetQt(QTKey.AOE))
        {
            int 保留蛇刺数量_forReal;
            if (SageSettings.Instance.保留蛇刺数量 > 3) 
            {
                保留蛇刺数量_forReal = 3; 
            }
            else if (SageSettings.Instance.保留蛇刺数量 < 0)
            {
                保留蛇刺数量_forReal = 0;
            }
            else
            {
                保留蛇刺数量_forReal = SageSettings.Instance.保留蛇刺数量;
            }
           
            //此处的aoeCount是用于下一行检测是施放发炎还是箭毒的
            var aoeCount_build_失衡 = TargetHelper.GetNearbyEnemyCount(5);
            var aoeCount_build_发炎 = TargetHelper.GetNearbyEnemyCount(6);
            //如果人数大于等于三直接打发炎，如果少于三但发炎充能满了也打出去防止空转
            if (SageRotationEntry.QT.GetQt(QTKey.发炎) && aoeCount_build_发炎 >= 3 && (SageSpell.发炎III.IsReady()|| SageSpell.发炎II.IsReady()||SageSpell.发炎.IsReady()) || (aoeCount_build_发炎 < 3 && Core.Resolve<MemApiSpell>().CheckActionChange(SageSpell.发炎).GetSpell().Charges >1.9) && target_for发炎 != null && target_for发炎.DistanceToPlayer() <= 6.5 )
            {
                slot.Add(new Spell(SageSpell.发炎, target_for发炎));
            }
            //打箭毒
            else if (SpellExtension.IsUnlock(SageSpell.箭毒) && Core.Resolve<JobApi_Sage>().Addersting > 保留蛇刺数量_forReal && TargetHelper.GetNearbyEnemyCount(target_for箭毒, 25, 5) >= aoeCount_build_失衡)
                slot.Add(new Spell(SageSpell.箭毒, target_for箭毒));
            //检查失衡是否解锁以及失衡覆盖人数
            else if((Core.Me.Level >= 46 && Core.Me.Level < 82) && aoeCount_build_失衡 >= 2)
                slot.Add(SpellsDefineAlternative.Dyskrasia.GetSpell());
            else if (Core.Me.Level >= 82  && aoeCount_build_失衡 >= 3)
                slot.Add(SpellsDefineAlternative.Dyskrasia.GetSpell());
            //没解锁就打注药
            else slot.Add(SageSpell.注药.GetSpell());
        }
        
    }
}
