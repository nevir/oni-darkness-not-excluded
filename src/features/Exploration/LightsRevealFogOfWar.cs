using HarmonyLib;
using PeterHan.PLib.Core;
using System.Collections.Generic;
using System;

namespace DarknessNotIncluded.Exploration
{
  static class LightsRevealFogOfWar
  {
    [HarmonyPatch(typeof(LightGridManager.LightGridEmitter)), HarmonyPatch("AddToGrid")]
    static class Patched_LightGridManager_LightGridEmitter_AddToGrid
    {
      static void Postfix(List<int> ___litCells)
      {
        var shouldExpandFogOfWar = false;
        foreach (var cell in ___litCells)
        {
          if (Grid.Visible[cell] > 0)
          {
            shouldExpandFogOfWar = true;
            break;
          }
        }
        if (!shouldExpandFogOfWar) return;

        var expandedCells = ExpandRegion(___litCells);
        foreach (var cell in expandedCells)
        {
          if (!Grid.IsValidCell(cell)) continue;
          Grid.Visible[cell] = 255;
        }
      }
    }

    // This should match the same area as defined in 
    // Darkness.Behavior.ActualOrImpliedLightLevel
    // 
    // TODO: Refactor & centralize that logic
    static HashSet<int> ExpandRegion(List<int> litCells)
    {
      var newRegion = new HashSet<int>(litCells);
      foreach (var cell in litCells)
      {
        // Adjacent cells
        newRegion.Add(Grid.CellAbove(cell));
        newRegion.Add(Grid.CellUpRight(cell));
        newRegion.Add(Grid.CellRight(cell));
        newRegion.Add(Grid.CellDownRight(cell));
        newRegion.Add(Grid.CellBelow(cell));
        newRegion.Add(Grid.CellDownLeft(cell));
        newRegion.Add(Grid.CellLeft(cell));
        newRegion.Add(Grid.CellUpLeft(cell));

        // Distant adjacent: up
        newRegion.Add(Grid.CellUpLeft(Grid.CellAbove(cell)));
        newRegion.Add(Grid.CellAbove(Grid.CellAbove(cell)));
        newRegion.Add(Grid.CellUpRight(Grid.CellAbove(cell)));
        // Distant adjacent: right
        newRegion.Add(Grid.CellUpRight(Grid.CellRight(cell)));
        newRegion.Add(Grid.CellRight(Grid.CellRight(cell)));
        newRegion.Add(Grid.CellDownRight(Grid.CellRight(cell)));
        // Distant adjacent: down
        newRegion.Add(Grid.CellDownRight(Grid.CellBelow(cell)));
        newRegion.Add(Grid.CellBelow(Grid.CellBelow(cell)));
        newRegion.Add(Grid.CellDownLeft(Grid.CellBelow(cell)));
        // Distant adjacent: left
        newRegion.Add(Grid.CellDownLeft(Grid.CellLeft(cell)));
        newRegion.Add(Grid.CellLeft(Grid.CellLeft(cell)));
        newRegion.Add(Grid.CellUpLeft(Grid.CellLeft(cell)));
      }

      return newRegion;
    }
  }
}
