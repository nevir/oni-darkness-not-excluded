using HarmonyLib;
using UnityEngine;

namespace DarknessNotIncluded.Exploration
{
  static class MinionFogOfWarReveal
  {
    [HarmonyPatch(typeof(MinionConfig)), HarmonyPatch("CreatePrefab")]
    static class Patched_MinionConfig_CreatePrefab
    {
      static void Postfix(GameObject __result)
      {
        var gridVisibility = __result.AddOrGet<GridVisibility>();
        // TODO: mining hats.
        gridVisibility.radius = 0;
        gridVisibility.innerRadius = 0;
      }
    }
  }
}
