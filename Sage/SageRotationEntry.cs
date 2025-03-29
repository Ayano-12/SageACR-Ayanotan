using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.View.JobView;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Dalamud.Game.ClientState.Objects.Types;
using Ayanotan.WhiteMage.Data;
using Ayanotan.WhiteMage.Setting;
using Ayanotan.Sage.SloResolvers.GCD;
using Ayanotan.Sage.SloResolvers.OFFGCD;
using Ayanotan.Sage.Triggers;
using ImGuiNET;
using Ayanotan.Sage.HotKey;
using Ayanotan.Sage.SloResolvers;
using Ayanotan.Sage.Himitu;
using static Ayanotan.Sage.HotKey.给血量最低的单盾;
using AEAssist.GUI;
using Dalamud.Game;
using Dalamud.Plugin.Services;
using Dalamud.Plugin;

namespace Ayanotan.Sage;


// 重要 类一定要Public声明才会被查找到

public class SageRotationEntry:IRotationEntry
{
    public string AuthorName { get; set; } = "Ayanotan";



    // 逻辑从上到下判断，通用队列是无论如何都会判断的 
    // gcd则在可以使用gcd时判断
    // offGcd则在不可以使用gcd 且没达到gcd内插入能力技上限时判断
    // pvp环境下 全都强制认为是通用队列
    private List<SlotResolverData> SlotResolvers = new()
    {
        // 通用队列 不管是不是gcd 都会判断的逻辑
        //new(new XXXXXXXX(),SlotMode.Always),
        
        // offGcd队列
        new (new SlotResoleer_OffGCD_心关(),SlotMode.OffGcd),                  
        new (new SlotResolver_OffGCD_自生II(),SlotMode.OffGcd),
        new (new SlotResolver_OffGCD_自生(),SlotMode.OffGcd),
        new (new SlotResolver_OffGCD_寄生清汁(),SlotMode.OffGcd),
        new (new SlotResoleer_OffGCD_输血(),SlotMode.OffGcd),
        new (new SlotResoleer_OffGCD_灵橡清汁(),SlotMode.OffGcd),
        new (new SlotResoleer_OffGCD_白牛清汁(),SlotMode.OffGcd),
        new (new SlotResoleer_OffGCD_坚角清汁(),SlotMode.OffGcd),
        new (new SlotResolver_OffGCD_根素(),SlotMode.OffGcd),
        new (new SlotResoleer_OffGCD_整体论(),SlotMode.OffGcd),
        new (new SlotResoleer_OffGCD_泛输血(),SlotMode.OffGcd),
        new (new SlotResoleer_OffGCD_拯救(),SlotMode.OffGcd),
        new (new SlotResoleer_OffGCD_醒梦(),SlotMode.OffGcd),
        new (new SlotResoleer_OffGCD_心神风息(),SlotMode.OffGcd),
        new (new SlotResoleer_OffGCD_智慧之爱(),SlotMode.OffGcd),
                 
              
        

            // gcd队列
          
        new(new SlotResolver_GCD_康复(),SlotMode.Gcd),
        new(new SlotResolver_GCD_魂灵风息(),SlotMode.Gcd),
        //new(new SlotResolver_GCD_群盾均衡(),SlotMode.Gcd),
        new(new SlotResolver_GCD_群盾(),SlotMode.Gcd),
        //new(new SlotResolver_GCD_单盾均衡(),SlotMode.Gcd),
        new(new SlotResolver_GCD_单盾(),SlotMode.Gcd),
        new(new SlotResolver_GCD_预后(),SlotMode.Gcd),
        new(new SlotResolver_GCD_诊断(),SlotMode.Gcd),
        //new(new SlotResolver_GCD_拉人(),SlotMode.Gcd),
        
        //new(new SlotResolver_GCD_Dot均衡(),SlotMode.Gcd),
        new(new SlotResolver_GCD_Dot(),SlotMode.Gcd),
        new(new SlotResolver_GCD_AOE(),SlotMode.Gcd),
        new(new SlotResolver_GCD_发炎(),SlotMode.Gcd),
        new(new SlotResolver_GCD_注药(),SlotMode.Gcd),




    };
   
 


    public Rotation Build(string settingFolder)
    {
        // 初始化设置
        SageSettings.Build(settingFolder);
        // 初始化QT （依赖了设置的数据）
        BuildQT();

        
        var rot = new Rotation(SlotResolvers)
        {
            TargetJob = Jobs.Sage,
            AcrType = AcrType.Normal,
            MinLevel = 0,
            MaxLevel = 100,
            Description = "Ayanotan的贤者ACR喵"+
                            "\n支持OkitaAyano喵"+
                              "\n支持OkitaAyano谢谢喵"+
                                "\n请务必去ACR设置里确认技能使用阈值,建议开启目标选择器"+
                                  "\n请不要开启GCD偏移或长臂猿等，可能会造成意料之外的卡手"
        }
       
        ;

        // 添加各种事件回调
        rot.SetRotationEventHandler(new SageRotationEventHandler());
        // 添加QT开关的时间轴行为
        rot.AddTriggerAction(new TriggerAction_QT());
        // 添加特殊技能序列
        rot.AddSlotSequences(SpecialSequence.Build());

        return rot;
    }

    // 声明当前要使用的UI的实例 示例里使用QT
    public static JobViewWindow QT { get; private set; }

    // 如果你不想用QT 可以自行创建一个实现IRotationUI接口的类
    public IRotationUI GetRotationUI()
    {
        return QT;
    }

    // 构造函数里初始化QT
    public void BuildQT()
    {
        
        // JobViewSave是AE底层提供的QT设置存档类 在你自己的设置里定义即可
        // 第二个参数是你设置文件的Save类 第三个参数是QT窗口标题
        QT = new JobViewWindow(SageSettings.Instance.JobViewSave, SageSettings.Instance.Save, "Ayanotan SGE");
        QT.SetUpdateAction(OnUIUpdate); // 设置QT中的Update回调 不需要就不设置

        //添加QT分页 第一个参数是分页标题 第二个是分页里的内容
        QT.AddTab("通用", DrawQtGeneral);
        QT.AddTab("Dev", DrawQtDev);


        //QT.AddQt(QTKey.爆发药, true);
        //QT.AddQt(QTKey.爆发, true);
        QT.AddQt(QTKey.心关, true);
        QT.AddQt(QTKey.AOE, true);
        QT.AddQt(QTKey.发炎, true);
        QT.AddQt(QTKey.Dot, true);
        QT.AddQt(QTKey.非均衡治疗, true);
        QT.AddQt(QTKey.能力技治疗, true);
        QT.AddQt(QTKey.整体论, true);
        QT.AddQt(QTKey.灵橡清汁, true);
        QT.AddQt(QTKey.输血, true);
        QT.AddQt(QTKey.泛输血, true);
        QT.AddQt(QTKey.白牛清汁, true);
        QT.AddQt(QTKey.康复, true);
        QT.AddQt(QTKey.群盾, true);
        QT.AddQt(QTKey.单盾, true);
        QT.AddQt(QTKey.拯救, true);
        QT.AddQt(QTKey.混合, true);
        QT.AddQt(QTKey.智慧之爱, true);
        QT.AddQt(QTKey.自生, true);
        QT.AddQt(QTKey.坚角清汁, true);
        QT.AddQt(QTKey.寄生清汁, true);
        QT.AddQt(QTKey.拉人, true);
        QT.AddQt(QTKey.魂灵风息, true); 
        QT.AddQt(QTKey.心神风息, true);



        QT.AddHotkey("群盾", new 均衡预后());
        QT.AddHotkey("给T套单盾", new 给T套单盾());
        QT.AddHotkey("给血量最低的单盾", new 给血量最低的单盾());
        QT.AddHotkey("手动拉人", new 手动拉人());
        QT.AddHotkey("三段LB救场！", new LB());


        //// 添加QT开关 第二个参数是默认值 (开or关) 第三个参数是鼠标悬浮时的tips
        //QT.AddQt(QTKey.UseBaseGcd, true, "是否使用基础的Gcd");
        //QT.AddQt(QTKey.Test1, true);
        //QT.AddQt(QTKey.Test2, false);
        //QT.AddQt(QTKey.UsePotion, false);

        //// 添加快捷按钮 (带技能图标)
        //QT.AddHotkey("战斗之声",
        //    new HotKeyResolver_NormalSpell(SpellsDefine.BattleVoice, SpellTargetType.Self));
        //QT.AddHotkey("失血",
        //    new HotKeyResolver_NormalSpell(SpellsDefine.Bloodletter, SpellTargetType.Target));
        //QT.AddHotkey("爆发药", new HotKeyResolver_Potion());
        //QT.AddHotkey("极限技", new HotKeyResolver_LB());

        /*
        // 这是一个自定义的快捷按钮 一般用不到
        // 图片路径是相对路径 基于AEAssist(C|E)NVersion/AEAssist
        // 如果想用AE自带的图片资源 路径示例: Resources/AE2Logo.png
        QT.AddHotkey("极限技", new HotkeyResolver_General("#自定义图片路径", () =>
        {
            // 点击这个图片会触发什么行为
            LogHelper.Print("你好");
        }));
        */
    }

    // 设置界面
    public void OnDrawSetting()
    {
        SageSettingUI.Instance.Draw();
    }
    public void OnUIUpdate()
    {
         
    }

    public void DrawQtGeneral(JobViewWindow jobViewWindow)
    {

        //if (ImGui.Checkbox("能力技只奶T", ref SageSettings.Instance.OnlyTank)) ;
        if (ImGui.Checkbox("发炎用于Boss走位", ref SageSettings.Instance.发炎走位)) ;
        ImGuiHelper.LeftInputInt("保留蛇刺数量（AOE时，可填0~3）", ref SageSettings.Instance.保留蛇刺数量);
        //if (ImGui.Checkbox("移动速度", ref SageSettings.Instance.移动速度开关)) ;
        //ImGuiHelper.LeftInputFloat("移动速度", ref SageSettings.Instance.移动速度);


    }

    public void DrawQtDev(JobViewWindow jobViewWindow)
    {

        if (ImGui.Button("检查"))
        {
            IBattleChara needheal;

            needheal = PartyHelper.CastableAlliesWithin30
                    .Where(r => r.CurrentHp > 0 && r.CurrentHpPercent() <= 0.6 && !r.NotInvulnerable())
                    .OrderBy(r => r.CurrentHpPercent())
                    .FirstOrDefault();
           
         
        }
     


    }


    public void Dispose()
    {
        // 释放需要释放的东西 没有就留空
    }

}
