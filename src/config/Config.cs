using HarmonyLib;
using KMod;
using Newtonsoft.Json;
using PeterHan.PLib;
using PeterHan.PLib.Options;
using UnityEngine;

namespace DarknessNotIncluded
{
  [JsonObject(MemberSerialization.OptOut)]
  [ModInfo("https://github.com/nevir/oni-darkness-not-excluded")]
  [ConfigFile(SharedConfigLocation: true)]
  class Config : SingletonOptions<Config>
  {
    public static void Initialize(UserMod2 mod)
    {
      new POptions().RegisterOptions(mod, typeof(Config));
    }

    // Darkness

    [Option("Lux threshold", "At what lux should a tile be fully visible?", "Darkness")]
    public int fullyVisibleLuxThreshold { get; set; }

    [Option("Tooltip blocked by darkness", "Whether the tooltip should not show information if pointing at a dark tile.", "Darkness")]
    public bool selectToolBlockedByDarkness { get; set; }

    [Option("Darkness grace period (cycles)", "How many cycles should it take to go from no darkness to maximum darkness?", "Darkness")]
    public float gracePeriodCycles { get; set; }

    [Option("Initial darkness level", "How dark should 0 lux tiles be at the start of the game?\n0 = pitch black, 255 = fully visible", "Darkness")]
    [Limit(0, 255)]
    public int initialFogLevel { get; set; }

    [Option("Maximum darkness level", "How dark should 0 lux tiles be after the initial grace period?\n0 = pitch black, 255 = fully visible", "Darkness")]
    [Limit(0, 255)]
    public int minimumFogLevel { get; set; }

    // Light Bonuses

    [Option("Decor bonus threshold (lux)", "At what lux should decor get a bonus for being well lit?", "Light Bonuses")]
    public int decorBonusThresholdLux { get; set; }

    [Option("Lux required for Lit Workspace bonus", "The minimum amount of lux before a Duplicant will receive the Lit Workspace speed bonus", "Light Bonuses")]
    public int litWorkspaceLux { get; set; }

    // Darkness Penalties

    [Option("Maximum lux tolerated while sleeping", "The maximum lux that a dupe can handle before being rudely woken up.", "Darkness Penalties")]
    public int maxSleepingLux { get; set; }

    [DynamicOption(typeof(MinionEffectsConfigEntry), "Darkness Penalties")]
    [Option("Configuration for dupe effects", "Configuration for dupe effects", "Darkness Penalties")]
    public MinionEffectsConfig minionEffectsConfig { get; set; }

    // Duplicant Lights

    [Option("Disable lights around sleeping dupes", "Whether dupes should turn their lights off when entering a bedroom.", "Duplicant Lights")]
    public bool disableDupeLightsInBedrooms { get; set; }

    [Option("Disable lights in lit areas", "Whether dupes should turn their lights off when entering an area with at least the same level of brightness as their light.", "Duplicant Lights")]
    public bool disableDupeLightsInLitAreas { get; set; }

    [DynamicOption(typeof(MinionLightingConfigEntry), "Duplicant Lights")]
    [Option("Configuration for dupe lights", "Configuration for dupe lights", "Duplicant Lights")]
    public MinionLightingConfig minionLightingConfig { get; set; }

    public Config()
    {
      // Darkness
      initialFogLevel = 150;
      minimumFogLevel = 40;
      gracePeriodCycles = 3.0f;
      fullyVisibleLuxThreshold = 1000;
      decorBonusThresholdLux = 1000;
      selectToolBlockedByDarkness = true;

      // Duplicant Behavior
      disableDupeLightsInLitAreas = true;
      disableDupeLightsInBedrooms = true;
      maxSleepingLux = 400;
      litWorkspaceLux = 1000;

      minionLightingConfig = new MinionLightingConfig {
        { MinionLightType.Intrinsic, new MinionLightingConfig.LightConfig(true,  200,  2, MinionLightShape.Pill,         Color.white) },
        { MinionLightType.Mining1,   new MinionLightingConfig.LightConfig(true,  600,  3, MinionLightShape.DirectedCone, TUNING.LIGHT2D.LIGHT_YELLOW)},
        { MinionLightType.Mining2,   new MinionLightingConfig.LightConfig(true,  700,  4, MinionLightShape.DirectedCone, TUNING.LIGHT2D.LIGHT_YELLOW)},
        { MinionLightType.Mining3,   new MinionLightingConfig.LightConfig(true,  800,  5, MinionLightShape.DirectedCone, Color.white)},
        { MinionLightType.Mining4,   new MinionLightingConfig.LightConfig(true,  1000, 6, MinionLightShape.DirectedCone, Color.white)},
        { MinionLightType.Science,   new MinionLightingConfig.LightConfig(true,  600,  3, MinionLightShape.Pill,         Color.white)},
        { MinionLightType.Rocketry,  new MinionLightingConfig.LightConfig(true,  600,  4, MinionLightShape.DirectedCone, Color.white)},
        { MinionLightType.AtmoSuit,  new MinionLightingConfig.LightConfig(true,  400,  3, MinionLightShape.Pill,         TUNING.LIGHT2D.LIGHT_YELLOW)},
        { MinionLightType.JetSuit,   new MinionLightingConfig.LightConfig(true,  800,  5, MinionLightShape.DirectedCone, TUNING.LIGHT2D.LIGHT_YELLOW)},
        { MinionLightType.LeadSuit,  new MinionLightingConfig.LightConfig(true,  400,  3, MinionLightShape.Pill,         TUNING.LIGHT2D.LIGHT_YELLOW)},
      };

      minionEffectsConfig = new MinionEffectsConfig {
        { MinionEffectType.Dim, new MinionEffectsConfig.EffectConfig(true, 700, -2) },
        { MinionEffectType.Dark, new MinionEffectsConfig.EffectConfig(true, 300, -5) },
      };
    }

    [HarmonyPatch(typeof(Game)), HarmonyPatch("OnPrefabInit")]
    static class Patched_Game_OnPrefabInit
    {
      static void Prefix()
      {
        Config.instance = POptions.ReadSettings<Config>();
      }
    }
  }
}
