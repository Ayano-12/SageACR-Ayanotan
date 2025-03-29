using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Ayanotan.Sage.Data;
using Dalamud.Game.ClientState.Objects.Types;
using Ayanotan.WhiteMage.Data;
using Ayanotan.WhiteMage.Setting;
using Ayanotan.Sage;


namespace Ayanotan.Sage.SloResolvers.OFFGCD;

public class SlotResoleer_OffGCD_拯救 : ISlotResolver
{
    private IBattleChara target;
    public int Check()
    {
        //检查开关
        if (SageRotationEntry.QT.GetQt(QTKey.拯救) == false)
            return -3;
        //检查CD
        if (!SageSpell.拯救.IsReady())
            return -1;
        if (SageSpell.输血.IsReady())
            return -4;
        if (SageSpell.白牛清汁.IsReady())
            return -5;
        //检查开关
        if (!SageRotationEntry.QT.GetQt(QTKey.能力技治疗))
            return -3;
        //检查BOSS是否读条死刑类技能
        if (Core.Me.GetCurrTarget() != null && DeathSentenceHelper.IsDeathSentence(Core.Me.GetCurrTarget()))
            return 1;
        //检查是否有满足条件的目标
        target = (from r in PartyHelper.CastableAlliesWithin30
                  where r.CurrentHp != 0 && GameObjectExtension.CurrentHpPercent(r) <= SageSettings.Instance.拯救阈值 && r.IsTank() && !r.HasAnyAura(SageData.不能奶的无敌) && !r.HasAnyAura(SageData.暂不需要奶的无敌, 4000)
                  orderby GameObjectExtension.CurrentHpPercent(r)
                  select r).FirstOrDefault();

        if (target == null)
            return -2;
        return 0;
    }

    public void Build(Slot slot)
    {
       
      
        slot.Add(new Spell(SageSpell.拯救, target));
    }

}
