using AEAssist.Avoid;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine;
using Dalamud.Game.ClientState.Objects.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ayanotan.Sage.Data;
using AEAssist.Extension;
using AEAssist;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Ayanotan.WhiteMage.Data;
using Ayanotan.WhiteMage.Setting;
using Ayanotan.Sage;
using static Dalamud.Interface.Utility.Raii.ImRaii;
using AEAssist.JobApi;

namespace Ayanotan.Sage.SloResolvers;
public class Slot_HealToDoom : ISlotSequence
{

    public List<Action<Slot>> Sequence { get; } = [Step0, Step1, Step2];

    private static IBattleChara target;

    public int StartCheck()
    {

        if (Helpers.队员是否拥有BUFF(SageData.塞壬的歌声) && Helpers.当前地图id == Helpers.Map.领航明灯天狼星灯塔)
        {
            if (GameObjectExtension.CurrentHpPercent(Helpers.获取拥有buff队员(SageData.塞壬的歌声)) < 100)
            {
                target = Helpers.获取拥有buff队员(SageData.塞壬的歌声);
                return 1;
            }
        }

        if (Helpers.队员是否拥有BUFF(SageData.死亡宣告_210) && Helpers.当前地图id == Helpers.Map.武装圣域放浪神古神殿)
        {
            if (GameObjectExtension.CurrentHpPercent(Helpers.获取拥有buff队员(SageData.死亡宣告_210)) < 100)
            {
                target = Helpers.获取拥有buff队员(SageData.死亡宣告_210);
                return 1;
            }
        }

        if (Helpers.队员是否拥有BUFF(SageData.死亡宣告_1769) && (Helpers.当前地图id == Helpers.Map.泽罗姆斯歼殛战_红月深处 || Helpers.当前地图id == Helpers.Map.乐欲之所瓯博讷修道院))
        {
            if (GameObjectExtension.CurrentHpPercent(Helpers.获取拥有buff队员(SageData.死亡宣告_1769)) < 100)
            {
                target = Helpers.获取拥有buff队员(SageData.死亡宣告_1769);
                return 1;
            }
        }

        if (Helpers.队员是否拥有BUFF(SageData.渐渐石化))
        {
            if (Helpers.获取拥有buff队员(SageData.渐渐石化).CurrentHpPercent() < 100)
            {
                target = Helpers.获取拥有buff队员(SageData.渐渐石化);
                return 1;
            }
        }

        if (Helpers.队员是否拥有BUFF(SageData.纯正死而不僵))
        {
            if (Helpers.获取拥有buff队员(SageData.纯正死而不僵).CurrentHpPercent() < 100)
            {
                target = Helpers.获取拥有buff队员(SageData.纯正死而不僵);
                return 1;
            }
        }

        return -1;
    }

    public int StopCheck(int index)
    {
        return -1;
    }

    private static bool needNext;


    private static void Step0(Slot slot)
    {

        needNext = false;
        var skillTarget = PartyHelper.CastableAlliesWithin15.Count(r =>
           r.CurrentHp > 0 && GameObjectExtension.CurrentHpPercent(r) <= 100);

        if (target.Distance(Core.Me) > 30)
        {
            return;
        }

        if (SageRotationEntry.QT.GetQt(QTKey.能力技治疗))
        {
            if (skillTarget > 1) 
            { 
                if (SageRotationEntry.QT.GetQt(QTKey.自生) && SpellsDefineAlternative.Physis.GetSpell().IsReadyWithCanCast()) 
                {
                    slot.Add(new Spell(SpellsDefineAlternative.Physis, Core.Me));
                    return;
                }
                if (SageRotationEntry.QT.GetQt(QTKey.寄生清汁) && SageSpell.寄生清汁.GetSpell().IsReadyWithCanCast())
                {
                    slot.Add(new Spell(SageSpell.寄生清汁, Core.Me));
                    return;
                }
                if (SageRotationEntry.QT.GetQt(QTKey.魂灵风息) && SageSpell.魂灵风息.GetSpell().IsReadyWithCanCast())
                {
                    slot.Add(new Spell(SageSpell.魂灵风息, Core.Me.GetCurrTarget()));
                    return;
                }
            }
            else
            { 
                if (SageRotationEntry.QT.GetQt(QTKey.白牛清汁) && SageSpell.白牛清汁.GetSpell().IsReadyWithCanCast())
                {
                    slot.Add(new Spell(SageSpell.白牛清汁, target));
                    return;
                }

                if (SageRotationEntry.QT.GetQt(QTKey.灵橡清汁) && SageSpell.灵橡清汁.GetSpell().IsReadyWithCanCast())
                {
                    slot.Add(new Spell(SageSpell.灵橡清汁, target));
                    return;
                }
            }

        }

        if (SageRotationEntry.QT.GetQt(QTKey.非均衡治疗))
        {
            if (skillTarget > 1 && !Core.Me.IsMoving())
            {
                slot.Add(new Spell(SageSpell.预后, Core.Me));
                return;
            }

            if (!Core.Me.IsMoving())
            {
                slot.Add(new Spell(SageSpell.诊断, target));
                return;
            }

        }
        needNext = true;
    }

    private static void Step1(Slot slot)
    {
        if (!needNext) return;
        if (SageSpell.活化.GetSpell().IsReadyWithCanCast())
        {
            slot.Add(new Spell(SageSpell.活化, SpellTargetType.Self));
        }
        else if (SageSpell.混合.GetSpell().IsReadyWithCanCast())
        {
            slot.Add(new Spell(SageSpell.混合, target));
        }

    }

    private static void Step2(Slot slot)
    {
        if (!needNext) return;
        var skillTarget = PartyHelper.CastableAlliesWithin15.Count(r =>
          r.CurrentHp > 0 && GameObjectExtension.CurrentHpPercent(r) <= 100);
        if (skillTarget > 1)
        {
            if (AI.Instance.BattleData.NextSlot == null)
                AI.Instance.BattleData.NextSlot = new Slot(2500);
            if (!Core.Resolve<JobApi_Sage>().Eukrasia)
                AI.Instance.BattleData.NextSlot.Add(SpellHelper.GetSpell(24290U));
            AI.Instance.BattleData.NextSlot.Add(SpellHelper.GetSpell(SpellsDefineAlternative.EukrasianPrognosis));
            if (SpellExtension.IsUnlockWithCDCheck(24301U))
                AI.Instance.BattleData.NextSlot.Add(new Spell(24301U, (IBattleChara)Core.Me));
            AI.Instance.BattleData.HighPrioritySlots_GCD.Enqueue(AI.Instance.BattleData.NextSlot);
        }
        else
        {
            if (Core.Me.IsMoving() && !Core.Me.HasAura(whmData.Swiftcast))
            {
                return;
            }
            else
            {
                slot.Add(new Spell(SageSpell.诊断, target));
                return;
            }
        }
    }


}
