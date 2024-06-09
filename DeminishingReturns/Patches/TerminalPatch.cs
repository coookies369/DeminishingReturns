using HarmonyLib;
using System.Text.RegularExpressions;

namespace DeminishingReturns.Patches;

[HarmonyPatch(typeof(Terminal))]
public class TerminalPatch
{
    [HarmonyPatch("TextPostProcess")]
    [HarmonyPrefix]
    private static void AddDeminishedWarning(ref string modifiedDisplayText, Terminal __instance)
    {
        int num = modifiedDisplayText.Split("[planetTime]").Length - 1;
        if (num > 0)
        {
            Regex regex = new(Regex.Escape("[planetTime]"));
            for (int i = 0; i < num && __instance.moonsCatalogueList.Length > i; i++)
            {
                float multiplier = DeminishingReturns.moonMultipliers.ContainsKey(__instance.moonsCatalogueList[i].levelID) ? DeminishingReturns.moonMultipliers[__instance.moonsCatalogueList[i].levelID] : 1.0f;
                string replacement = (GameNetworkManager.Instance.isDemo && __instance.moonsCatalogueList[i].lockedForDemo) ? "(Locked)" : ((__instance.moonsCatalogueList[i].currentWeather != LevelWeatherType.None) ? ("(" + __instance.moonsCatalogueList[i].currentWeather.ToString() + ") ") : "") + (multiplier < 1.0 ? string.Format("{0:N0}%", multiplier * 100.0) : "");
                modifiedDisplayText = regex.Replace(modifiedDisplayText, replacement, 1);
            }
        }
    }
}
