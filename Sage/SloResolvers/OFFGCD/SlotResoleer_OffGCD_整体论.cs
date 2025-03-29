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
using Ayanotan.WhiteMage.Setting;
using Dalamud.Game.ClientState.Objects.Types;


namespace Ayanotan.Sage.SloResolvers.OFFGCD;

public class SlotResoleer_OffGCD_整体论 : ISlotResolver
{
    Spell spell = new Spell(SageSpell.坚角清汁, SpellTargetType.Self);
    public static bool IsBoss(IBattleChara target)
    {
        if (target.IsDummy())
        {
            return true;
        }

        if (Core.Resolve<MemApiTarget>().IsBoss(target))
        {
            return true;
        }

        return false;
    }
    public int Check_Before_Use()
    {

        if (!SageRotationEntry.QT.GetQt(QTKey.整体论) || !SageRotationEntry.QT.GetQt(QTKey.能力技治疗))
            return -2;

        //检查CD
        if (!SageSpell.整体论.IsReady())
            return -1;

        if (!IsBoss(Core.Me.GetCurrTarget()))
            return -4;
        if (SageSpell.寄生清汁.RecentlyUsed(3000) || SageSpell.魂灵风息.RecentlyUsed(3000) || SageSpell.坚角清汁.RecentlyUsed(6000))
            return -3;
        if (TargetHelper.targetCastingIsBossAOE(Core.Me.GetCurrTarget(), 5000))
            
        {
            return 1;
        }

        //之前奶过了或是身上有团减的情况下不开
        

        var skillTarget = PartyHelper.CastableAlliesWithin20.Count(r =>
       r.CurrentHp > 0 && GameObjectExtension.CurrentHpPercent(r) <= SageSettings.Instance.整体论抬血阈值);
        if (skillTarget == 0)
        {
            return -1;
        }
        //没有寄生清汁且要奶血的时候用整体论
        if (skillTarget >= SageSettings.Instance.群奶人数阈值 && !SageSpell.寄生清汁.IsReady())
            return 0;

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
        if (SageBattleData.Healing_CD.ContainsKey("LastPartyHeal"))
        {
            SageBattleData.Healing_CD["LastPartyHeal"] = Helpers.GetTimeStamps();
        }
        else
        {
            SageBattleData.Healing_CD.TryAdd("LastPartyHeal", Helpers.GetTimeStamps());
        }
        return result;
    }
    public void Build(Slot slot)
    {
        slot.Add(SageSpell.整体论.GetSpell());
    }

}
