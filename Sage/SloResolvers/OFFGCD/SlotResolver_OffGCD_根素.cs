using AEAssist.CombatRoutine;
using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using AEAssist.JobApi;
using Ayanotan.WhiteMage.Setting;
using Ayanotan.WhiteMage.Data;
using Ayanotan.Sage.Data;
using Ayanotan.Sage;

namespace Ayanotan.Sage.SloResolvers.GCD;

public class SlotResolver_OffGCD_根素 : ISlotResolver
{

    // 返回>=0表示检测通过 即将调用Build方法
    public int Check()
    {
        
        
        //检查CD
        if(!SageSpell.根素.IsReady())
            return -9;
        if (Core.Resolve<JobApi_Sage>().Addersgall > 1)
            return -4;
        return 0;
       
    }

    // 将指定技能加入技能队列中
    public void Build(Slot slot)
    {
        slot.Add(SageSpell.根素.GetSpell());
    }
}

