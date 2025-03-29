using AEAssist.CombatRoutine;
using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using Ayanotan.WhiteMage.Data;
using Ayanotan.Sage.Data;
using Ayanotan.Sage;
using Ayanotan.WhiteMage.Setting;
using Dalamud.Game.ClientState.Objects.Types;
using AEAssist.ACT;

namespace Ayanotan.Sage.SloResolvers.GCD;

public class Slot_Raise : ISlotSequence
{
    public List<Action<Slot>> Sequence { get; } = [Step0, Step1];
    private static IBattleChara? killTarget;

    public static bool 目标是否可见或在技能范围内(uint actionId)
    {
        return Core.Resolve<MemApiSpell>().GetActionInRangeOrLoS(actionId) is not (566 or 562);
    }

    // 返回>=0表示检测通过 即将调用Build方法
    public int StartCheck()
    {
        //没即刻不拉人
        Spell spell_即刻咏唱 = new Spell(SpellsDefine.Swiftcast, SpellTargetType.Self);
        if (!SpellExtension.IsReadyWithCanCast(spell_即刻咏唱))
            return -4;
        //检查开关
        if (!SageRotationEntry.QT.GetQt(QTKey.拉人))
            return -3;
        //没蓝不拉
        if (Core.Me.CurrentMp < 2400)
            return -2;
        //PartyHelper.DeadAllies.FirstOrDefault(r => !r.HasAura(Buff.复活) && r.IsTargetable
        //检查有没有死的人,且死的人在可见范围内（没有掉到场地外）
        var killTarget = (from r in PartyHelper.DeadAllies
                  where !r.HasAura(148u) && 目标是否可见或在技能范围内(24287)
                  select r).FirstOrDefault();

        if (!Core.Resolve<MemApiSpell>().CheckActionInRangeOrLoS(24287, (IGameObject)killTarget))
            return -17;
        //var killTarget = PartyHelper.DeadAllies.FirstOrDefault(r => !r.HasAura(SageSpell.复苏));
        if (killTarget == null || !killTarget.IsValid())
            return -1;
        if (killTarget.Distance(Core.Me) > 30)
            return -6;
        if (Core.Me.Position.Y - killTarget.Position.Y > 1)
        {
            if (PartyHelper.Party.Count > 4)
            {
                return -5;
            }
        }

        return 1;
    }

    // 将指定技能加入技能队列中
    //public void Build(Slot slot)
    //{
    //    var killTarget = (from r in PartyHelper.DeadAllies
    //                      where !r.HasAura(148u) && 目标是否可见或在技能范围内(24287)
    //                      select r).FirstOrDefault();
    //    slot.Add(SpellsDefine.Swiftcast.GetSpell());
       
    //    slot.Add(new Spell(SageSpell.复苏, killTarget));
    //}
    public int StopCheck(int index)
    {
        return -1;
    }

    private static void Step0(Slot slot)
    {
        Coroutine.Instance.WaitAsync(300);
        var killTarget = (from r in PartyHelper.DeadAllies
                          where !r.HasAura(148u) && 目标是否可见或在技能范围内(24287) && Core.Resolve<MemApiSpell>().CheckActionInRangeOrLoS(24287, (IGameObject)r)
                          select r).FirstOrDefault();
        if (killTarget == null || !killTarget.IsValid())
        {
            return;
        }
        if (Core.Me.Position.Y - killTarget.Position.Y > 0.5)
        {
            if (PartyHelper.Party.Count > 4)
            {
                return;
            }
        }
        slot.Add(new Spell(SpellsDefine.Swiftcast, SpellTargetType.Self));
    }


    private static void Step1(Slot slot)
    {
        var killTarget = (from r in PartyHelper.DeadAllies
                          where !r.HasAura(148u) && 目标是否可见或在技能范围内(24287) && Core.Resolve<MemApiSpell>().CheckActionInRangeOrLoS(24287, (IGameObject)r)
                          select r).FirstOrDefault();
        if (killTarget == null || !killTarget.IsValid())
        {
            return;
        }
        if (Core.Me.Position.Y - killTarget.Position.Y > 0.5)
        {
            if (PartyHelper.Party.Count > 4)
            {
                return;
            }
        }
        slot.Add(new Spell(SageSpell.复苏, killTarget));
    }
}