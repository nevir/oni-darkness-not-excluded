using HarmonyLib;
using Newtonsoft.Json;
using PeterHan.PLib.Options;
using UnityEngine;

namespace DarknessNotIncluded
{
  [JsonObject(MemberSerialization.OptOut)]
  [ModInfo("https://github.com/nevir/oni-darkness-not-excluded")]
  [ConfigFile(SharedConfigLocation: true)]
  class Config : SingletonOptions<Config>
  {
    // Darkness

    [Option("Darkness grace period (cycles)", "How many cycles should it take to go from no darkness to maximum darkness?", "Darkness")]
    public float gracePeriodCycles { get; set; }

    [Option("Initial darkness level", "How dark should 0 lux tiles be at the start of the game?\n0 = pitch black, 255 = fully visible", "Darkness")]
    [Limit(0, 255)]
    public int initialFogLevel { get; set; }

    [Option("Maximum darkness level", "How dark should 0 lux tiles be after the initial grace period?\n0 = pitch black, 255 = fully visible", "Darkness")]
    [Limit(0, 255)]
    public int minimumFogLevel { get; set; }

    [Option("Lux threshold", "At what lux should a tile be fully visible?", "Darkness")]
    public int fullyVisibleLuxThreshold { get; set; }

    // Dupe Base Glow

    [Option("Enabled", "Whether light should be emitted around dupes, even when no other light sources are present.", "Dupe Base Glow")]
    public bool dupeIntrinsicLightEnabled { get; set; }

    [Option("Lux", "How much light should be emitted around dupes, even when no other light sources are present?", "Dupe Base Glow")]
    public int dupeIntrinsicLightLux { get; set; }

    [Option("Radius", "How far should light be emitted around dupes, even when no other light sources are present?", "Dupe Base Glow")]
    public int dupeIntrinsicLightRadius { get; set; }

    // [Option("Color", "What color of light should be emitted around dupes, even when no other light sources are present?", "Dupe Base Glow")]
    [JsonIgnore]
    public Color dupeIntrinsicLightColor { get; set; }

    // Tier 1 Mining Hat (Hard Digging)

    [Option("Enabled", "Whether tier 1 mining hats should emit light.", "Tier 1 Mining Hat (Hard Digging)")]
    public bool miningHatTier1Enabled { get; set; }

    [Option("Lux", "How much light should be emitted from a tier 1 mining hat?", "Tier 1 Mining Hat (Hard Digging)")]
    public int miningHatTier1Lux { get; set; }

    [Option("Radius", "How far should light be emitted from a tier 1 mining hat?", "Tier 1 Mining Hat (Hard Digging)")]
    public int miningHatTier1Radius { get; set; }

    // [Option("Color", "What color should a tier 1 mining hat emit?", "Tier 1 Mining Hat (Hard Digging)")]
    [JsonIgnore]
    public Color miningHatTier1Color { get; set; }

    // Tier 2 Mining Hat (Superhard Digging)

    [Option("Enabled", "Whether tier 2 mining hats should emit light.", "Tier 2 Mining Hat (Superhard Digging)")]
    public bool miningHatTier2Enabled { get; set; }

    [Option("Lux", "How much light should be emitted from a tier 2 mining hat?", "Tier 2 Mining Hat (Superhard Digging)")]
    public int miningHatTier2Lux { get; set; }

    [Option("Radius", "How far should light be emitted from a tier 2 mining hat?", "Tier 2 Mining Hat (Superhard Digging)")]
    public int miningHatTier2Radius { get; set; }

    // [Option("Color", "What color should a tier 2 mining hat emit?", "Tier 2 Mining Hat (Superhard Digging)")]
    [JsonIgnore]
    public Color miningHatTier2Color { get; set; }

    // Tier 3 Mining Hat (Super-Duperhard Digging)

    [Option("Enabled", "Whether tier 3 mining hats should emit light.", "Tier 3 Mining Hat (Super-Duperhard Digging)")]
    public bool miningHatTier3Enabled { get; set; }

    [Option("Lux", "How much light should be emitted from a tier 3 mining hat?", "Tier 3 Mining Hat (Super-Duperhard Digging)")]
    public int miningHatTier3Lux { get; set; }

    [Option("Radius", "How far should light be emitted from a tier 3 mining hat?", "Tier 3 Mining Hat (Super-Duperhard Digging)")]
    public int miningHatTier3Radius { get; set; }

    // [Option("Color", "What color should a tier 3 mining hat emit?", "Tier 3 Mining Hat (Super-Duperhard Digging)")]
    [JsonIgnore]
    public Color miningHatTier3Color { get; set; }

    // Tier 4 Mining Hat (Hazmat Digging)

    [Option("Enabled", "Whether tier 4 mining hats should emit light.", "Tier 4 Mining Hat (Hazmat Digging)")]
    public bool miningHatTier4Enabled { get; set; }

    [Option("Lux", "How much light should be emitted from a tier 4 mining hat?", "Tier 4 Mining Hat (Hazmat Digging)")]
    public int miningHatTier4Lux { get; set; }

    [Option("Radius", "How far should light be emitted from a tier 4 mining hat?", "Tier 4 Mining Hat (Hazmat Digging)")]
    public int miningHatTier4Radius { get; set; }

    // [Option("Color", "What color should a tier 4 mining hat emit?", "Tier 4 Mining Hat (Hazmat Digging)")]
    [JsonIgnore]
    public Color miningHatTier4Color { get; set; }

    // Science Hats

    [Option("Enabled", "Whether science hats should emit light.", "Science Hats")]
    public bool scienceHatEnabled { get; set; }

    [Option("Lux", "How much light should be emitted from a science hat?", "Science Hats")]
    public int scienceHatLux { get; set; }

    [Option("Radius", "How far should light be emitted from a science hat?", "Science Hats")]
    public int scienceHatRadius { get; set; }

    // [Option("Color", "What color should a science hat emit?", "Science Hats")]
    [JsonIgnore]
    public Color scienceHatColor { get; set; }

    // Rocketry Hats

    [Option("Enabled", "Whether rocketry hats should emit light.", "Rocketry Hats")]
    public bool rocketryHatEnabled { get; set; }

    [Option("Lux", "How much light should be emitted from a rocketry hat?", "Rocketry Hats")]
    public int rocketryHatLux { get; set; }

    [Option("Radius", "How far should light be emitted from a rocketry hat?", "Rocketry Hats")]
    public int rocketryHatRadius { get; set; }

    // [Option("Color", "What color should a rocketry hat emit?", "Rocketry Hats")]
    [JsonIgnore]
    public Color rocketryHatColor { get; set; }

    public Config()
    {
      // Darkness
      initialFogLevel = 200;
      minimumFogLevel = 0;
      gracePeriodCycles = 0.15f;
      fullyVisibleLuxThreshold = TUNING.DUPLICANTSTATS.LIGHT.MEDIUM_LIGHT;

      // Dupe Base Glow
      dupeIntrinsicLightEnabled = true;
      dupeIntrinsicLightLux = 200;
      dupeIntrinsicLightRadius = 2;
      dupeIntrinsicLightColor = Color.white;

      // Tier 1 Mining Hat (Hard Digging)
      miningHatTier1Enabled = true;
      miningHatTier1Lux = 600;
      miningHatTier1Radius = 3;
      miningHatTier1Color = TUNING.LIGHT2D.LIGHT_YELLOW;

      // Tier 2 Mining Hat (Superhard Digging)
      miningHatTier2Enabled = true;
      miningHatTier2Lux = 800;
      miningHatTier2Radius = 4;
      miningHatTier2Color = TUNING.LIGHT2D.LIGHT_YELLOW;

      // Tier 3 Mining Hat (Super-Duperhard Digging)
      miningHatTier3Enabled = true;
      miningHatTier3Lux = 1200;
      miningHatTier3Radius = 5;
      miningHatTier3Color = Color.white;

      // Tier 4 Mining Hat (Hazmat Digging)
      miningHatTier4Enabled = true;
      miningHatTier4Lux = 1600;
      miningHatTier4Radius = 6;
      miningHatTier4Color = Color.white;

      // Science Hats
      scienceHatEnabled = true;
      scienceHatLux = 400;
      scienceHatRadius = 3;
      scienceHatColor = Color.white;

      // Rocketry Hats
      rocketryHatEnabled = true;
      rocketryHatLux = 600;
      rocketryHatRadius = 4;
      rocketryHatColor = Color.white;
    }

    [HarmonyPatch(typeof(Game)), HarmonyPatch("OnPrefabInit")]
    static class Loader
    {
      static void Prefix()
      {
        Config.instance = POptions.ReadSettings<Config>();
      }
    }
  }
}
