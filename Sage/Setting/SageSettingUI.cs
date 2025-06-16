using AEAssist.CombatRoutine.Module.Opener;
using AEAssist.GUI;
using Ayanotan.WhiteMage.Setting;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Ayanotan.WhiteMage.Setting;

public class SageSettingUI
{
    public static SageSettingUI Instance = new();
    public SageSettings SageSettings => SageSettings.Instance;

    private bool setting;
    public string? opener;
   

    public void Draw()
    {
        //ImGuiHelper.LeftInputInt("预读时间",
        //  ref SageSettings.Instance.time, 1300, 1500, 10);
        //ImGui.Text("起手设置");
        //if (SageSettings.Instance.opener == 0)
        //{
        //    opener = "使用即刻";
        //}
        //else if (SageSettings.Instance.opener == 1)
        //{
        //    opener = "保留即刻";
        //}
        //if (ImGui.BeginCombo("", opener))
        //{
        //    if (ImGui.Selectable("使用即刻"))
        //    {
        //        SageSettings.Instance.opener = 0;
        //    }
        //    if (ImGui.Selectable("保留即刻"))
        //    {
        //        SageSettings.Instance.opener = 1;
        //    }
        //    ImGui.EndCombo();
        //}
       

     
       if (ImGui.CollapsingHeader("我超！外挂！（关闭本ACR后仍会生效）", (ImGuiTreeNodeFlags)32))
                this.RenderHackSettings();
       if (ImGui.CollapsingHeader("治疗设置", (ImGuiTreeNodeFlags)32))
                this.RenderHealSettings();

        

        if (ImGui.Button("Save"))
        {
            SageSettings.Instance.Save();
        }
    }
    private void RenderToggle(string label, ref bool setting, string tooltip)
    {
        ImGuiHelper.ToggleButton(label, ref setting);
        ImGuiHelper.SetHoverTooltip(tooltip);
    }
    private void RenderHackSettings()
    {
        if (SageSettings.Instance == null)
            return;
        SageSettings instance = SageSettings.Instance;

        bool movePermission = instance.MovePermission;
        this.RenderToggle("强制移动", ref instance.MovePermission, "免除控制");
        if (instance.MovePermission != movePermission)
            Sage.Helpers.MovePermission();

        bool actionRange = instance.ActionRange;
        this.RenderToggle("长臂猿", ref instance.ActionRange, "技能距离增加3米");
        if (instance.ActionRange != actionRange)
            Sage.Helpers.ActionRange();

        bool speedUP = instance.SpeedUP;
        float 加速量 = instance.加速量;
        ImGuiHelper.LeftInputFloat("加速量（0~1，默认为0.2）", ref instance.加速量, 0, 1);
        this.RenderToggle("移速加快", ref instance.SpeedUP, "移速加百分之15");
        if (instance.SpeedUP != speedUP || instance.加速量 != 加速量)
            Sage.Helpers.SpeedUP();

        //bool skillPostActionMove = instance.SkillPostActionMove;
        //this.RenderToggle("后摇可移动", ref instance.SkillPostActionMove, "所有技能后摇时可以移动");
        //if (instance.SkillPostActionMove == skillPostActionMove)
        //    return;
        //Sage.Helpers.SkillPostActionMove();
    }
    private void RenderHealSettings()
    {
        if (SageSettings.Instance == null)
            return;
        SageSettings instance = SageSettings.Instance;

        ImGuiHelper.LeftInputInt("群奶人数阈值", ref instance.群奶人数阈值);
        ImGuiHelper.LeftInputFloat("群抬血阈值", ref instance.群抬血阈值, 0.01f);
        ImGuiHelper.LeftInputFloat("输血阈值", ref instance.输血阈值, 0.01f);
        ImGuiHelper.LeftInputFloat("拯救阈值", ref instance.拯救阈值, 0.01f);
        ImGuiHelper.LeftInputFloat("混合阈值", ref instance.混合阈值, 0.01f);
        ImGuiHelper.LeftInputFloat("单抬能力技抬血阈值", ref instance.单抬能力技抬血阈值, 0.01f);
        ImGuiHelper.LeftInputFloat("寄生清汁抬血阈值", ref instance.寄生清汁抬血阈值, 0.01f);
        ImGuiHelper.LeftInputFloat("整体论抬血阈值", ref instance.整体论抬血阈值, 0.01f);
        ImGuiHelper.LeftInputFloat("单盾阈值", ref instance.单盾阈值, 0.01f);
        ImGuiHelper.LeftInputFloat("单GCD抬血阈值（诊断）", ref instance.单GCD抬血阈值, 0.01f);
        ImGuiHelper.LeftInputFloat("群GCD抬血阈值（预后）", ref instance.群GCD抬血阈值, 0.01f);
        //bool skillPostActionMove = instance.SkillPostActionMove;
        //this.RenderToggle("后摇可移动", ref instance.SkillPostActionMove, "所有技能后摇时可以移动");
        //if (instance.SkillPostActionMove == skillPostActionMove)
        //    return;
        //Sage.Helpers.SkillPostActionMove();
    }
}