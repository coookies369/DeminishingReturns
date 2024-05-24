using HarmonyLib;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            if (!DeminishingReturns.moonMultipliers.ContainsKey(__instance.currentLevel.levelID))
            {
                DeminishingReturns.moonMultipliers[__instance.currentLevel.levelID] = 1.0f;
            }
            RoundManager.Instance.scrapAmountMultiplier = DeminishingReturns.moonMultipliers[__instance.currentLevel.levelID];
        }
    }

    [HarmonyPatch("EndOfGame")]
    [HarmonyPrefix]
    private static void setMoonMultipliers(StartOfRound __instance)
    {
        if (__instance.IsServer)
        {
            if (__instance.currentLevel.planetHasTime)
            {
                var keys = DeminishingReturns.moonMultipliers.Keys.ToList();
                foreach (var key in keys)
                {
                    var value = DeminishingReturns.moonMultipliers[key];
                    value += Config.dailyRegen.Value;
                    value = Mathf.Clamp(value, 0.0f, 1.0f);
                    DeminishingReturns.moonMultipliers[key] = value;
                }
                if (!__instance.allPlayersDead)
                {
                    DeminishingReturns.moonMultipliers[__instance.currentLevel.levelID] -= (float)RoundManager.Instance.valueOfFoundScrapItems / RoundManager.Instance.totalScrapValueInLevel;
                    DeminishingReturns.moonMultipliers[__instance.currentLevel.levelID] = Mathf.Clamp(DeminishingReturns.moonMultipliers[__instance.currentLevel.levelID], 0.0f, 1.0f);
                }
            }
            if (Config.resetAfterQuota.Value && ((float)(TimeOfDay.Instance.profitQuota - TimeOfDay.Instance.quotaFulfilled) <= 0f || __instance.isChallengeFile))
            {
                DeminishingReturns.moonMultipliers.Clear();
                DeminishingReturns.Logger.LogDebug("Cleared moon multipliers");
            }
            DeminishingReturns.moonMultipliersNet.Value = DeminishingReturns.moonMultipliers; //This triggers a sync for the network variable
        }
    }
}