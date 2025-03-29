using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ayanotan.Sage;
using AEAssist.MemoryApi;
using AEAssist;
using Ayanotan.Sage.SloResolvers.GCD;
using Ayanotan.Sage.SloResolvers.OFFGCD;
using AEAssist.Extension;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using System.Numerics;



namespace Ayanotan.Sage
{   public class SpellsDefineAlternative {
        public static uint EukrasianPrognosis
        {
            get
            {
                const uint defaultEukrasianPrognosis = 24292;
                const uint level96EukrasianPrognosis = 37034;

                return Core.Me.Level >= 96 ? level96EukrasianPrognosis : defaultEukrasianPrognosis;
            }
        }
        public static uint Dyskrasia
        {
            get
            {
                const uint defaultDyskrasia = 24297;
                const uint level82Dyskrasia = 24315;

                return Core.Me.Level >= 82 ? level82Dyskrasia : defaultDyskrasia;
            }
        }
        public static uint Toxikon
        {
            get
            {
                const uint defaultToxikon = 24304;
                const uint level82Toxikon = 24316;

                return Core.Me.Level >= 82 ? level82Toxikon : defaultToxikon;
            }
        }
        public static uint Physis
        {
            get
            {
                const uint defaultPhysis = 24288;
                const uint level60Physis = 24302;

                return Core.Me.Level >= 60 ? level60Physis : defaultPhysis;
            }
        }
    }
   

    public static class SageOnNoTargetStrategy
    {
        public static bool
            奶人()
        {


            if ( Helpers.副本人数() > 4 || Core.Resolve<MemApiDuty>().InBossBattle)
            {

                SlotResolver_GCD_群盾 上天群盾 = new SlotResolver_GCD_群盾();
                if (上天群盾.Check() >= 0)
                {
                    AI.Instance.BattleData.NextSlot ??= new Slot();
                    上天群盾.Build(AI.Instance.BattleData.NextSlot);
                    return true;
                }

                SlotResolver_OffGCD_寄生清汁 上天寄生清汁 = new SlotResolver_OffGCD_寄生清汁();
                if (上天寄生清汁.Check() >= 0)
                {
                    AI.Instance.BattleData.NextSlot ??= new Slot();
                    上天寄生清汁.Build(AI.Instance.BattleData.NextSlot);
                    return true;
                }

                SlotResoleer_OffGCD_整体论 上天整体论 = new SlotResoleer_OffGCD_整体论();
                if (上天整体论.Check() >= 0)
                {
                    AI.Instance.BattleData.NextSlot ??= new Slot();
                    上天整体论.Build(AI.Instance.BattleData.NextSlot);
                    return true;
                }

                SlotResoleer_OffGCD_白牛清汁 上天白牛清汁 = new SlotResoleer_OffGCD_白牛清汁();
                if (上天白牛清汁.Check() >= 0)
                {
                    AI.Instance.BattleData.NextSlot ??= new Slot();
                    上天白牛清汁.Build(AI.Instance.BattleData.NextSlot);
                    return true;
                }

                SlotResoleer_OffGCD_灵橡清汁 上天灵橡清汁 = new SlotResoleer_OffGCD_灵橡清汁();
                if (上天灵橡清汁.Check() >= 0)
                {
                    AI.Instance.BattleData.NextSlot ??= new Slot();
                    上天灵橡清汁.Build(AI.Instance.BattleData.NextSlot);
                    return true;
                }
                SlotResolver_GCD_预后 上天预后 = new SlotResolver_GCD_预后();
                if (上天预后.Check() >= 0)
                {
                    AI.Instance.BattleData.NextSlot ??= new Slot();
                    上天预后.Build(AI.Instance.BattleData.NextSlot);
                    return true;
                }

                SlotResolver_GCD_诊断 上天诊断 = new SlotResolver_GCD_诊断();
                if (上天预后.Check() >= 0)
                {
                    AI.Instance.BattleData.NextSlot ??= new Slot();
                    上天预后.Build(AI.Instance.BattleData.NextSlot);
                    return true;
                }
                SlotResolver_GCD_康复 上天康复 = new SlotResolver_GCD_康复();
                if (上天群盾.Check() >= 0)
                {
                    AI.Instance.BattleData.NextSlot ??= new Slot();
                    上天群盾.Build(AI.Instance.BattleData.NextSlot);
                    return true;
                }


            }

            return false;
        }
        public static bool 拉人()
        {
            Slot_Raise 上天复活 = new();
            if (上天复活.StartCheck() < 0) return false;

            foreach (var action in 上天复活.Sequence)
            {
                AI.Instance.BattleData.NextSlot ??= new Slot();
                action(AI.Instance.BattleData.NextSlot);
            }
            return true;

        }

      
    }
}
