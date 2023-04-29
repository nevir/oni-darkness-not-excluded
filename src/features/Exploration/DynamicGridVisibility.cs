using HarmonyLib;

namespace DarknessNotIncluded.Exploration
{
  public static class DynamicGridVisibility
  {
    public static void SetRadius(this GridVisibility gridVisibility, int radius)
    {
      gridVisibility.radius = radius;
      gridVisibility.innerRadius = (float)gridVisibility.radius * 0.7f;
    }

    [HarmonyPatch(typeof(GridVisibility)), HarmonyPatch("OnCellChange")]
    static class Patched_GridVisibility_OnCellChange
    {
      static bool Prefix(GridVisibility __instance)
      {
        if (__instance == null) return false;
        if (__instance.gameObject == null) return false;
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
