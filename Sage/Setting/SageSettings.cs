using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.View.JobView;
using AEAssist.Helper;
using AEAssist.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Ayanotan.WhiteMage.Setting;

/// <summary>
/// 配置文件适合放一些一般不会在战斗中随时调整的开关数据
/// 如果一些开关需要在战斗中调整 或者提供给时间轴操作 那就用QT
/// 非开关类型的配置都放配置里 比如诗人绝峰能量配置
/// </summary>
public class SageSettings
{
    public static SageSettings Instance;

    #region 标准模板代码 可以直接复制后改掉类名即可
    private static string path;
    public static void Build(string settingPath)
    {
        path = Path.Combine(settingPath, nameof(SageSettings), ".json");
        if (!File.Exists(path))
        {
            Instance = new SageSettings();
            Instance.Save();
            return;
        }
        try
        {
            Instance = JsonHelper.FromJson<SageSettings>(File.ReadAllText(path));
        }
        catch (Exception e)
        {
            Instance = new();
            LogHelper.Error(e.ToString());
        }
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, JsonHelper.ToJson(this));
    }
    #endregion
    public bool MedicalII;
    public bool CureIII;
    public bool AfflatusSolace;
    public bool Aquaveil = true;
    public bool Assize = true;
    public bool DivineBenison = true;
    public bool Plenary = true;
    public bool Tetragrammaton = true;
    public bool PresenceofMind = true;
    public bool ThinAir = true;
    public float 群抬血阈值 = 0.8f;
    public float 输血阈值 = 0.9f;
    public float 单抬能力技抬血阈值 = 0.75f;
    public float 单盾阈值 = 0.7f;
    public int 群奶人数阈值 = 2;
    public float 拯救阈值 = 0.75f;
    public float 混合阈值 = 0.7f;
    public float 整体论抬血阈值 = 0.7f;
    public float 寄生清汁抬血阈值 = 0.75f;
    public float 单GCD抬血阈值 = 0.72f;
    public float 群GCD抬血阈值 = 0.65f;
    public int time = 1500;
    public int opener = 0;
    public int Esuna = 2;
    public int stack = 3;
    public bool OnlyTank;
    public bool 发炎走位 = true ;
    public int 保留蛇刺数量 = 1;
    public Dictionary<string, object> StyleSetting = new();
    public bool AutoReset = true;
    public JobViewSave JobViewSave = new() { MainColor = new Vector4(186 / 255f, 85 / 255f, 211 / 255f, 0.7f) };
    public List<Spell> OpenerList = new List<Spell>();
    public List<(uint, SpellTargetType)> OpenerListtest = new List<(uint, SpellTargetType)>();
    public List<uint> open = new List<uint>();

    

}