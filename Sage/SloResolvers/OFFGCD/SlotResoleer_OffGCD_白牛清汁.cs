using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Ayanotan.Sage.Data;
using Dalamud.Game.ClientState.Objects.Types;
using Ayanotan.WhiteMage.Data;
using Ayanotan.WhiteMage.Setting;
using Ayanotan.Sage;


namespace Ayanotan.Sage.SloResolvers.OFFGCD;

public class SlotResoleer_OffGCD_白牛清汁 : ISlotResolver
{
    private IBattleChara target;
    
    public int Check_Before_Use()
    {
       
        target = (from r in PartyHelper.CastableAlliesWithin30
                  where r.CurrentHp != 0 && GameObjectExtension.CurrentHpPercent(r) <= SageSettings.Instance.单抬能力技抬血阈值 && !r.HasAnyAura(SageData.不能奶的无敌) && !r.HasAnyAura(SageData.暂不需要奶的无敌, 4000)
                  orderby GameObjectExtension.CurrentHpPercent(r)
                  select r).FirstOrDefault();
        Spell spell_白牛清汁 = new Spell(SageSpell.白牛清汁, target);
        if (!SpellExtension.IsReadyWithCanCast(spell_白牛清汁))
            return -1;
        if (SageRotationEntry.QT.GetQt(QTKey.白牛清汁) == false)
            return -3;
        if (SageRotationEntry.QT.GetQt(QTKey.能力技治疗) == false)
            return -4;
        if (SageSpell.输血.RecentlyUsed(3000))
            return -5;
        if (Core.Resolve<JobApi_Sage>().Addersgall < 1)
            return -4;
        if (Helpers.二十五米视线内血量低于设定的队员数量(SageSettings.Instance.寄生清汁抬血阈值) >= 2 && SageSpell.寄生清汁.GetSpell().IsReadyWithCanCast())
            return -7;
        if (SageSpell.寄生清汁.RecentlyUsed(1000))
            return -6;

        //检查是否有满足条件的目标
        if (target == null)
            return -2;
      
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
            if (SageBattleData.Healing_CD.TryGetValue("LastSingleHeal" + target.GameObjectId, out var lastUseTime_1))
            {
                if (Helpers.GetTimeStamps() - lastUseTime_1 < 500)
                    return -12;
            }
        }

        return result;
    }

    public void Build(Slot slot)
    {
        slot.Add(new Spell(SageSpell.白牛清汁, target));

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
