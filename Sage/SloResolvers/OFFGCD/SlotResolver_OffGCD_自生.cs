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
using AEAssist.Helper;
using Dalamud.Game.ClientState.Objects.Types;

namespace Ayanotan.Sage.SloResolvers.GCD;

public class SlotResolver_OffGCD_自生 : ISlotResolver
{
    private IBattleChara target;
    Spell spell_1 = new Spell(SageSpell.自生, SpellTargetType.Self);
    Spell spell_2 = new Spell(SageSpell.自生II, SpellTargetType.Self);
    // 返回>=0表示检测通过 即将调用Build方法
    public int Check_Before_Use()
    {
        //检查开关
        if (!SageRotationEntry.QT.GetQt(QTKey.自生) || !SageRotationEntry.QT.GetQt(QTKey.能力技治疗))
            return -10;
        //检查CD
        if (!SpellExtension.IsReadyWithCanCast(spell_1) || SpellExtension.IsUnlock(SageSpell.自生II))
            return -9;

        if (SageSpell.自生.GetSpell().RecentlyUsed(60000))
            return -11;
        if (SageSpell.智慧之爱.RecentlyUsed(10000))
            return -2;
        var aoeCount = TargetHelper.GetNearbyEnemyCount(5);
        if (aoeCount >= 2 && SageSpell.自生.IsReady())
            return 1;

        if (Helpers.二十五米视线内血量低于设定的队员数量(SageSettings.Instance.群抬血阈值) >= SageSettings.Instance.群奶人数阈值)
        {
            return 2;
        }
        return -1;
    }
    public int Check()
    {
        var result = Check_Before_Use();
        if (result < 0) return result;

       
        return result;
    }
    // 将指定技能加入技能队列中
    public void Build(Slot slot)
    {
       slot.Add(SageSpell.自生.GetSpell());

        if (SageBattleData.Healing_CD.ContainsKey("LastHotHeal"))
        {
            SageBattleData.Healing_CD["LastHotHeal"] = Helpers.GetTimeStamps();
        }
        else
        {
            SageBattleData.Healing_CD.TryAdd("LastHotHeal", Helpers.GetTimeStamps());
        }
    }
}

