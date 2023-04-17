using HarmonyLib;
using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace DarknessNotIncluded
{
  [JsonObject(MemberSerialization.OptOut)]
  [ModInfo("https://github.com/nevir/oni-darkness-not-excluded")]
  [ConfigFile(SharedConfigLocation: true)]
  class Config : SingletonOptions<Config>
  {
    [Option("Darkness grace period (cycles)", "How many cycles should it take to go from no darkness to maximum darkness?", "Darkness")]
    public float gracePeriodCycles { get; set; }

    [Option("Initial darkness level", "How dark should 0 lux tiles be at the start of the game?\n0 = pitch black, 255 = fully visible", "Darkness")]
    public int initialFogLevel { get; set; }

    [Option("Maximum darkness level", "How dark should 0 lux tiles be after the initial grace period?\n0 = pitch black, 255 = fully visible", "Darkness")]
    public int minimumFogLevel { get; set; }

    [Option("Lux threshold", "At what lux should a tile be fully visible?", "Darkness")]
    public int fullyVisibleLuxThreshold { get; set; }

    public Config()
    {
      initialFogLevel = 200;
      minimumFogLevel = 0;
      gracePeriodCycles = 0.15f;
      fullyVisibleLuxThreshold = 1800;
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
