using HarmonyLib;
using BepInEx.Configuration;

namespace DeminishingReturns.Patches;

[HarmonyPatch(typeof(StartOfRound))]
public class StartOfRoundPatch
{
    [HarmonyPatch("StartGame")]
    [HarmonyPrefix]
    private static void reduceScrapAmount(StartOfRound __instance)
    {
        if (__instance.IsServer && __instance.inShipPhase)
        {
            if (DeminishingReturns.recentMoonsSave.Contains(__instance.currentLevel.levelID))
            {
                RoundManager.Instance.scrapAmountMultiplier = Config.deminishedScrapMultiplier.Value;
            }
            else
            {
                RoundManager.Instance.scrapAmountMultiplier = 1f;
            }
        }
    }

    [HarmonyPatch("EndOfGame")]
    [HarmonyPrefix]
    private static void recordRecentMoons(StartOfRound __instance)
    {
        if (__instance.IsServer)
        {
            if (__instance.currentLevel.PlanetName == "71 Gordion")
            {
                DeminishingReturns.Logger.LogDebug("Not marking this moon as recent because it is the company building");
            }
            else
            {
                DeminishingReturns.recentMoonsSave.Enqueue(__instance.currentLevel.levelID);
            }
            while (DeminishingReturns.recentMoonsSave.Count > Config.recentMoonCount.Value)
            {
                DeminishingReturns.recentMoonsSave.Dequeue();
            }
            if (Config.resetAfterQuota.Value && ((float)(TimeOfDay.Instance.profitQuota - TimeOfDay.Instance.quotaFulfilled) <= 0f || __instance.isChallengeFile))
            {
                DeminishingReturns.recentMoonsSave.Clear();
                DeminishingReturns.Logger.LogDebug("Cleared recent moons list");
            }
            DeminishingReturns.recentMoons.Value = DeminishingReturns.recentMoonsSave;
        }
    }
}
