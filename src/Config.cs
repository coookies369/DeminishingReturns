using BepInEx.Configuration;

public class Config
{
    public static ConfigEntry<bool> resetAfterQuota;
    public static ConfigEntry<float> dailyRegen;
    public static ConfigEntry<float> reductionMultiplier;

    public Config(ConfigFile cfg)
    {
        resetAfterQuota = cfg.Bind(
                "General",
                "ResetAfterQuota",
                false,
                "Reset all scrap reduction after each quota cycle"
        );
        dailyRegen = cfg.Bind(
                "General",
                "DailyRegen",
                0.25f,
                "How much of a moon's multiplier should be restored each day"
        );
        reductionMultiplier = cfg.Bind(
                "General",
                "ReductionMultiplier",
                1f,
                "Multiplier for scrap reduction. For example: if a moon has a 100% multiplier, 9 of 12 scrap is collected, and reductionMultiplier is 0.5, the moon's final multiplier will be 62.5% instead of 25%"
        );
    }
}