using AEAssist;
using AEAssist.ACT;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.Trigger.Node;
using AEAssist.CombatRoutine.View.JobView;
using AEAssist.Extension;
using AEAssist.FFLogsRank;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Ayanotan.Sage.Data;
using Dalamud.Interface.Textures.TextureWraps;
//using ECommons;
using ImGuiNET;
//using PlatformServer.Network;
using System.Numerics;

namespace Ayanotan.Sage.HotKey;
public class 均衡预后 : IHotkeyResolver
{
    public void Draw(Vector2 size)
    {
        Vector2 size3 = size * 0.8f;
        ImGui.SetCursorPos(size * 0.1f);
        IDalamudTextureWrap textureWrap;
        if (!Core.Resolve<MemApiIcon>().GetActionTexture(SageSpell.均衡预后, out textureWrap))
            return;
        ImGui.Image(textureWrap.ImGuiHandle, size3);
    }

    public void DrawExternal(Vector2 size, bool isActive)
        => SpellHelper.DrawSpellInfo(new Spell(SageSpell.均衡预后, SpellTargetType.Self), size, isActive);

    public int Check() => 0;

    public void Run()
    {
        // 这里实现触发时的逻辑
        if (AI.Instance.BattleData.NextSlot == null)
            AI.Instance.BattleData.NextSlot = new Slot();
        if (!Core.Resolve<JobApi_Sage>().Eukrasia)
            AI.Instance.BattleData.NextSlot.Add(SageSpell.均衡.GetSpell());
        AI.Instance.BattleData.NextSlot.Add(SpellsDefineAlternative.EukrasianPrognosis.GetSpell());
    }

}
public class 给T套单盾 : IHotkeyResolver
{
    public void Draw(Vector2 size)
    {
        Vector2 size3 = size * 0.8f;
        ImGui.SetCursorPos(size * 0.1f);
        IDalamudTextureWrap textureWrap;
        if (!Core.Resolve<MemApiIcon>().GetActionTexture(SageSpell.均衡诊断, out textureWrap))
            return;
        ImGui.Image(textureWrap.ImGuiHandle, size3);
    }

    public void DrawExternal(Vector2 size, bool isActive)
    {
        //=> SpellHelper.DrawSpellInfo(new Spell(SageSpell.均衡诊断, PartyHelper.CastableTanks.FirstOrDefault(agent => !agent.HasAura(2607))), size, isActive);
    }

    public int Check() => 0;
    public void Run()
    {
        if (PartyHelper.CastableTanks.FirstOrDefault() == null)
        {
            Core.Resolve<MemApiChatMessage>().Toast2("有T吗你就开？有T吗你就开？", 300, 4000);
            return;
        }
        else
        {
            if (AI.Instance.BattleData.NextSlot == null)
                AI.Instance.BattleData.NextSlot = new Slot();
            if (!Core.Resolve<JobApi_Sage>().Eukrasia)
                AI.Instance.BattleData.NextSlot.Add(SageSpell.均衡.GetSpell());
            AI.Instance.BattleData.NextSlot.Add(new Spell(SageSpell.均衡诊断, PartyHelper.CastableTanks.FirstOrDefault()));
        } 
    }
}

public class 给血量最低的单盾 : IHotkeyResolver
{
    public void Draw(Vector2 size)
    {
        Vector2 size3 = size * 0.8f;
        ImGui.SetCursorPos(size * 0.1f);
        IDalamudTextureWrap textureWrap;
        if (!Core.Resolve<MemApiIcon>().GetActionTexture(SageSpell.均衡诊断, out textureWrap))
            return;
        ImGui.Image(textureWrap.ImGuiHandle, size3);
    }

    public void DrawExternal(Vector2 size, bool isActive) { }
        //=> SpellHelper.DrawSpellInfo(new Spell(SageSpell.均衡诊断, PartyHelper.CastableTanks.FirstOrDefault(agent => !agent.HasAura(2607))), size, isActive);
    public int Check() => 0;
    public void Run()
    {
        if (AI.Instance.BattleData.NextSlot == null)
            AI.Instance.BattleData.NextSlot = new Slot();
        if (!Core.Resolve<JobApi_Sage>().Eukrasia)
            AI.Instance.BattleData.NextSlot.Add(SageSpell.均衡.GetSpell());
        AI.Instance.BattleData.NextSlot.Add(new Spell(SageSpell.均衡诊断, Helpers.获取血量最低成员));
    }
    public class 手动拉人 : IHotkeyResolver
    {
        public void Draw(Vector2 size)
        {
            Vector2 size3 = size * 0.8f;
            ImGui.SetCursorPos(size * 0.1f);
            IDalamudTextureWrap textureWrap;
            if (!Core.Resolve<MemApiIcon>().GetActionTexture(SageSpell.复苏, out textureWrap))
                return;
            ImGui.Image(textureWrap.ImGuiHandle, size3);
        }

        public void DrawExternal(Vector2 size, bool isActive)
            => SpellHelper.DrawSpellInfo(new Spell(SageSpell.复苏, (from r in PartyHelper.DeadAllies
                                                                  where !r.HasAura(148u) && Helpers.目标是否可见或在技能范围内(24287) 
                                                                  select r).FirstOrDefault()), size, isActive);

        public int Check() => 0;

        public void Run()
        {
            // 这里实现触发时的逻辑
            if (AI.Instance.BattleData.NextSlot == null)
                AI.Instance.BattleData.NextSlot = new Slot();
            if (Core.Me.CurrentMp >= 2400)
                AI.Instance.BattleData.NextSlot.Add(new Spell(SageSpell.复苏, (from r in PartyHelper.DeadAllies
                                                                             where !r.HasAura(148u) && Helpers.目标是否可见或在技能范围内(24287)
                                                                             select r).FirstOrDefault()));
        }

    }
}

public class LB : IHotkeyResolver
{
    public void Draw(Vector2 size)
    {
        Vector2 size1 = size * 0.8f; 
        ImGui.SetCursorPos(size * 0.1f); 
        if (Core.Resolve<MemApiIcon>().TryGetTexture("Resources\\Spells\\LB.png", out var textureWrap))
            ImGui.Image(textureWrap.ImGuiHandle, size1);
    }

    public void DrawExternal(Vector2 size, bool isActive)
    {

    }

    public int Check() => 0;

    public void Run()
    {
        if (!SageSpell.极限技.GetSpell().IsReadyWithCanCast()) 
        {
            Core.Resolve<MemApiChatMessage>().Toast2("没有三段LB，别点了", 300, 4000);
            return;
        }
        else
        {
            if (AI.Instance.BattleData.NextSlot == null)
            {
                AI.Instance.BattleData.NextSlot = new Slot();
            }
            AI.Instance.BattleData.NextSlot.Add(SageSpell.极限技.GetSpell());
        }
        
    }
}


