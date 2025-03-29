using AEAssist.CombatRoutine;
using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using Ayanotan.WhiteMage.Data;
using Ayanotan.Sage.Data;
using AEAssist.JobApi;
using Ayanotan.Sage;
using Dalamud.Game.ClientState.Objects.Types;
using Ayanotan.WhiteMage.Setting;
using AEAssist.Define;

namespace Ayanotan.Sage.SloResolvers.GCD;

public class SlotResolver_GCD_注药 : ISlotResolver
{
    Spell spell_发炎 = new Spell(SageSpell.发炎, Core.Me.GetCurrTarget());
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
    

    Spell spell_箭毒 = new Spell(SageSpell.箭毒,Core.Me.GetCurrTarget());
    private Spell GetSpell()
    {
         return SageSpell.注药.GetSpell();
    }
    // 返回>=0表示检测通过 即将调用Build方法
    public int Check()
    {
        var aoeCount = TargetHelper.GetNearbyEnemyCount(5);
        if (SageRotationEntry.QT.GetQt(QTKey.停手))
            return -1;
        if (Core.Me.HasAura(whmData.Swiftcast))
            return -3;
        if (Core.Me.IsMoving() && (!SpellExtension.IsUnlock(SageSpell.箭毒) || Core.Resolve<JobApi_Sage>().Addersting < 1 ) && Core.Resolve<MemApiSpell>().CheckActionChange(SageSpell.发炎).GetSpell().Charges < 1)
            return -2;
        if (Core.Me.IsMoving() && !SpellExtension.IsUnlock(SageSpell.箭毒) && !SpellExtension.IsUnlock(SageSpell.发炎))
            return -6;
        if (Core.Me.IsMoving() && !IsBoss(Core.Me.GetCurrTarget()))
            return -7;
        if (Core.Me.IsMoving() && GCDHelper.GetGCDDuration() > 50)
            return -9;
        if (Core.Me.IsMoving() && !(Core.Resolve<JobApi_Sage>().Addersting > 0 && GCDHelper.GetGCDDuration() <= 50 && SpellExtension.IsUnlock(SageSpell.箭毒)) && !(GCDHelper.GetGCDDuration() <= 50 && Core.Resolve<MemApiSpell>().CheckActionChange(SageSpell.发炎).GetSpell().Charges >= 1 && SageSettings.Instance.发炎走位 && IsBoss(Core.Me.GetCurrTarget()) && SageRotationEntry.QT.GetQt(QTKey.发炎) && GameObjectExtension.Distance((IGameObject)Core.Me, (IGameObject)GameObjectExtension.GetCurrTarget((IBattleChara)Core.Me), (DistanceMode)7) <= 6.0f))
            return -11;
        //没有DOT不打注药，但怪物少于2且血量少于30%时DOT不会续，此时AOE也不会打，为防止停手，将前述条件非化后与原条件串行判断）
        if ( !(aoeCount <= 2 || Core.Me.GetCurrTarget().CurrentHpPercent() <= 0.3f ) && 
            (Core.Me.GetCurrTarget().IsBoss() || Core.Me.Level >= 82) &&
            !Core.Me.GetCurrTarget().HasAura(SageData.EukrasianDosis) &&
            !Core.Me.GetCurrTarget().HasAura(SageData.EukrasianDosisIi) &&
            !Core.Me.GetCurrTarget().HasAura(SageData.EukrasianDosisIii) &&
            !Core.Me.GetCurrTarget().HasAura(SageData.EukrasianDyskrasia) && SageSpell.均衡.IsReady() )
            return -5;
        if (SageSpell.失衡.IsReady() && aoeCount >= 2)
            return -4;
        if (Core.Me.HasAnyAura(SageData.Henshin))
            return -8;
        if (!Helpers.目标是否可见或在技能范围内(SageSpell.注药))
            return -10;
        if (Core.Me.GetCurrTarget().Distance(Core.Me,(DistanceMode)7) > 25.0f )
            return -12;
        if (Helpers.TargetHasAura(Core.Me.GetCurrTarget(), SageData.Enemyinvulnerability))
            return -2;
        if (Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(SageData.EukrasianDosis,3000)|| 
            Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(SageData.EukrasianDosisIi,3000) || 
            Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(SageData.EukrasianDosisIii,3000)||
            Core.Me.GetCurrTarget().HasMyAuraWithTimeleft(SageData.EukrasianDyskrasia,3000))
            return 1;
        

        return 0;
    }

    // 将指定技能加入技能队列中
    public void Build(Slot slot)
    {
        if(Core.Me.IsMoving() && Core.Resolve<JobApi_Sage>().Addersting > 0 && GCDHelper.GetGCDDuration() <= 50 && SpellExtension.IsUnlock(SageSpell.箭毒))
            slot.Add(SpellsDefineAlternative.Toxikon.GetSpell());
        //else if (Core.Me.IsMoving() && GCDHelper.GetGCDCooldown() <= 100)
        //    slot.Add(SageSpell.箭毒.GetSpell());
        else if (Core.Me.IsMoving() && GCDHelper.GetGCDDuration() <= 50 && Core.Resolve<MemApiSpell>().CheckActionChange(SageSpell.发炎).GetSpell().Charges >= 1 && SageSettings.Instance.发炎走位 && IsBoss(Core.Me.GetCurrTarget()) && SageRotationEntry.QT.GetQt(QTKey.发炎) && GameObjectExtension.Distance((IGameObject)Core.Me, (IGameObject)GameObjectExtension.GetCurrTarget((IBattleChara)Core.Me), (DistanceMode)7) <= 6.0)
        {
            slot.Add(SageSpell.发炎.GetSpell());
        }
        else slot.Add(GetSpell());
    }
}
