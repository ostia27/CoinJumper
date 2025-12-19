using CoinJumper.Scripts;
using HarmonyLib;
using UnityEngine;

namespace CoinJumper.Patches;

[HarmonyPatch]
public static class CoinPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Coin), "GetDeleted")]
    public static void patch_GetDeleted(Coin __instance)
    {
        var bouncy = __instance.GetComponent<Bouncy>();
        if (bouncy) { Object.Destroy(bouncy); }
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Coin), "Start")]
    private static void patch_Start(Coin __instance)
    {
        var bouncy = __instance.GetOrAddComponent<Bouncy>();
        bouncy.audioObjectPrefab = __instance.coinHitSound;
        
        bouncy.OnBounce += (bounceCount) =>
        {
            if (Plugin.MaxBouncesValue > 0 && bounceCount > Plugin.MaxBouncesValue)
            {
                __instance.GetDeleted();
                return; 
            }
            
            __instance.CancelInvoke(nameof(Coin.GetDeleted));
            __instance.Invoke(nameof(Coin.GetDeleted), 5f);
        };
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Coin), "OnCollisionEnter")]
    private static bool patch_OnCollisionEnter(Coin __instance, Collision collision)
    {
        return false;
    }
}