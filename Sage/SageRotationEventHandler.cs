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
using AEAssist.Extension;
using AEAssist.Helper;
using Ayanotan.Sage.SloResolvers.GCD;
using AEAssist.CombatRoutine.Module.AILoop;
using AEAssist.JobApi;
using Ayanotan.Sage.Data;
using Dalamud.Game.ClientState.Objects.Types;



namespace Ayanotan.Sage;

public class SageRotationEventHandler:IRotationEventHandler
{
    private long randomSongTime;
    
    public async Task OnPreCombat()
    {

        if (Core.Me.Level < 30) return;
        
        if (Helpers.队伍成员数量 > 4 && !Core.Me.HasAura(2609) && !Core.Resolve<MemApiDuty>().IsOver && SageRotationEntry.QT.GetQt(QTKey.群盾))
        {
            Random random = new Random();
            int RandomWaitingTime = random.Next(700, 1600);
            await Coroutine.Instance.WaitAsync(RandomWaitingTime);
            var slot = new Slot();
            //如果群盾没法用，则用一下均衡，如果可以，直接单盾
            if (!Core.Resolve<JobApi_Sage>().Eukrasia)
            {
                slot.Add(SageSpell.均衡.GetSpell());
            }
            slot.Add(new Spell(SpellsDefineAlternative.EukrasianPrognosis, Core.Me));

            //执行并等待
            await slot.Run(AI.Instance.BattleData, false);
        }
        //获取可以施放技能的坦克队友中第一个没有你的单盾的对象 id2607的buff是均衡诊断
        if (Helpers.队伍成员数量 <= 4 && !(PartyHelper.CastableTanks.FirstOrDefault(agent => !agent.HasAura(2607)) == null) && !Core.Resolve<MemApiDuty>().IsOver && SageRotationEntry.QT.GetQt(QTKey.单盾))
        {
            Random random = new Random();
            int RandomWaitingTime = random.Next(700, 1600);
            await Coroutine.Instance.WaitAsync(RandomWaitingTime);
            var slot = new Slot();
            //如果单盾没法用，则用一下均衡，如果可以，直接单盾
            if (!Core.Resolve<JobApi_Sage>().Eukrasia)
            {
                slot.Add(SageSpell.均衡.GetSpell());
            }
            slot.Add(new Spell(SageSpell.均衡诊断, PartyHelper.CastableTanks.FirstOrDefault(agent => !agent.HasAura(2607))));
            //执行并等待
            await slot.Run(AI.Instance.BattleData, false);
        }
    }

    public void OnResetBattle()
    {
        // QT的设置重置为默认值
        //SageRotationEntry.QT.Reset();

        // 重置战斗中缓存的数据
        SageBattleData.Reset();

    }

    public Task OnNoTarget()
    {
      

        if (Helpers.是否在副本中())
        {
         

            if (SageOnNoTargetStrategy.拉人())
            {
                return Task.CompletedTask;
            }

            if (Helpers.目标战斗状态(Core.Me) && !Core.Resolve<MemApiDuty>().IsOver)
            {
                if (SageOnNoTargetStrategy.奶人())
                {
                    return Task.CompletedTask;
                }

                if (SageOnNoTargetStrategy.拉人())
                {
                    return Task.CompletedTask;
                }
            }

            
        }


        return Task.CompletedTask;
    }

    public void OnSpellCastSuccess(Slot slot, Spell spell)
    {

    }

    public void AfterSpell(Slot slot, Spell spell)
    {

    }

    public void OnBattleUpdate(int currTimeInMs)
    {

    }
    
    public void OnEnterRotation()
    {
        Core.Resolve<MemApiChatMessage>().Toast2("Ciallo～(∠・ω< )⌒☆"+"\n欢迎使用Ayanotan的贤者日随ACR喵~当前版本v25.4.1", 300, 6000);
        LogHelper.Print("欢迎使用Ayanotan的贤者日随ACR喵~发现问题还请随时到DC反馈");
        LogHelper.Print("请不要开启GCD偏移或长臂猿等，可能会造成意料之外的卡手");


    }

    public void OnExitRotation()
    {

    }

    public void OnTerritoryChanged()
    {

    }
}
