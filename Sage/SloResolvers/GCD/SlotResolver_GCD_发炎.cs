using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AEAssist.Helper;
using AEAssist;
using AEAssist.Extension;
using AEAssist.JobApi;
using Dalamud.Game.ClientState.JobGauge.Types;
using Ayanotan.WhiteMage.Data;
using Ayanotan.Sage.Data;
using Ayanotan.Sage;
using AEAssist.MemoryApi;
using Dalamud.Game.ClientState.Objects.Types;
using AEAssist.CombatRoutine.Module.Target;
using AEAssist.Define;
using System.Numerics;

namespace Ayanotan.Sage.SloResolvers.GCD;

public class SlotResolver_GCD_发炎 : ISlotResolver
{
    private IBattleChara target;
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
    public int Check()
    {
        Spell spell_均衡注药 = new Spell(SageSpell.均衡注药, Core.Me.GetCurrTarget());
        Spell spell_均衡失衡 = new Spell(SageSpell.均衡失衡, SpellTargetType.Self);
        if (SpellExtension.IsReadyWithCanCast(spell_均衡失衡) || SpellExtension.IsReadyWithCanCast(spell_均衡注药))
            return -1;
        if (!SageRotationEntry.QT.GetQt(QTKey.发炎))
            return -3;
        target = (from r in TargetMgr.Instance.Enemys.Values
                  where r.DistanceToPlayer() <= 6.5
                  orderby TargetHelper.GetNearbyEnemyCount(r, 6, 5) descending
                  select r).FirstOrDefault();
        Spell spell_发炎 = new Spell(SageSpell.发炎, target);
        if (!SpellExtension.IsReadyWithCanCast(spell_发炎))
            return -4;
        if (Helpers.TargetHasAura(Core.Me.GetCurrTarget(), SageData.Enemyinvulnerability))
            return -2;
        var aoeCount = TargetHelper.GetNearbyEnemyCount(6);
        if (aoeCount > 3)
            return 2;
        if (aoeCount <= 3 && Core.Resolve<MemApiSpell>().CheckActionChange(SageSpell.发炎).GetSpell().Charges >= 2 && Helpers.目标是否可见或在技能范围内(SageSpell.发炎))
            return 1;
       
        if (aoeCount <= 3 &&  Core.Resolve<MemApiSpell>().CheckActionChange(SageSpell.发炎).GetSpell().Charges < 2)
            return -2;



        return 0;


    }

    // 将指定技能加入技能队列中
    public void Build(Slot slot)
    {
       
            slot.Add(new Spell(SageSpell.发炎, target));
    }
}
