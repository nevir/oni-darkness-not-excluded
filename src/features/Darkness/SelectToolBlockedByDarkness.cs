using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace DarknessNotIncluded.Darkness
{
  static class SelectToolBlockedByDarkness
  {
    [HarmonyPatch(typeof(SelectToolHoverTextCard)), HarmonyPatch("UpdateHoverElements")]
    static class Patched_SelectToolHoverTextCard_UpdateHoverElements
    {
      static bool Prefix(SelectToolHoverTextCard __instance, List<KSelectable> hoverObjects)
      {
        int cell = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
        var inspectionLevel = Behavior.InspectionLevelForCell(cell);
        switch (inspectionLevel)
        {
          case InspectionLevel.None:
            RenderUnknownHoverCard(__instance);
            return false;

          case InspectionLevel.BasicDetails:
            RenderBasicHoverCard(__instance, cell, hoverObjects);
            return false;

          case InspectionLevel.FullDetails:
          default:
            return true;
        }
      }

      static void RenderUnknownHoverCard(SelectToolHoverTextCard hoverCard)
      {
        HoverTextDrawer drawer = HoverTextScreen.Instance.BeginDrawing();
        drawer.BeginShadowBar();
        drawer.DrawIcon(hoverCard.iconWarning);
        drawer.DrawText(STRINGS.UI.TOOLS.GENERIC.UNKNOWN, hoverCard.Styles_BodyText.Standard);
        drawer.EndShadowBar();
        hoverCard.recentNumberOfDisplayedSelectables = 1;
        drawer.EndDrawing();
      }

      static void RenderBasicHoverCard(SelectToolHoverTextCard hoverCard, int cell, List<KSelectable> hoverObjects)
      {
        HoverTextDrawer drawer = HoverTextScreen.Instance.BeginDrawing();

        drawer.BeginShadowBar();
        drawer.DrawIcon(hoverCard.iconWarning);
        drawer.DrawText("UNLIT", hoverCard.Styles_Title.Standard);
        drawer.EndShadowBar();

        foreach (var hoverObject in hoverObjects)
        {
          if (CellSelectionObject.IsSelectionObject(hoverObject.gameObject)) continue;

          var primaryElement = hoverObject.GetComponent<PrimaryElement>();
          var name = GameUtil.GetUnitFormattedName(hoverObject.gameObject, true);
          if (primaryElement != null && hoverObject.GetComponent<Building>())
          {
            name = StringFormatter.Replace(StringFormatter.Replace(STRINGS.UI.TOOLS.GENERIC.BUILDING_HOVER_NAME_FMT, "{Name}", name), "{Element}", primaryElement.Element.nameUpperCase);
          }

          drawer.BeginShadowBar(SelectTool.Instance.selected == hoverObject);
          drawer.DrawText(name, hoverCard.Styles_Title.Standard);
          drawer.EndShadowBar();
        }

        var showElement = !Grid.DupePassable[cell] || !Grid.Solid[cell];
        if (showElement)
        {
          var element = Grid.Element[cell];
          drawer.BeginShadowBar();
          drawer.DrawText(element.nameUpperCase, hoverCard.Styles_Title.Standard);
          drawer.EndShadowBar();
        }

        drawer.EndDrawing();
      }
    }

    [HarmonyPatch(typeof(SelectTool)), HarmonyPatch("Select")]
    static class Patched_SelectTool_Select
    {
      static bool Prefix(KSelectable new_selected, bool skipSound)
      {
        if (new_selected == null) return true;

        var cell = Grid.PosToCell(new_selected);
        var inspectionLevel = Behavior.InspectionLevelForCell(cell);
        if (inspectionLevel == InspectionLevel.None) return false;
        if (inspectionLevel == InspectionLevel.FullDetails) return true;

        // Can select non-elements.
        return !CellSelectionObject.IsSelectionObject(new_selected.gameObject);
      }
    }
  }
}
