using HarmonyLib;
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
    private static List<GrabbableObject> new_scrap = new();
    private static int scrap_count;

    public static void Init()
    {
        syncMoonMultipliersClient.OnReceived += SyncMoonMultipliers;
    }

    private static void SyncMoonMultipliers(Dictionary<int, float> data)
    {
        Plugin.moonMultipliers = data;
    }

    [HarmonyPatch("StartGame")]
    [HarmonyPrefix]
    private static void ReduceScrapAmount(StartOfRound __instance)
    {
        if (__instance.IsServer && __instance.inShipPhase)
        {
            if (!Plugin.moonMultipliers.ContainsKey(__instance.currentLevel.levelID))
            {
                Plugin.moonMultipliers[__instance.currentLevel.levelID] = 1.0f;
            }
            RoundManager.Instance.scrapAmountMultiplier = Plugin.moonMultipliers[__instance.currentLevel.levelID];
        }
    }

    [HarmonyPatch("EndOfGame")]
    [HarmonyPrefix]
    private static void RecordScrapCount(StartOfRound __instance)
    {
        if (__instance.IsServer)
        {
            GrabbableObject[] array = Object.FindObjectsOfType<GrabbableObject>();

            scrap_count = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].itemProperties.isScrap)
                {
                    scrap_count++;
                }
            }

            new_scrap = new List<GrabbableObject>(RoundManager.Instance.scrapCollectedThisRound);
        }
    }

    [HarmonyPatch("PassTimeToNextDay")]
    [HarmonyPrefix]
    private static void SetMoonMultipliers(StartOfRound __instance)
    {
        if (__instance.IsServer)
        {
            var newMults = new Dictionary<int, float>();
            if (__instance.currentLevel.planetHasTime)
            {
                var keys = Plugin.moonMultipliers.Keys.ToList();
                foreach (var key in keys)
                {
                    var value = Plugin.moonMultipliers[key];
                    value += Config.dailyRegen.Value;
                    value = Mathf.Clamp(value, 0.0f, 1.0f);
                    newMults[key] = value;
                }

                var scrap_collected = 0;
                foreach (GrabbableObject scrap in new_scrap)
                {
                    if (scrap != null)
                    {
                        scrap_collected++;
                    }
                }

                Plugin.Logger.LogDebug($"Collected {scrap_collected} scrap of {scrap_count} total");
                newMults[__instance.currentLevel.levelID] -= (float)scrap_collected / scrap_count * Config.reductionMultiplier.Value;
                newMults[__instance.currentLevel.levelID] = Mathf.Clamp(newMults[__instance.currentLevel.levelID], 0.0f, 1.0f);
            }
            if (Config.resetAfterQuota.Value && (TimeOfDay.Instance.profitQuota - TimeOfDay.Instance.quotaFulfilled <= 0f || __instance.isChallengeFile))
            {
                newMults.Clear();
                Plugin.Logger.LogDebug("Cleared moon multipliers");
            }
            syncMoonMultipliersServer.SendAllClients(newMults);
        }
    }

    [HarmonyPatch("OnClientConnect")]
    [HarmonyPrefix]
    private static void OnClientConnect(ulong clientId)
    {
        syncMoonMultipliersServer.SendClient(Plugin.moonMultipliers, clientId);
    }
}