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

public class SlotResoleer_OffGCD_心关 : ISlotResolver
{
    private IBattleChara OrijinaruTtarget;
    private IBattleChara OrijinaruTnashitarget;
    private IBattleChara Genzaitarget;
    private IBattleChara Tadashitarget;
    private IBattleChara IkiteruTank;

    public static bool 队员是否拥有BUFF(uint buff)
    {
        return PartyHelper.CastableAlliesWithin30.Any(agent => agent.HasAura(buff));
    }
    public int Check()
    {   //定义目标：包括初始T目标，初始非T目标，现在心关目标，应该心关的目标
        OrijinaruTtarget = (from t in PartyHelper.CastableAlliesWithin30
                            where t.IsTank() && (!队员是否拥有BUFF(2605)|| !Core.Me.HasAura(2604)) && t.CurrentHp != 0
                            orderby t.MaxHp descending
                            select t).FirstOrDefault();
        OrijinaruTnashitarget = (from xt in PartyHelper.CastableAlliesWithin30
                                 where (!队员是否拥有BUFF(2605) || !Core.Me.HasAura(2604)) && xt.CurrentHp != 0
                                 orderby xt.MaxHp descending
                                 select xt).FirstOrDefault();
        Genzaitarget = (from g in PartyHelper.CastableAlliesWithin30
                        where g.HasLocalPlayerAura(2605) && Core.Me.HasAura(2604)
                        select g).FirstOrDefault();
        Tadashitarget = (from c in PartyHelper.CastableAlliesWithin30
                         where c == Core.Me.GetCurrTargetsTarget() && c.CurrentHp != 0
                         select c).FirstOrDefault();
        IkiteruTank = (from it in PartyHelper.CastableTanks
                       where it.IsTank() && it.CurrentHp != 0
                       orderby it.MaxHp descending
                       select it).FirstOrDefault();

        //检查开关,2604心关，2605关心
        if (SageRotationEntry.QT.GetQt(QTKey.心关) == false) 
            return -3;
        //如果队伍里有T，且此时自己没有上过心关，则先挑选一位幸运T上心关
        if (Helpers.队伍里是否有T() && OrijinaruTtarget != null && !Core.Me.HasAura(2604))
            return 1;
        //如果队伍里没有T，且此时自己没有上过心关，则先挑选一位幸运观众上心关
        if (!Helpers.队伍里是否有T() && OrijinaruTnashitarget != null && !Core.Me.HasAura(2604))
            return 2;
        //如果不存在活着的T，且现在心关的目标不是目标的目标，且有正确心关目标的转移对象，则转移心关
        if (IkiteruTank == null && Genzaitarget != Core.Me.GetCurrTargetsTarget() && !SageSpell.心关.RecentlyUsed(5200) && Tadashitarget != null)
            return 3;
        //如果存在活着的T，且现在心关的目标不是目标的目标，且有正确心关Tank目标的转移对象，则转移心关
        if (IkiteruTank != null && Genzaitarget != Core.Me.GetCurrTargetsTarget() && !SageSpell.心关.RecentlyUsed(5200) && Tadashitarget != null && Tadashitarget.IsTank())
            return 4;

        return -1;
    }

    public void Build(Slot slot)
    {
        if (Check() == 1)
        {
            slot.Add(new Spell(SageSpell.心关, OrijinaruTtarget));
        }
        else if (Check() == 2)
        {
            slot.Add(new Spell(SageSpell.心关, OrijinaruTnashitarget));
        }
        else if (Check() == 3 || Check() == 4)
        {
            slot.Add(new Spell(SageSpell.心关, Tadashitarget));
        }

    }

}
