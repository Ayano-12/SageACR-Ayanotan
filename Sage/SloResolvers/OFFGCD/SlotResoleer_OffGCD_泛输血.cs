using AEAssist;
using AEAssist.ACT;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Ayanotan.Sage.Data;
using Ayanotan.WhiteMage.Data;
using Ayanotan.Sage;


namespace Ayanotan.Sage.SloResolvers.OFFGCD;

public class SlotResoleer_OffGCD_泛输血 : ISlotResolver
{

    public int Check()
    {

        if (!SageRotationEntry.QT.GetQt(QTKey.泛输血))
            return -2;
        if (SageSpell.整体论.RecentlyUsed(3000))
            return -4;
        //检查CD
        if (!SageSpell.泛输血.IsReady())
            return -1;

        if (Core.Me.GetCurrTarget().CurrentHpPercent()<=0.05f)
            return -3;

        //有整体论或坚角清汁的情况下不开
        if (Core.Me.HasAura(2618)|| Core.Me.HasAura(3003) || SageSpell.坚角清汁.RecentlyUsed(10000))
            return -5;
       
        if (TargetHelper.targetCastingIsBossAOE(Core.Me.GetCurrTarget(), 5000))
            
        {
            return 1;
        }


        return -1;

      
    }

    public void Build(Slot slot)
    {
        slot.Add(SageSpell.泛输血.GetSpell());
    }

}
