using HarmonyLib;
using UnityEngine;

namespace DarknessNotIncluded.Exploration
{
  static class ResetDefaultGridVisibility
  {
    [HarmonyPatch(typeof(MinionConfig)), HarmonyPatch("CreatePrefab")]
    static class Patched_MinionConfig_CreatePrefab
    {
      static void Postfix(GameObject __result)
      {
        var gridVisibility = __result.AddOrGet<GridVisibility>();
        gridVisibility.SetRadius(0);
      }
    }
  }
}
