using HarmonyLib;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace DarknessNotIncluded.Exploration
{
  static class DynamicGridVisibility
  {
    [HarmonyPatch(typeof(GridVisibility)), HarmonyPatch("OnCellChange")]
    static class Patched_GridVisibility_OnCellChange
    {
      static bool Prefix(GridVisibility __instance)
      {
        if (__instance.radius <= 0) return false;
        if (__instance.gameObject.HasTag(GameTags.Dead)) return false;

        var cell = Grid.PosToCell(__instance);
        if (!Grid.IsValidCell(cell)) return false;

        int x;
        int y;
        Grid.PosToXY(__instance.transform.GetPosition(), out x, out y);
        GridVisibility.Reveal(x, y, __instance.radius, __instance.innerRadius);

        return false;
      }
    }
  }
}
