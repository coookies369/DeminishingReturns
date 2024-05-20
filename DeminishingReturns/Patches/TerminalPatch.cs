using HarmonyLib;
using System.Text.RegularExpressions;

namespace DeminishingReturns.Patches;

[HarmonyPatch(typeof(Terminal))]
public class TerminalPatch
{
    [HarmonyPatch("TextPostProcess")]
    [HarmonyPrefix]
    private static void addDeminishedWarning(ref string modifiedDisplayText, TerminalNode node, Terminal __instance)
    {
        int num = modifiedDisplayText.Split("[planetTime]").Length - 1;
        if (num > 0)
        {
            Regex regex = new Regex(Regex.Escape("[planetTime]"));
            for (int i = 0; i < num && __instance.moonsCatalogueList.Length > i; i++)
            {
                string replacement = ((GameNetworkManager.Instance.isDemo && __instance.moonsCatalogueList[i].lockedForDemo) ? "(Locked)" : ((__instance.moonsCatalogueList[i].currentWeather != LevelWeatherType.None) ? ("(" + __instance.moonsCatalogueList[i].currentWeather.ToString() + ")") : "") + ((DeminishingReturns.recentMoons.Value.Contains(__instance.moonsCatalogueList[i].levelID)) ? "[Reduced]" : ""));
                replacement = replacement.Replace(")[", ") [");
                modifiedDisplayText = regex.Replace(modifiedDisplayText, replacement, 1);
            }
        }
        try
        {
            if (node.displayPlanetInfo != -1)
            {
                string replacement = ((StartOfRound.Instance.levels[node.displayPlanetInfo].currentWeather != LevelWeatherType.None) ? (StartOfRound.Instance.levels[node.displayPlanetInfo].currentWeather.ToString().ToLower() ?? "") : "mild weather");
                modifiedDisplayText = modifiedDisplayText.Replace("[currentPlanetTime]", replacement);
                if (DeminishingReturns.recentMoons.Value.Contains(StartOfRound.Instance.levels[node.displayPlanetInfo].levelID))
                {
                    modifiedDisplayText = modifiedDisplayText.Insert(modifiedDisplayText.LastIndexOf('\n', modifiedDisplayText.Length - 4), "\nThis moon has been visited recently; expect less scrap.\n");
                }
            }
        }
        catch
        {
            DeminishingReturns.Logger.LogDebug($"Exception occured on terminal while setting node planet info; current node displayPlanetInfo:{node.displayPlanetInfo}");
        }
    }
}
