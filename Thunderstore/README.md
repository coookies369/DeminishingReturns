# Deminishing Returns

This mod, which I apparently spelled wrong, is a mod that punishes players for going to the same moons too frequently. When leaving a moon, its scrap multiplier will be reduced based on how much scrap you've collected.

![Catalog](https://raw.githubusercontent.com/coookies369/DeminishingReturns/main/Images/Catalog.png)

Because going to the same moon repeatedly will reduce it's scrap count, paid moons become less worthwhile. Consider a mod to adjust their prices [such as this one](https://thunderstore.io/c/lethal-company/p/coookies369/MoonCostMultiplier/) (made by me :3)

## Compatibility

The mod is now compatible with LethalLevelLoader. It is not compatible with TerminalFormatter, but it will work anyway if LethalLevelLoader is also installed.
This mod is incompatible with other mods that have scrap multipliers, such as MeteoMultiplier.

## Configs

* **ResetAfterQuota:** If scrap reductions on moons should be reset after each quota cycle. *Default: false*
* **dailyRegen:** How much each moons scrap reduction should recover each day. *Default: 0.25*
* **reductionMultiplier** Multiplier for scrap reduction. For example: if a moon has a 100% multiplier, 9 of 12 scrap is collected, and reductionMultiplier is 0.5, the moon's final multiplier will be 62.5% instead of 25%. *Default: 1*
