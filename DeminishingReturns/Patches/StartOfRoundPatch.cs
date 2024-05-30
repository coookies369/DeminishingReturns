using HarmonyLib;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LethalNetworkAPI;

namespace DeminishingReturns.Patches;

[HarmonyPatch(typeof(StartOfRound))]
public class StartOfRoundPatch
{
    public static LethalServerMessage<Dictionary<int, float>> syncMoonMultipliersServer = new("moonMultipliers");
    public static LethalClientMessage<Dictionary<int, float>> syncMoonMultipliersClient = new("moonMultipliers");

    public static void Init()
    {
        syncMoonMultipliersClient.OnReceived += SyncMoonMultipliers;
    }

    private static void SyncMoonMultipliers(Dictionary<int, float> data)
    {
        DeminishingReturns.moonMultipliers = data;
    }

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
            var newMults = new Dictionary<int, float>();
            if (__instance.currentLevel.planetHasTime)
            {
                var keys = DeminishingReturns.moonMultipliers.Keys.ToList();
                foreach (var key in keys)
                {
                    var value = DeminishingReturns.moonMultipliers[key];
                    value += Config.dailyRegen.Value;
                    value = Mathf.Clamp(value, 0.0f, 1.0f);
                    newMults[key] = value;
                }
                if (!__instance.allPlayersDead)
                {
                    GrabbableObject[] array = UnityEngine.Object.FindObjectsOfType<GrabbableObject>();
                    var scrap_collected = RoundManager.Instance.scrapCollectedThisRound.Count;
                    var scrap_count = scrap_collected; //The below for loop only counts scrap outside of the ship, so we also include scrap collected this round for the total
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i].itemProperties.isScrap && !array[i].isInShipRoom)
                        {
                            //Because we just count scrap outside of the ship, players can reduce the penalty by dropping some scrap outside of the ship.
                            //Good news: that's stupid and nobody will do that
                            scrap_count++;
                        }
                    }
                    DeminishingReturns.Logger.LogDebug($"Collected {scrap_collected} scrap of {scrap_count} total");
                    newMults[__instance.currentLevel.levelID] -= (float)scrap_collected / scrap_count;
                    newMults[__instance.currentLevel.levelID] = Mathf.Clamp(newMults[__instance.currentLevel.levelID], 0.0f, 1.0f);
                }
            }
            if (Config.resetAfterQuota.Value && ((float)(TimeOfDay.Instance.profitQuota - TimeOfDay.Instance.quotaFulfilled) <= 0f || __instance.isChallengeFile))
            {
                newMults.Clear();
                DeminishingReturns.Logger.LogDebug("Cleared moon multipliers");
            }
            syncMoonMultipliersServer.SendAllClients(newMults);
        }
    }

    [HarmonyPatch("OnClientConnect")]
    [HarmonyPrefix]
    private static void OnClientConnect(ulong clientId)
    {
        syncMoonMultipliersServer.SendClient(DeminishingReturns.moonMultipliers, clientId);
    }
}