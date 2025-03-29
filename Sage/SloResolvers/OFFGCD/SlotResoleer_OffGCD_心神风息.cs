using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Ayanotan.Sage.Data;
using Ayanotan.WhiteMage.Data;
using Ayanotan.Sage;
using AEAssist.Extension;
using AEAssist.CombatRoutine.Module.Target;
using AEAssist.Define;
using Dalamud.Game.ClientState.Objects.Types;
using System.Numerics;


namespace Ayanotan.Sage.SloResolvers.OFFGCD;

public class SlotResoleer_OffGCD_心神风息 : ISlotResolver
{
    private IBattleChara target;
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
    
    public int Check()
    {
        target = (from r in TargetMgr.Instance.EnemysIn25.Values
                  where r.DistanceToPlayer() <= 25
                  orderby TargetHelper.GetNearbyEnemyCount(r, 25, 5) descending
                  select r).FirstOrDefault();

        var aoeCount = TargetHelper.GetNearbyEnemyCount(25);
        
        if (target == null) //解决NullReferenceException问题
        {
            return -5;
        }
        else
        {
            if (!(TargetHelper.GetNearbyEnemyCount(target, 25, 5) <= 2 && Core.Me.GetCurrTarget().CurrentHpPercent() <= 0.3f) &&
               (Core.Me.GetCurrTarget().IsBoss() || Core.Me.Level >= 82) &&
               !Core.Me.GetCurrTarget().HasAura(SageData.EukrasianDosis) &&
               !Core.Me.GetCurrTarget().HasAura(SageData.EukrasianDosisIi) &&
               !Core.Me.GetCurrTarget().HasAura(SageData.EukrasianDosisIii) &&
               !Core.Me.GetCurrTarget().HasAura(SageData.EukrasianDyskrasia) && SageSpell.均衡.IsReady())
                return -1;
            if (!IsBoss(Core.Me.GetCurrTarget()) && (TargetHelper.GetNearbyEnemyCount(target, 25, 5) <= 1) && aoeCount >= 3)
                return -4;
        }

        if (!SageSpell.心神风息.IsReady())
            return -3;
        if (SageRotationEntry.QT.GetQt(QTKey.心神风息) == false)
            return -2;

        return 0;
    }

    public void Build(Slot slot)
    {
        slot.Add(new Spell(SageSpell.心神风息, target));
    }

}
