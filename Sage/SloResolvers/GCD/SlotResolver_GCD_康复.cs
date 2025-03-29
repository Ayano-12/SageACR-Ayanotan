using AEAssist.CombatRoutine;
using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Extension;
using Ayanotan.WhiteMage.Setting;
using Ayanotan.WhiteMage.Data;
using Ayanotan.Sage.Data;
using Ayanotan.Sage;
using AEAssist.ACT;
using System.Buffers.Text;

namespace Ayanotan.Sage.SloResolvers.GCD;

public class SlotResolver_GCD_康复 : ISlotResolver
{
    public int Check()
    {
        if (Core.Me.IsMoving() && !Core.Me.HasAura(whmData.Swiftcast))
        {
            return -3;
        }

        Spell spell_康复 = new Spell(SageSpell.康复, Helpers.获取可驱散队员());
        if (!SageRotationEntry.QT.GetQt(QTKey.康复) || !SpellExtension.IsReadyWithCanCast(spell_康复))
        {
            return -1;
        }
        //当自身处于沉默状态或是战技封印状态时 不执行康复
        if (Core.Me.HasAura(7) || Core.Me.HasAura(1060) || Core.Me.HasAura(1347) || Core.Me.HasAura(6) || Core.Me.HasAura(620))
            return -4;
        if (Core.Me.HasAnyAura(SageData.Henshin))
            return -5;
        if (SageSpell.康复.RecentlyUsed(3000))
            return -8;

        if (!Helpers.队员是否拥有可驱散状态())
        {
            return -1;
        }

        return 1;
    }


    public void Build(Slot slot)
    {
        slot.Add(new Spell(SageSpell.康复, Helpers.获取可驱散队员()));
    }

 
}