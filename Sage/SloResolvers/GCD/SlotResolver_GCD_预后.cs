using AEAssist.CombatRoutine;
using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using Ayanotan.WhiteMage.Setting;
using Ayanotan.WhiteMage.Data;
using Ayanotan.Sage.Data;
using Ayanotan.Sage;

namespace Ayanotan.Sage.SloResolvers.GCD;

public class SlotResolver_GCD_预后 : ISlotResolver
{
    private Spell GetSpell()
    {
        return SageSpell.预后.GetSpell();
    }
    // 返回>=0表示检测通过 即将调用Build方法
    public int Check_Before_Use()
    {

        //检查开关
        if (!SageRotationEntry.QT.GetQt(QTKey.非均衡治疗))
            return -2;
        //检查CD
        var skillTarget = PartyHelper.CastableAlliesWithin15.Count(r =>
            r.CurrentHp > 0 && GameObjectExtension.CurrentHpPercent(r) <= SageSettings.Instance.群GCD抬血阈值);
        if (!SageSpell.寄生清汁.IsReady() && !SageSpell.整体论.IsReady() && !SageSpell.魂灵风息.IsReady() && skillTarget >= 2 && !Core.Me.IsMoving())
            return 1;
        if (SageSpell.均衡.IsReady())
            return -3;
        //检查满足条件人数
        
        if (skillTarget == 0)
            return -4;
     
        
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

        return result;
    }



    // 将指定技能加入技能队列中
    public void Build(Slot slot)
    {
        slot.Add(GetSpell());

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