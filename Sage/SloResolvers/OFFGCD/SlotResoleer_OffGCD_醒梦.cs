using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;


namespace Ayanotan.Sage.SloResolvers.OFFGCD;

public class SlotResoleer_OffGCD_醒梦 : ISlotResolver
{

    public int Check()
    {

        //检查CD
        if (!SpellsDefine.LucidDreaming.IsReady())
            return -1;
        //检查自身蓝量
        if (Core.Me.CurrentMp > 8000)
            return -2;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(SpellsDefine.LucidDreaming.GetSpell());
    }

}
