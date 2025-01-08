using GameNetcodeStuff;
using HarmonyLib;
using LCSoundTool;

namespace LCPagerMod.Patches;

[HarmonyPatch(typeof(TVScript))]
public class ExampleTVPatch
{
    [HarmonyPatch(nameof(TVScript.SwitchTVLocalClient))]
    [HarmonyPrefix]
    private static void SwitchTVPrefix(TVScript __instance)
    {
        /*
         *  When the method is called, the TV will be turning off when we want to
         *  turn the lights on and vice-versa. At that time, the TV's tvOn field
         *  will be the opposite of what it's doing, ie it'll be on when turning off.
         *  So, we want to set the lights to what the tv's state was
         *  when this method is called.
         */
        StartOfRound.Instance.shipRoomLights.SetShipLightsBoolean(__instance.tvOn);
        GameNetworkManager.Instance.localPlayerController.itemAudio.PlayOneShot(LCPagerMod.newSound, 1f);
        WalkieTalkie.TransmitOneShotAudio(GameNetworkManager.Instance.localPlayerController.itemAudio, LCPagerMod.newSound, 1f);
    }
}