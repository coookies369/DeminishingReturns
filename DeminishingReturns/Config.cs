using BepInEx.Configuration;

public class Config
{
        public static ConfigEntry<bool> resetAfterQuota;
        public static ConfigEntry<float> dailyRegen;

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
        }
}