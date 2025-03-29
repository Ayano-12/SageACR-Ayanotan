using AEAssist.Helper;
using AEAssist.MemoryApi;
using Ayanotan.WhiteMage.Setting;
using Dalamud.Game;
using Dalamud.Hooking;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Signatures;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Ayanotan.Sage.Himitu
{
    public unsafe class SpeedHack 
    {
        private SageSettings _sageSettings;
        private string hookMessage;
        private delegate float SpeedDelegate(IntPtr a1);
        private readonly ISigScanner _sigScanner;
        private readonly IGameInteropProvider _gameInteropProvider;  // 添加 IGameInteropProvider

        [Signature("40 ? 48 ? ? ? 48 ? ? 48 ? ? ? 48 ? ? ff 90 ? ? ? ? 48 ? ? 75 ? f3 ? ? ? ? ? ? ?")]
        private Hook<SpeedDelegate> speedHook;

        // 修改构造函数，同时注入 ISigScanner 和 IGameInteropProvider
        public SpeedHack(ISigScanner sigScanner, IGameInteropProvider gameInteropProvider)
        {
            _sigScanner = sigScanner;
            _gameInteropProvider = gameInteropProvider;
        }

        public SpeedHack()
        {
            try
            {
                // 使用注入的 _gameInteropProvider 实例
                this.speedHook = _gameInteropProvider.HookFromAddress<SpeedDelegate>(
                    _sigScanner.ScanText("40 ? 48 ? ? ? 48 ? ? 48 ? ? ? 48 ? ? ff 90 ? ? ? ? 48 ? ? 75 ? f3 ? ? ? ? ? ? ?"),
                    new SpeedDelegate(this.SpeedDetour),
                    (IGameInteropProvider.HookBackend)0);
                _gameInteropProvider.InitializeFromAttributes(this);
                this.speedHook?.Enable();


            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
            }
        }

        public void SpeedHook()
        {
            try
            {
                // 使用注入的 _gameInteropProvider 实例
                this.speedHook = _gameInteropProvider.HookFromAddress<SpeedDelegate>(
                    _sigScanner.ScanText("40 ? 48 ? ? ? 48 ? ? 48 ? ? ? 48 ? ? ff 90 ? ? ? ? 48 ? ? 75 ? f3 ? ? ? ? ? ? ?"),
                    new SpeedDelegate(this.SpeedDetour),
                    (IGameInteropProvider.HookBackend)0);
                _gameInteropProvider.InitializeFromAttributes(this);
                this.speedHook.Enable();

                //this.speedHook.Enable();
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);
            }
        }
        public void Dispose()
        {
            this.speedHook.Dispose();
        }


        // Detour method for the speed hook
        private float SpeedDetour(IntPtr a1)
        {
            // Call the original function
            float originalValue = this.speedHook.Original(a1);

            // Only modify if injected/enabled
            if (!SageSettings.Instance.移动速度开关)
                return originalValue;
           

            // Add our speed value
            float result = originalValue + SageSettings.Instance.移动速度;

            // Update hook message
            hookMessage = $"{originalValue} + {SageSettings.Instance.移动速度:F2}";

            return result;
        }
        
    }
}
