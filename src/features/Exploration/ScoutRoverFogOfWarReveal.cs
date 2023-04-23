using HarmonyLib;
using UnityEngine;

namespace DarknessNotIncluded.Exploration
{
  static class ScoutRoverFogOfWarReveal
  {
    [HarmonyPatch(typeof(ScoutRoverConfig)), HarmonyPatch("CreateScout")]
    static class Patched_ScoutRoverConfig_CreateScout
    {
      static void Postfix(GameObject __result)
      {
        var gridVisibility = __result.AddOrGet<GridVisibility>();
        // TODO: reveal some?
        gridVisibility.radius = 0;
        gridVisibility.innerRadius = 0;
      }
    }
  }
}
