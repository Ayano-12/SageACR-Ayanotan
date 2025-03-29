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

public class SlotResoleer_OffGCD_混合 : ISlotResolver
{
    private IBattleChara target;
    Spell spell_混合 = new Spell(SageSpell.混合, Core.Me.GetCurrTargetsTarget());
    public int Check()
    {
        //检查开关
        if (!SageRotationEntry.QT.GetQt(QTKey.混合))
            return -3;
        //检查CD
        if (!SpellExtension.IsReadyWithCanCast(spell_混合))
            return -1;
        if (SageSpell.输血.IsReady())
            return -4;
        
        //检查开关
        if (!SageRotationEntry.QT.GetQt(QTKey.能力技治疗))
            return -2;
        //检查BOSS是否读条死刑类技能
        if (Core.Me.GetCurrTarget() != null && DeathSentenceHelper.IsDeathSentence(Core.Me.GetCurrTarget()))
            return 1;
        //检查是否有满足条件的目标
        target = (from r in PartyHelper.CastableAlliesWithin30
                  where r.CurrentHp != 0 && GameObjectExtension.CurrentHpPercent(r) <= SageSettings.Instance.混合阈值 && r.IsTank() 
                  orderby GameObjectExtension.CurrentHpPercent(r)
                  select r).FirstOrDefault();

        if (target == null )
            return -2;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(new Spell(SageSpell.混合, target));
    }

}
