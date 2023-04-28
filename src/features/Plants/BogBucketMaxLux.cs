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
        var illuminationVulnerableInitial = __result.AddOrGet<IlluminationVulnerable>();

        var illuminationVulnerableRef = new WeakReference(illuminationVulnerableInitial);
        new Config.Observer((config) =>
        {
          var illuminationVulnerable = (IlluminationVulnerable)illuminationVulnerableRef.Target;
          if (illuminationVulnerable == null) return;

          illuminationVulnerable.SetMaxLux(config.bogBucketPlantMaxLux);
        });
      }
    }
  }

}
