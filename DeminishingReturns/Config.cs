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
                "Multiplier for scrap reduction. For example: if 5 of 10 scrap is collected and this value is 0.5, the penalty will be 25% instead of 50%"
        );
    }
}