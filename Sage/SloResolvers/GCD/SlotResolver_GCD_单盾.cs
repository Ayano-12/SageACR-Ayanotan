using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist;
using Dalamud.Game.ClientState.Objects.Types;
using Ayanotan.WhiteMage.Setting;
using AEAssist.Extension;
using Ayanotan.WhiteMage.Data;
using Dalamud.Plugin.Services;
using System.Diagnostics;
using Ayanotan.Sage.Data;
using System.Runtime.Intrinsics.X86;
using Ayanotan.Sage;
using AEAssist.MemoryApi;
using AEAssist.ACT;


namespace Ayanotan.Sage.SloResolvers.GCD;

public class SlotResolver_GCD_单盾 : ISlotResolver

{
    private IBattleChara skillTarget;

    private IBattleChara target;

    private IBattleChara Ttarget;
    public static bool 队员是否拥有BUFF(uint buff)
    {
        return PartyHelper.CastableAlliesWithin30.Any(agent => agent.HasAura(buff));
    }
    public static bool 目标战斗状态(IBattleChara target)
    {
        return target.InCombat();
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
    {
        //检查QT状态

        if (!SageRotationEntry.QT.GetQt(QTKey.单盾))
            return -10;
       
        if (!SageSpell.均衡.IsUnlock())
            return -6;
        var aoeCount = TargetHelper.GetNearbyEnemyCount(10);
        float r = Core.Me.GetCurrTarget().CurrentHpPercent();
        //if (aoeCount==1 && IsBoss(Core.Me.GetCurrTarget())&& r <= 0.04f)
        //    return 0;
        //if (aoeCount <= 2 && !IsBoss(Core.Me.GetCurrTarget()) && r <= 0.18f)
        //    return 0;
        Ttarget = (from f in PartyHelper.CastableTanks
                   where (f.CurrentHp != 0)
                   orderby GameObjectExtension.CurrentHpPercent(f)
                   select f).FirstOrDefault();
        //判断血线，是否需要施放
        target = (from q in PartyHelper.CastableAlliesWithin30
                  where (q.CurrentHp != 0 && GameObjectExtension.CurrentHpPercent(q) <= SageSettings.Instance.单盾阈值 && !q.HasAnyAura(SageData.不能奶的无敌) && !q.HasAnyAura(SageData.暂不需要奶的无敌, 4000))
                  orderby GameObjectExtension.CurrentHpPercent(q)
                  select q).FirstOrDefault(); 
        Spell spell_白牛 = new Spell(SageSpell.白牛清汁, target);
        Spell spell_灵橡 = new Spell(SageSpell.灵橡清汁, target);
        if (SpellExtension.IsReadyWithCanCast(spell_白牛))
            return -1;
        if ((SpellExtension.IsReadyWithCanCast(spell_灵橡) && Core.Resolve<JobApi_Sage>().Addersgall > 1))
            return -2;
        if (SageSpell.白牛清汁.RecentlyUsed(1000) || SageSpell.灵橡清汁.RecentlyUsed(1000))
            return -4;
        if (Core.Me.Level >= 50 && (SageSpell.均衡预后.RecentlyUsed(5000) || SageSpell.均衡预后II.RecentlyUsed(5000)))
            return -8;
        if (target == null)    
            return -7;
        //不连打单盾，50级以下允许连打
        if ((Core.Me.Level >= 50 && SageSpell.均衡诊断.RecentlyUsed(3000)) && Core.Me.Level >= 50)
            return -3;
        if (Core.Me.Level >= 45 && Core.Resolve<JobApi_Sage>().Addersgall > 1)
            return -6;
        if (Core.Me.HasAnyAura(SageData.Henshin))
            return -5;
        return 1;
    }

    public int Check()
    {
        var result = Check_Before_Use();
        if (result < 0) return result;

        if (target != null)
        {
            if (SageBattleData.Healing_CD.TryGetValue("LastPartyHeal", out var lastUseTime_0))
            {
                if (Helpers.GetTimeStamps() - lastUseTime_0 < 1000)
                    return -11;
            }
        }

        return result;
    }


    public void Build(Slot slot)
    {
        if (!Core.Resolve<JobApi_Sage>().Eukrasia)
        {
            slot.Add(SageSpell.均衡.GetSpell());
        }
        slot.Add(new Spell(SageSpell.均衡诊断, target));
        if (target != null)
        {
            if (SageBattleData.Healing_CD.ContainsKey("LastSingleHeal" + target.GameObjectId))
            {
                SageBattleData.Healing_CD["LastSingleHeal" + target.GameObjectId] = Helpers.GetTimeStamps();
            }
            else
            {
                SageBattleData.Healing_CD.TryAdd("LastSingleHeal" + target.GameObjectId, Helpers.GetTimeStamps());
            }
        }
    }
 

}


