using AEAssist.CombatRoutine.Module.Opener;
using AEAssist.GUI;
using Ayanotan.WhiteMage.Setting;
using ImGuiNET;

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
        ImGui.Checkbox("治疗详细设置", ref setting);

        if (setting)
        {
            ImGuiHelper.LeftInputInt("群奶人数阈值", ref SageSettings.Instance.群奶人数阈值);
            ImGuiHelper.LeftInputFloat("群抬血阈值", ref SageSettings.Instance.群抬血阈值, 0.01f);
            ImGuiHelper.LeftInputFloat("输血阈值", ref SageSettings.Instance.输血阈值, 0.01f);
            ImGuiHelper.LeftInputFloat("拯救阈值", ref SageSettings.Instance.拯救阈值, 0.01f);
            ImGuiHelper.LeftInputFloat("混合阈值", ref SageSettings.Instance.混合阈值, 0.01f);
            ImGuiHelper.LeftInputFloat("单抬能力技抬血阈值", ref SageSettings.Instance.单抬能力技抬血阈值, 0.01f);
            ImGuiHelper.LeftInputFloat("寄生清汁抬血阈值", ref SageSettings.Instance.寄生清汁抬血阈值, 0.01f);
            ImGuiHelper.LeftInputFloat("整体论抬血阈值", ref SageSettings.Instance.整体论抬血阈值, 0.01f);
            ImGuiHelper.LeftInputFloat("单盾阈值", ref SageSettings.Instance.单盾阈值, 0.01f);
            ImGuiHelper.LeftInputFloat("单GCD抬血阈值（诊断）", ref SageSettings.Instance.单GCD抬血阈值, 0.01f);
            ImGuiHelper.LeftInputFloat("群GCD抬血阈值（预后）", ref SageSettings.Instance.群GCD抬血阈值, 0.01f);
        }

        if (ImGui.Button("Save"))
        {
            SageSettings.Instance.Save();
        }
    }
}