using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LCSoundTool;
using LobbyCompatibility.Attributes;
using LobbyCompatibility.Enums;
using UnityEngine;

namespace LCPagerMod;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.HardDependency)]
[LobbyCompatibility(CompatibilityLevel.Everyone, VersionStrictness.None)]
public class LCPagerMod : BaseUnityPlugin
{
    public static LCPagerMod Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }
    
    public static AudioClip newSound;
    
    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;

        Patch();
        newSound = SoundTool.GetAudioClip("yunixfanaccount-LCPagerMod", "pager.wav");

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
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