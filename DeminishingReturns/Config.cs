using BepInEx.Configuration;

public class Config
{
        public static ConfigEntry<float> deminishedScrapMultiplier;
        public static ConfigEntry<int> recentMoonCount;
        public static ConfigEntry<bool> resetAfterQuota;

        public Config(ConfigFile cfg)
        {
                deminishedScrapMultiplier = cfg.Bind(
                        "General",
                        "DeminishedScrapMultiplier",
                        0.5f,
                        "Multiplier for scrap amounts on deminished moons"
                );
                recentMoonCount = cfg.Bind(
                        "General",
                        "RecentMoonCount",
                        3,
                        "How many days before a recent moon is \"forgotten\""
                );
                resetAfterQuota = cfg.Bind(
                        "General",
                        "ResetAfterQuota",
                        true,
                        "If the recent moons list should be cleared after a quota cycle"
                );
        }
}