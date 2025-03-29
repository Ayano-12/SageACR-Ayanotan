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
using Dalamud.Game.ClientState.Objects.Types;
using System.Runtime.Intrinsics.X86;
using Ayanotan.Sage;

namespace Ayanotan.Sage.SloResolvers.GCD;

public class SlotResolver_GCD_群盾 : ISlotResolver

{
    private IBattleChara skillTarget;

    private IBattleChara target;
    
    // 返回>=0表示检测通过 即将调用Build方法
    public int Check_Before_Use()
    {
        //检查开关
        if (!SageRotationEntry.QT.GetQt(QTKey.群盾))
            return -10;
        //有豆子且寄生清汁能用的情况下不用群盾
        if (Core.Resolve<JobApi_Sage>().Addersgall > 0 && SageSpell.寄生清汁.IsReady())
            return -4;
        var skillTarget = PartyHelper.CastableAlliesWithin15.Count(r =>
          r.CurrentHp > 0 && GameObjectExtension.CurrentHpPercent(r) <= SageSettings.Instance.群抬血阈值);
        if (!SageSpell.均衡.IsUnlock())
            return -6;
        if (SageSpell.均衡预后.RecentlyUsed(5000) || SageSpell.均衡预后II.RecentlyUsed(5000))
            return -3;
        if (SageSpell.白牛清汁.RecentlyUsed(1000) || SageSpell.灵橡清汁.RecentlyUsed(1000) || SageSpell.寄生清汁.RecentlyUsed(1000) || SageSpell.整体论.RecentlyUsed(1000))
            return -8;
        if (!SpellExtension.IsUnlock(SageSpell.均衡预后))
            return -7;
        if (!SageSpell.寄生清汁.IsReady() && skillTarget >= SageSettings.Instance.群奶人数阈值 && !SageSpell.魂灵风息.IsReady())
            return 1;
        if (skillTarget == 0)
            return -1;
        if (Core.Me.HasAnyAura(SageData.Henshin))
            return -5;
        if (skillTarget >= SageSettings.Instance.群奶人数阈值)
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

        return result;
    }

    // 将指定技能加入技能队列中
    public void Build(Slot slot)
    {
        if (!Core.Resolve<JobApi_Sage>().Eukrasia)
        {
            slot.Add(SageSpell.均衡.GetSpell());
        }
        else
        {
            if (Helpers.十五米视线内血量低于设定的队员数量(0.6f) >= SageSettings.Instance.群奶人数阈值 && SageSpell.活化.IsReady())
            { slot.Add(new Spell(SageSpell.活化, Core.Me)); }

            if (SageSpell.均衡预后II.IsReady())
                slot.Add(new Spell(SageSpell.均衡预后II, SpellTargetType.Self));
            else
                slot.Add(new Spell(SageSpell.均衡预后, SpellTargetType.Self));
        }
        
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
 
    


