using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace DarknessNotIncluded
{
  static class SelectToolBlockedByDarkness
  {
    public static bool ShouldBlockSelect(int cell)
    {
      var config = Config.Instance;
      if (!config.selectToolBlockedByDarkness) return false;
      if (GameClock.Instance.GetTimeInCycles() < config.gracePeriodCycles) return false;

      var lightIntensity = Grid.LightIntensity;
      if (lightIntensity[cell] > 0) return false;

      foreach (var neighbor in DarknessGridUtils.GetOrthogonallyAdjacentCells(cell))
      {
        if (lightIntensity[neighbor] > 0) return false;
      }

      return true;
    }

    [HarmonyPatch(typeof(SelectToolHoverTextCard)), HarmonyPatch("UpdateHoverElements")]
    static class Hover
    {
      static bool Prefix(SelectToolHoverTextCard __instance, List<KSelectable> hoverObjects)
      {
        int cell = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
        if (ShouldBlockSelect(cell))
        {
          HoverTextDrawer drawer = HoverTextScreen.Instance.BeginDrawing();
          drawer.BeginShadowBar();
          drawer.DrawIcon(__instance.iconWarning);
          drawer.DrawText(STRINGS.UI.TOOLS.GENERIC.UNKNOWN, __instance.Styles_BodyText.Standard);
          drawer.EndShadowBar();
          __instance.recentNumberOfDisplayedSelectables = 1;
          drawer.EndDrawing();

          return false;
        }

        return true;
      }
    }

    [HarmonyPatch(typeof(SelectTool)), HarmonyPatch("Select")]
    static class Select
    {
      static bool Prefix(KSelectable new_selected, bool skipSound)
      {
        if (new_selected == null) return true;

        var cell = Grid.PosToCell(new_selected);
        return !ShouldBlockSelect(cell);
      }
    }
  }
}
