using HarmonyLib;
using UnityEngine;

namespace DarknessNotIncluded.Plants
{
  public static class DuskCapMaxLux
  {
    [HarmonyPatch(typeof(MushroomPlantConfig)), HarmonyPatch("CreatePrefab")]
    static class Patched_MushroomPlantConfig_CreatePrefab
    {
      static void Postfix(ref GameObject __result)
      {
        var illuminationVulnerable = __result.AddOrGet<IlluminationVulnerable>();

        Config.ObserveFor(illuminationVulnerable, (config) =>
        {
          illuminationVulnerable.SetMaxLux(config.duskCapPlantMaxLux);
        });
      }
    }
  }

}
