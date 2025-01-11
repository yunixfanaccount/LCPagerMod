using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using LCSoundTool;
using LethalCompanyInputUtils.Api;
using LethalCompanyInputUtils.BindingPathEnums;
using LethalNetworkAPI;
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

    public static LethalClientMessage<ulong> PagerRingMessage;
    
    public static LethalServerMessage<ulong> PagerRingMessageServer;
    
    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;

        Patch();
        NewSound = SoundTool.GetAudioClip("yunixfanaccount-LCPagerMod", "pager.wav");
        InputActionsInstance = new PagerInput();
        InputActionsInstance.PagerKey.performed += PagerKeyOnPress;
        
        PagerRingMessage = new LethalClientMessage<ulong>(identifier: "PagerSoundPlay");
        PagerRingMessageServer = new LethalServerMessage<ulong>(identifier: "PagerSoundPlay");
        PagerRingMessage.OnReceived += ReceiveFromServer;
        PagerRingMessage.OnReceivedFromClient += ReceiveFromClient;
        PagerRingMessageServer.OnReceived += ServerReceiveFromClient;
        
        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    private void ServerReceiveFromClient(ulong data, ulong clientID)
    {
        Logger.LogWarning($"server got response from client: {data} with id: {clientID}");
        PagerRingMessageServer.SendAllClients(clientID);
    }

    private void ReceiveFromClient(ulong data, ulong clientID)
    {
        Logger.LogWarning($"client got response from client: {data} with id: {clientID}");
    }

    private void ReceiveFromServer(ulong data)
    {
        Logger.LogWarning($"client got response from server: {data}");
        PlayerControllerB player = data.GetPlayerController();
        // player.itemAudio.PlayOneShot(LCPagerMod.NewSound, 1f);
        // player.currentVoiceChatAudioSource.PlayOneShot(LCPagerMod.NewSound, 1f);
        player.itemAudio.clip = LCPagerMod.NewSound;
        player.itemAudio.Play();
        RoundManager.Instance.PlayAudibleNoise(player.oldPlayerPosition, 16f, 1f);

        
        WalkieTalkie.TransmitOneShotAudio(player.itemAudio, LCPagerMod.NewSound, 1f);
    }

    private void PagerKeyOnPress(InputAction.CallbackContext obj)
    {
        if (!obj.performed) return;
        PagerRingMessage.SendServer(0);
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