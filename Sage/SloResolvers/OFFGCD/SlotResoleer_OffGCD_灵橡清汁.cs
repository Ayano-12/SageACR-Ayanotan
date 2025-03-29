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

public class SlotResoleer_OffGCD_灵橡清汁 : ISlotResolver
{
    private IBattleChara skillTarget;

    private IBattleChara target;
    public int Check_Before_Use()
    {

        if (!SageSpell.灵橡清汁.IsReady())
            return -1;
        if (SageRotationEntry.QT.GetQt(QTKey.灵橡清汁) == false)
            return -2;
        if (SageRotationEntry.QT.GetQt(QTKey.能力技治疗) == false)
            return -3;
        //if (Core.Resolve<JobApi_Sage>().Addersgall <=1)
        //    return -4;
        if (SageSpell.白牛清汁.IsReady())
            return -5;
        if (SageSpell.白牛清汁.RecentlyUsed(1000))
            return -6;
        if (SageSpell.寄生清汁.RecentlyUsed(1000))
            return -7;
        if (SageSpell.灵橡清汁.RecentlyUsed(1000))
            return -8;
        //if (WhiteMageSettings.Instance.OnlyTank)
        //{
        //    skillTarget = PartyHelper.CastableAlliesWithin30
        //        .Where(r => r.CurrentHp > 0 && GameObjectExtension.CurrentHpPercent(r) <= WhiteMageSettings.Instance.TetragrammatonPP && r.IsTank() && !r.NotInvulnerable())
        //        .OrderBy(r => r.CurrentHpPercent())
        //        .FirstOrDefault();
        //}
        //else
        //{
        //    skillTarget = PartyHelper.CastableAlliesWithin30
        //        .Where(r => r.CurrentHp > 0 && GameObjectExtension.CurrentHpPercent(r) <= WhiteMageSettings.Instance.TetragrammatonPP && !r.NotInvulnerable())
        //        .OrderBy(r => r.CurrentHpPercent())
        //        .FirstOrDefault();
        //}
        target = (from r in PartyHelper.CastableAlliesWithin30
                  where r.CurrentHp != 0 && GameObjectExtension.CurrentHpPercent(r) <= SageSettings.Instance.单抬能力技抬血阈值 && !r.HasAnyAura(SageData.不能奶的无敌) && !r.HasAnyAura(SageData.暂不需要奶的无敌, 4000)
                  orderby GameObjectExtension.CurrentHpPercent(r)
                  select r).FirstOrDefault();
        var skillTarget = PartyHelper.CastableAlliesWithin15.Count(r =>
           r.CurrentHp > 0 && GameObjectExtension.CurrentHpPercent(r) <= SageSettings.Instance.群抬血阈值);
        if (target == null || skillTarget >= SageSettings.Instance.群奶人数阈值)
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
                if (Helpers.GetTimeStamps() - lastUseTime_1 < 1000)
                    return -12;
            }
        }

        return result;
    }

    public void Build(Slot slot)
    {
        
        slot.Add(new Spell(SageSpell.灵橡清汁, target));

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
