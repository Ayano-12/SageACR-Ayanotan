using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AEAssist.Helper;
using AEAssist.Verify;
using AEAssist;
using Dalamud.Game;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using Ayanotan.WhiteMage.Setting;


namespace Ayanotan.Sage.Himitu
{
    internal static class HackingXIV  //感谢贾老板和棍哥的代码
    {
        internal enum PatchType
        {
            MovePermission,
            SkillPostActionMove,
            ActionRange,
            SpeedUP
        }

        internal static class Hook
        {
            private static readonly Dictionary<PatchType, IDisposable> _activeHooks = new Dictionary<PatchType, IDisposable>();
            private static Hook<Hook.MovePermissionDelegate>? _movePermissionHook;
            private static readonly Hook.MovePermissionDelegate _movePermissionDetour = new Hook.MovePermissionDelegate(Hook.MovePermissionDetour);
            private static Hook<Hook.SkillPostActionMoveDelegate>? _skillPostActionMoveHook;
            private static readonly Hook.SkillPostActionMoveDelegate _skillPostActionMoveDetour = new Hook.SkillPostActionMoveDelegate(Hook.SkillPostActionMoveDetour);
            private static Hook<Hook.ActionRangeDelegate>? _actionRangeHook;
            private static readonly Hook.ActionRangeDelegate _actionRangeDetour = new Hook.ActionRangeDelegate(Hook.ActionRangeDetour);
            private static Hook<Hook.SpeedUPDelegate>? _SpeedUPHook;
            private static readonly Hook.SpeedUPDelegate _SpeedUPDetour = new Hook.SpeedUPDelegate(Hook.SpeedUPDetour);
            private static readonly Dictionary<PatchType, Hook.PatchMeta> _patchMetas = new Dictionary<PatchType, Hook.PatchMeta>()
            {
                {
                    PatchType.SpeedUP,
                    new Hook.PatchMeta("移动速度", "40 ?? 48 ?? ?? ?? 48 ?? ?? 48 ?? ?? ?? 48 ?? ?? ff 90 ?? ?? ?? ?? 48 ?? ?? 75 ?? f3 ?? ?? ?? ?? ?? ?? ??")
                },
                {
                    PatchType.MovePermission,
                    new Hook.PatchMeta("强制移动", "e8 ?? ?? ?? ?? 84 ?? 74 ?? 48 c7 05")
                },
                {
                    PatchType.SkillPostActionMove,
                    new Hook.PatchMeta("技能后摇可移动", "E8 ?? ?? ?? ?? C6 83 ?? ?? ?? ?? ?? E9 96 00 ?? ??")
                },
                {
                    PatchType.ActionRange,
                    new Hook.PatchMeta("技能距离增强", "48 89 5C 24 ?? 57 48 ?? ?? ?? 48 ?? ?? ?? ?? ?? ?? 8B ?? 0F 29 74 24 20")
                }
            };

            private static IGameInteropProvider GameInteropProvider => ECHelper.Hook;

            private static ISigScanner SigScanner => ECHelper.SigScanner;

            public static void EnablePatch(PatchType patchType)
            {
                if (Hook._activeHooks.ContainsKey(patchType))
                    return;
                Hook.PatchMeta patchMeta = Hook._patchMetas[patchType];
                if (patchMeta.RequiredVIPLevel != null && Share.VIP.Level < patchMeta.RequiredVIPLevel)
                {
                    DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
                    interpolatedStringHandler.AppendFormatted(patchMeta.Name);
                    interpolatedStringHandler.AppendLiteral("需要");
                    interpolatedStringHandler.AppendFormatted<VIPLevel>(patchMeta.RequiredVIPLevel);
                    interpolatedStringHandler.AppendLiteral("权限");
                    LogHelper.Print(interpolatedStringHandler.ToStringAndClear());
                }
                else
                {
                    try
                    {
                        IntPtr num = Hook.SigScanner.ScanText(patchMeta.MemorySignature);
                        if (num == IntPtr.Zero)
                        {
                            LogHelper.Print("无法找到方法 " + patchMeta.Name + " 的内存地址");
                        }
                        else
                        {
                            switch (patchType)
                            {
                                case PatchType.MovePermission:
                                    Hook._movePermissionHook = Hook.GameInteropProvider.HookFromAddress<Hook.MovePermissionDelegate>(num, Hook._movePermissionDetour, (IGameInteropProvider.HookBackend)0);
                                    Hook._movePermissionHook.Enable();
                                    Hook._activeHooks[patchType] = (IDisposable)Hook._movePermissionHook;
                                    break;
                                case PatchType.SkillPostActionMove:
                                    Hook._skillPostActionMoveHook = Hook.GameInteropProvider.HookFromAddress<Hook.SkillPostActionMoveDelegate>(num, Hook._skillPostActionMoveDetour, (IGameInteropProvider.HookBackend)0);
                                    Hook._skillPostActionMoveHook.Enable();
                                    Hook._activeHooks[patchType] = (IDisposable)Hook._skillPostActionMoveHook;
                                    break;
                                case PatchType.ActionRange:
                                    Hook._actionRangeHook = Hook.GameInteropProvider.HookFromAddress<Hook.ActionRangeDelegate>(num, Hook._actionRangeDetour, (IGameInteropProvider.HookBackend)0);
                                    Hook._actionRangeHook.Enable();
                                    Hook._activeHooks[patchType] = (IDisposable)Hook._actionRangeHook;
                                    break;
                                case PatchType.SpeedUP:
                                    Hook._SpeedUPHook = Hook.GameInteropProvider.HookFromAddress<Hook.SpeedUPDelegate>(num, Hook._SpeedUPDetour, (IGameInteropProvider.HookBackend)0);
                                    Hook._SpeedUPHook.Enable();
                                    Hook._activeHooks[patchType] = (IDisposable)Hook._SpeedUPHook;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(patchType), (object)patchType, (string)null);
                            }
                            LogHelper.Print(patchMeta.Name + "已开启");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("启用" + patchMeta.Name + "时发生错误: " + ex.Message);
                    }
                }
            }

            public static void DisablePatch(PatchType patchType)
            {
                IDisposable disposable;
                if (!Hook._activeHooks.TryGetValue(patchType, out disposable))
                    return;
                try
                {
                    disposable.Dispose();
                    Hook._activeHooks.Remove(patchType);
                    switch (patchType)
                    {
                        case PatchType.MovePermission:
                            Hook._movePermissionHook = (Hook<Hook.MovePermissionDelegate>)null;
                            break;
                        case PatchType.SkillPostActionMove:
                            Hook._skillPostActionMoveHook = (Hook<Hook.SkillPostActionMoveDelegate>)null;
                            break;
                        case PatchType.ActionRange:
                            Hook._actionRangeHook = (Hook<Hook.ActionRangeDelegate>)null;
                            break;
                        case PatchType.SpeedUP:
                            Hook._SpeedUPHook = (Hook<Hook.SpeedUPDelegate>)null;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(patchType), (object)patchType, (string)null);
                    }
                    LogHelper.Print(Hook._patchMetas[patchType].Name + "已关闭");
                }
                catch (Exception ex)
                {
                    LogHelper.Error("禁用" + Hook._patchMetas[patchType].Name + "时发生错误: " + ex.Message);
                }
            }

            private static IntPtr MovePermissionDetour(
              IntPtr arg1,
              uint arg2,
              int arg3,
              int arg4)
            {
                var p_ids = Enumerable.Range(96, 4).Select(x => (uint)x)
                .Concat(new uint[] { 0x3E9, 0x3EE, 0x3EF, 0x3F0 }).ToList();
                if (p_ids.Contains(arg2))
                {
                    return 1;
                }
                return Hook._movePermissionHook.Original(arg1,arg2,arg3,arg4);
            }

            private static long SkillPostActionMoveDetour(long arg1) => arg1;

            private static float SpeedUPDetour(IntPtr arg1)
            {
                return Hook._SpeedUPHook.Original(arg1) + SageSettings.Instance.加速量;
            }

            private static float ActionRangeDetour(uint actionId)
            {
                return Hook._actionRangeHook.Original(actionId) + 3f;
            }

            private delegate IntPtr MovePermissionDelegate(
              IntPtr arg1,
              uint arg2,
              int arg3,
              int arg4);

            private delegate long SkillPostActionMoveDelegate(long arg1);

            private delegate float SpeedUPDelegate(IntPtr arg1);

            private delegate float ActionRangeDelegate(uint actionId);

            private class PatchMeta(string name, string memorySignature, VIPLevel requiredVipLevel = 0)
            {
                public string Name { get; } = name;

                public string MemorySignature { get; } = memorySignature;

                public VIPLevel RequiredVIPLevel { get; } = requiredVipLevel;
            }
        }
    }

}

