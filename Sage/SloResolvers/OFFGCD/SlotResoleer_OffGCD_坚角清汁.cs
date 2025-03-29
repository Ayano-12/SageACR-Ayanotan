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

public class SlotResoleer_OffGCD_坚角清汁 : ISlotResolver
{

    public int Check()
    {

        if (SageRotationEntry.QT.GetQt(QTKey.坚角清汁) == false)
            return -2;

        //检查CD
        if (!SageSpell.坚角清汁.IsReady())
            return -3;

        var aoeCount = TargetHelper.GetNearbyEnemyCount(5);
        if (aoeCount >= 2 && SageSpell.坚角清汁.IsReady())
            return 0;
        if (SageSpell.整体论.RecentlyUsed(10000))
            return -3;

        if (TargetHelper.targetCastingIsBossAOE(Core.Me.GetCurrTarget(), 5000)) 
            
        {
            return 1;
        }


        return -1;

      
    }

    public void Build(Slot slot)
    {
        slot.Add(SageSpell.坚角清汁.GetSpell());
    }

}
