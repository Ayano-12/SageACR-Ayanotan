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
using Dalamud.Game.ClientState.Objects.Types;

namespace Ayanotan.Sage.SloResolvers.GCD;

public class SlotResolver_GCD_诊断 : ISlotResolver
{
    private IBattleChara target;
    // 返回>=0表示检测通过 即将调用Build方法
    public int Check_Before_Use()
    {

        //检查开关
        if (!SageRotationEntry.QT.GetQt(QTKey.非均衡治疗))
            return -2;
        //检查CD
        if (SageSpell.均衡.IsReady() || SageSpell.均衡注药.IsReady() || SageSpell.均衡注药II.IsReady() || SageSpell.均衡注药III.IsReady() || SageSpell.均衡失衡.IsReady())
            return -3;
        //检查满足条件人数
        var skillTarget = PartyHelper.CastableAlliesWithin15.Count(r =>
            r.CurrentHp > 0 && GameObjectExtension.CurrentHpPercent(r) <= SageSettings.Instance.群GCD抬血阈值);
        target = (from r in PartyHelper.CastableAlliesWithin30
                  where r.CurrentHp != 0 && GameObjectExtension.CurrentHpPercent(r) <= SageSettings.Instance.单GCD抬血阈值
                  orderby GameObjectExtension.CurrentHpPercent(r)
                  select r).FirstOrDefault();
        if (skillTarget >= SageSettings.Instance.群奶人数阈值)
           return -4;
        if (target == null)
            return -6;
        if ((Core.Me.IsMoving()))
            return -5;
        return 0;
    }
    public int Check()
    {
        var result = Check_Before_Use();
        if (result < 0) return result;

        if (target != null)
        {
            if (SageBattleData.Healing_CD.TryGetValue("LastPartyHeal", out var lastUseTime_0))
            {
                if (Helpers.GetTimeStamps() - lastUseTime_0 < 1000)
                    return -11;
            }
        }

        return result;
    }

    // 将指定技能加入技能队列中
    public void Build(Slot slot)
    {
        slot.Add(new Spell(SageSpell.诊断, target));

        if (target != null)
        {
            if (SageBattleData.Healing_CD.ContainsKey("LastSingleHeal" + target.GameObjectId))
            {
                SageBattleData.Healing_CD["LastSingleHeal" + target.GameObjectId] = Helpers.GetTimeStamps();
            }
            else
            {
                SageBattleData.Healing_CD.TryAdd("LastSingleHeal" + target.GameObjectId, Helpers.GetTimeStamps());
            }
        }
    }
}