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
using System;


namespace Ayanotan.Sage.SloResolvers.OFFGCD;

public class SlotResoleer_OffGCD_输血 : ISlotResolver
{
    private IBattleChara target;

    public static bool 目标是否可见或在技能范围内(uint actionId)
    {
        return Core.Resolve<MemApiSpell>().GetActionInRangeOrLoS(actionId) is not (566 or 562);
    }

    public int Check()
    {
        //检查开关
        if (SageRotationEntry.QT.GetQt(QTKey.输血) == false)
            return -3;
        //检查CD
        if (!SageSpell.输血.IsReady())
            return -1;
        //检查开关
        if (!SageRotationEntry.QT.GetQt(QTKey.能力技治疗))
            return -3;
        //目标血量过少不需要开
        if (Core.Me.GetCurrTarget().CurrentHpPercent() <= 0.05f)
            return -3;
        //检查BOSS是否读条死刑类技能
        if (Core.Me.GetCurrTarget() != null && DeathSentenceHelper.IsDeathSentence(Core.Me.GetCurrTarget()))
            return 1;

        //检查是否有满足条件的目标
        target = (from r in PartyHelper.CastableAlliesWithin30
                  where r.CurrentHp != 0 && GameObjectExtension.CurrentHpPercent(r) <= SageSettings.Instance.输血阈值 && r.IsTank() && 目标是否可见或在技能范围内(SageSpell.输血) && !r.HasAnyAura(SageData.不能奶的无敌) && !r.HasAnyAura(SageData.暂不需要奶的无敌, 4000)
                  orderby GameObjectExtension.CurrentHpPercent(r)
                  select r).FirstOrDefault();

        if (target== null )
            return -2;
        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(new Spell(SageSpell.输血, target));
    }

}
