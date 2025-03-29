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

public class SlotResolver_OffGCD_寄生清汁 : ISlotResolver
{

    // 返回>=0表示检测通过 即将调用Build方法
    public int Check_Before_Use()
    {
        //检查开关
        if (!SageRotationEntry.QT.GetQt(QTKey.寄生清汁) || !SageRotationEntry.QT.GetQt(QTKey.能力技治疗))
            return -10;
        //检查CD
        if(!SageSpell.寄生清汁.IsReady())
            return -9;
        if (Core.Resolve<JobApi_Sage>().Addersgall <= 0)
            return -4;
        if (SageSpell.魂灵风息.RecentlyUsed(3000)|| SageSpell.整体论.RecentlyUsed(3000))
            return -2;
        if (Helpers.二十五米视线内血量低于设定的队员数量(SageSettings.Instance.寄生清汁抬血阈值) >= SageSettings.Instance.群奶人数阈值)
        {
            return 2;
        }
        
        return -1;
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
        if (SageBattleData.Healing_CD.TryGetValue("LastHotHeal", out var lastUseTime_1))
        {
            if (Helpers.GetTimeStamps() - lastUseTime_1 < 1000)
                return -11;
        }
        return result;
    }

    // 将指定技能加入技能队列中
    public void Build(Slot slot)
    {
        slot.Add(SageSpell.寄生清汁.GetSpell());

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

