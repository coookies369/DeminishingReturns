using HarmonyLib;
using LethalLevelLoader;

namespace DeminishingReturns.Patches;

public class LLLTerminalPatch
{
    internal static void Init()
    {
        Plugin.Harmony.Patch(
          AccessTools.Method(typeof(TerminalManager), "GetWeatherConditions"),
          postfix: new HarmonyMethod(typeof(LLLTerminalPatch), "addDeminishedWarning")
        );
    }

    [HarmonyPriority(0)]
    private static void AddDeminishedWarning(ExtendedLevel extendedLevel, ref string __result)
    {
        float multiplier = Plugin.moonMultipliers.ContainsKey(extendedLevel.SelectableLevel.levelID) ? Plugin.moonMultipliers[extendedLevel.SelectableLevel.levelID] : 1.0f;
        __result += multiplier < 1.0 ? string.Format("{0:N0}%", multiplier * 100.0) : "";
    }
}