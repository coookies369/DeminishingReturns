using HarmonyLib;
using LethalLevelLoader;

namespace DeminishingReturns.Patches;

public class LLLTerminalPatch
{
    internal static void Init()
    {
        DeminishingReturns.Harmony.Patch(
          AccessTools.Method(typeof(LethalLevelLoader.TerminalManager), "GetWeatherConditions"),
          postfix: new HarmonyMethod(typeof(Patches.LLLTerminalPatch), "addDeminishedWarning")
        );
    }

    [HarmonyPriority(0)]
    private static void addDeminishedWarning(ExtendedLevel extendedLevel, ref string __result)
    {
        float multiplier = DeminishingReturns.moonMultipliers.ContainsKey(extendedLevel.SelectableLevel.levelID) ? DeminishingReturns.moonMultipliers[extendedLevel.SelectableLevel.levelID] : 1.0f;
        __result = __result + (multiplier < 1.0 ? string.Format("{0:N0}%", multiplier * 100.0) : "");
    }
}