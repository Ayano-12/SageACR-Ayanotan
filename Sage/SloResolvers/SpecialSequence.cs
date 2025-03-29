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
using Ayanotan.Sage.SloResolvers.GCD;

namespace Ayanotan.Sage.SloResolvers;
public static class SpecialSequence
{
    public static ISlotSequence[] Build()
    {
        return
        [   new Slot_Raise(),
            new Slot_HealToDoom()
        ];
    }
}
