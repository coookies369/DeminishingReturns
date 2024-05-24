using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using LobbyCompatibility.Attributes;
using LobbyCompatibility.Enums;
using LethalModDataLib.Attributes;
using LethalModDataLib.Enums;
using LethalNetworkAPI;
using System.Collections.Generic;

namespace DeminishingReturns;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.HardDependency)]
[LobbyCompatibility(CompatibilityLevel.Everyone, VersionStrictness.Minor)]
[BepInDependency("MaxWasUnavailable.LethalModDataLib", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("LethalNetworkAPI", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("imabatby.lethallevelloader", BepInDependency.DependencyFlags.SoftDependency)]
public class DeminishingReturns : BaseUnityPlugin
{
    public static DeminishingReturns Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }

    public static Config MyConfig { get; internal set; }

    [ModData(SaveWhen.OnAutoSave, LoadWhen.OnLoad, SaveLocation.CurrentSave, ResetWhen.OnGameOver)]
    public static Dictionary<int, float> moonMultipliers = new Dictionary<int, float>();
    public static LethalNetworkVariable<Dictionary<int, float>> moonMultipliersNet = new LethalNetworkVariable<Dictionary<int, float>>(identifier: "moonMultipliers") { Value = moonMultipliers };

    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;

        MyConfig = new(base.Config);

        Patch();

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    internal static void Patch()
    {
        Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

        Logger.LogDebug("Patching...");

        Harmony.PatchAll();

        if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("imabatby.lethallevelloader"))
        {
            Patches.LLLTerminalPatch.Init();
        }

        Logger.LogDebug("Finished patching!");
    }

    internal static void Unpatch()
    {
        Logger.LogDebug("Unpatching...");

        Harmony?.UnpatchSelf();

        Logger.LogDebug("Finished unpatching!");
    }
}
