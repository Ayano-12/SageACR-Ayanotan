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

public class SlotResolver_GCD_Dot均衡 : ISlotResolver
{
    Spell spell_均衡 = new Spell(SageSpell.均衡, SpellTargetType.Self);
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
        var aoeCount = TargetHelper.GetNearbyEnemyCount(5);
        //检测是否将要过期
        if (Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(SageData.EukrasianDosis, 3000) ||
            Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(SageData.EukrasianDosisIi, 3000) ||
            Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(SageData.EukrasianDosisIii, 3000) ||
            Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(SageData.EukrasianDyskrasia, 3000))
            return -1;
        if (!IsBoss(Core.Me.GetCurrTarget()) && !SpellExtension.IsUnlock(SageSpell.均衡失衡))
            return -8;
        if (!IsBoss(Core.Me.GetCurrTarget()) && Core.Me.GetCurrTarget().CurrentHpPercent() <= 0.3f)
            return -3;
        if (!IsBoss(Core.Me.GetCurrTarget()) && aoeCount <= 2)
            return -7;
        //检测是否在黑名单中
        if (DotBlacklistHelper.IsBlackList(Core.Me.GetCurrTarget()))
            return -10;
        if (!SageRotationEntry.QT.GetQt(QTKey.Dot))
            return -3;
        if (!SpellExtension.IsUnlock(SageSpell.均衡))
            return -6;
        if (Core.Me.HasAura(2606))
            return -4;
        if (!SpellExtension.IsReadyWithCanCast(spell_均衡))
            return -5;
        if (SageSpell.均衡失衡.RecentlyUsed(5000))
            return -9;

        return 1;
    }

   

    public void Build(Slot slot)
    {
       
        slot.Add(SageSpell.均衡.GetSpell());

        //var aoeCount = TargetHelper.GetNearbyEnemyCount(5);
        //if (aoeCount>2)
        //    slot.Add(SageSpell.均衡失衡.GetSpell());
        //else slot.Add(SageSpell.均衡注药.GetSpell());


    }
}

