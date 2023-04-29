using HarmonyLib;
using System;
using UnityEngine;

namespace DarknessNotIncluded.Plants
{
  public static class BogBucketMaxLux
  {
    [HarmonyPatch(typeof(SwampHarvestPlantConfig)), HarmonyPatch("CreatePrefab")]
    static class Patched_SwampHarvestPlantConfig_CreatePrefab
    {
      static void Postfix(ref GameObject __result)
      {
        var illuminationVulnerable = __result.AddOrGet<IlluminationVulnerable>();

        Config.ObserveFor(illuminationVulnerable, (config) =>
        {
          illuminationVulnerable.SetMaxLux(config.bogBucketPlantMaxLux);
        });
      }
    }
  }

}
