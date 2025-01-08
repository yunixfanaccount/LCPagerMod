using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LCSoundTool;
using LethalCompanyInputUtils.Api;
using LethalCompanyInputUtils.BindingPathEnums;
using LobbyCompatibility.Attributes;
using LobbyCompatibility.Enums;
using UnityEngine;
using UnityEngine.InputSystem;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace LCPagerMod;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.rune580.LethalCompanyInputUtils", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.HardDependency)]
[LobbyCompatibility(CompatibilityLevel.Everyone, VersionStrictness.None)]

public class LCPagerMod : BaseUnityPlugin
{
    public static LCPagerMod Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }
    
    public static AudioClip NewSound;

    internal static PagerInput InputActionsInstance;
    
    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;

        Patch();
        NewSound = SoundTool.GetAudioClip("yunixfanaccount-LCPagerMod", "pager.wav");
        InputActionsInstance = new PagerInput();
        InputActionsInstance.PagerKey.performed += PagerKeyOnPress;
        
        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }
    
    private void PagerKeyOnPress(InputAction.CallbackContext obj)
    {
        if (!obj.performed) return;
        GameNetworkManager.Instance.localPlayerController.itemAudio.PlayOneShot(LCPagerMod.NewSound, 1f);
        WalkieTalkie.TransmitOneShotAudio(GameNetworkManager.Instance.localPlayerController.itemAudio, LCPagerMod.NewSound, 1f);
    }

    internal static void Patch()
    {
        Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

        Logger.LogDebug("Patching...");

        Harmony.PatchAll();

        Logger.LogDebug("Finished patching!");
    }

    internal static void Unpatch()
    {
        Logger.LogDebug("Unpatching...");

        Harmony?.UnpatchSelf();

        Logger.LogDebug("Finished unpatching!");
    }
}

public class PagerInput : LcInputActions 
{
    [InputAction("<Keyboard>/v", Name = "PagerRing")]
    public InputAction PagerKey { get; set; }
}