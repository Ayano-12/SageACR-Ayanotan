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
using Ayanotan.WhiteMage.Data;
using Ayanotan.Sage.Data;
using static Dalamud.Interface.Utility.Raii.ImRaii;
using Dalamud.Game.ClientState.Objects.Types;
using System.Runtime.Intrinsics.X86;
using AEAssist.JobApi;
using Ayanotan.Sage;
using AEAssist.MemoryApi;

namespace Ayanotan.Sage.SloResolvers.GCD;

public class SlotResolver_GCD_Dot : ISlotResolver
{
    Spell spell_均衡注药 = new Spell(SageSpell.均衡注药, Core.Me.GetCurrTarget());
    Spell spell_均衡注药II = new Spell(SageSpell.均衡注药II, Core.Me.GetCurrTarget());
    Spell spell_均衡注药III = new Spell(SageSpell.均衡注药III, Core.Me.GetCurrTarget());
    Spell spell_均衡失衡 = new Spell(SageSpell.均衡失衡, SpellTargetType.Self);
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
        if (!SageRotationEntry.QT.GetQt(QTKey.Dot))
            return -5;
        var aoeCount = TargetHelper.GetNearbyEnemyCount(5);
        //检测是否将要过期以及是否有均衡状态
        if (Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(SageData.EukrasianDosis, 3000)    ||
            Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(SageData.EukrasianDosisIi, 3000)  ||
            Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(SageData.EukrasianDosisIii, 3000) ||
            Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(SageData.EukrasianDyskrasia, 3000)  )
            return -1;
        //如果当前目标不是BOSS且均衡失衡未解锁则不上DOT
        if (!IsBoss(Core.Me.GetCurrTarget()) && !SpellExtension.IsUnlock(SageSpell.均衡失衡))
            return -8;
        //如果当前目标不是BOSS且当前目标血量不到30%则不上DOT
        if (!IsBoss(Core.Me.GetCurrTarget()) && Core.Me.GetCurrTarget().CurrentHpPercent() <= 0.3f)
            return -3;
        //if (!SpellExtension.IsReadyWithCanCast(spell_均衡注药) && !SpellExtension.IsReadyWithCanCast(spell_均衡注药II) && !SpellExtension.IsReadyWithCanCast(spell_均衡注药III))
        //    return -2;
        //如果当前目标不是BOSS且附近小怪数量小于等于2个则不上DOT
        if (!IsBoss(Core.Me.GetCurrTarget()) && aoeCount<=2)
            return -4;
        //检测是否在黑名单中
        if (DotBlacklistHelper.IsBlackList(Core.Me.GetCurrTarget()))
            return -10;
        if (Helpers.TargetHasAura(Core.Me.GetCurrTarget(),SageData.Enemyinvulnerability))
            return -2;
        //检测均衡是否解锁
        if (!SageSpell.均衡.IsUnlock())
            return -6;
        //不连续上DOT
        if (SageSpell.均衡失衡.RecentlyUsed(5000))
            return -9;


        return 1;
    }

   

    public void Build(Slot slot)
    {
       
        //slot.Add(SageSpell.均衡.GetSpell());

        var aoeCount = TargetHelper.GetNearbyEnemyCount(5);
        //if (aoeCount>2 && SpellExtension.IsReadyWithCanCast(spell_均衡失衡))
        //    slot.Add(SageSpell.均衡失衡.GetSpell());
        //else if(SpellExtension.IsReadyWithCanCast(spell_均衡注药III)) 
        //    slot.Add(SageSpell.均衡注药III.GetSpell());
        //else if (SpellExtension.IsReadyWithCanCast(spell_均衡注药II))
        //    slot.Add(SageSpell.均衡注药II.GetSpell());
        //else slot.Add(SageSpell.均衡注药.GetSpell());
        if (!Core.Resolve<JobApi_Sage>().Eukrasia)
        {
            slot.Add(SageSpell.均衡.GetSpell());
        }
        else
        {
            if (aoeCount > 2 && SpellExtension.IsReadyWithCanCast(spell_均衡失衡))
                slot.Add(SageSpell.均衡失衡.GetSpell());
            else if (Core.Me.Level >= 82)
                slot.Add(SageSpell.均衡注药III.GetSpell());
            else if (Core.Me.Level >= 72)
                slot.Add(SageSpell.均衡注药II.GetSpell());
            else slot.Add(SageSpell.均衡注药.GetSpell());
        }
    }
}

