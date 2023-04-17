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

    [Option("Radius", "How much light should be emitted around dupes, even when no other light sources are present?", "Dupe Base Glow")]
    public int dupeIntrinsicLightRadius { get; set; }

    [Option("Color", "What color of light should be emitted around dupes, even when no other light sources are present?", "Dupe Base Glow")]
    public Color dupeIntrinsicLightColor { get; set; }

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
