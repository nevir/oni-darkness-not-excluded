using HarmonyLib;
using System.Collections.Generic;

namespace DarknessNotIncluded.Exploration
{
  public static class DragOverFogOfWar
  {
    private static bool dragToolIgnoresVisibility;

    private static Config.Observer configObserver = new Config.Observer((config) =>
    {
      dragToolIgnoresVisibility = config.dragToolIgnoresVisibility;
    });

    [HarmonyPatch(typeof(DragTool)), HarmonyPatch("OnLeftClickUp")]
    static class Patched_DragTool_OnLeftClickUp
    {
      static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
      {
        var GridIsVisible = typeof(Grid).GetMethod("IsVisible");

        foreach (var instruction in instructions)
        {
          // We're searching for:
          //
          //   if (Grid.IsValidCell(cell) && Grid.IsVisible(cell))
          //
          // IL:
          //
          //   ldloc.s 9 (System.Int32)
          //   call static System.Boolean Grid::IsValidCell(System.Int32 cell)
          //   brfalse.s Label12
          //   ldloc.s 9(System.Int32)
          //   call static System.Boolean Grid::IsVisible(System.Int32 cell)
          //   brfalse.s Label13
          //
          if (instruction.Calls(GridIsVisible))
          {
            // And rewriting to:
            //
            //   if (Grid.IsValidCell(cell) && Grid.IsVisible(cell))
            //
            // IL:
            //
            //   ldloc.s 9 (System.Int32)
            //   call static System.Boolean Grid::IsValidCell(System.Int32 cell)
            //   brfalse.s Label12
            //   ldloc.s 9(System.Int32)
            //   call static System.Boolean Grid::IsVisible(System.Int32 cell)
            //   brfalse.s Label13
            //
            yield return CodeInstruction.Call(typeof(DragOverFogOfWar), "ShouldConsiderCell");
          }
          else
          {
            yield return instruction;
          }
        }
      }
    }

    static bool ShouldConsiderCell(int cell)
    {
      if (dragToolIgnoresVisibility)
      {
        return true;
      }
      else
      {
        return Grid.IsVisible(cell);
      }
    }
  }
}
