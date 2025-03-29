using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Ayanotan.Sage.Data;
using Ayanotan.WhiteMage.Data;
using Ayanotan.WhiteMage.Setting;
using Ayanotan.Sage;


namespace Ayanotan.Sage.SloResolvers.OFFGCD;

public class SlotResoleer_OffGCD_智慧之爱 : ISlotResolver
{

    public int Check_Before_Use()
    {
        //检查CD
        if (!SageSpell.智慧之爱.IsReady())
            return -1;
        //检查开关
        if (!SageRotationEntry.QT.GetQt(QTKey.智慧之爱))
            return -3;
        //检查开关
        if (!SageRotationEntry.QT.GetQt(QTKey.能力技治疗))
            return -10;
        if (SageSpell.寄生清汁.RecentlyUsed(3000) || SageSpell.魂灵风息.RecentlyUsed(3000) || SageSpell.整体论.RecentlyUsed(5000) || SageSpell.自生.RecentlyUsed(10000) || SageSpell.自生II.RecentlyUsed(10000))
            return -3;
        //检查满足条件人数
        var skillTarget = PartyHelper.CastableAlliesWithin15.Count(r =>
            r.CurrentHp > 0 && GameObjectExtension.CurrentHpPercent(r) <= SageSettings.Instance.群抬血阈值);
        if (skillTarget == 0)
            return -2; 
        if ( skillTarget >= SageSettings.Instance.群奶人数阈值)
            return 0;
        return -1;
    }
    public int Check()
    {
        var result = Check_Before_Use();
        if (result < 0) return result;

        if (SageBattleData.Healing_CD.ContainsKey("LastHotHeal"))
        {
            SageBattleData.Healing_CD["LastHotHeal"] = Helpers.GetTimeStamps();
        }
        else
        {
            SageBattleData.Healing_CD.TryAdd("LastHotHeal", Helpers.GetTimeStamps());
        }
        return result;
    }

    public void Build(Slot slot)
    {
        slot.Add(SageSpell.智慧之爱.GetSpell());
    }

}
