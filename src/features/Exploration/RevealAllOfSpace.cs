using HarmonyLib;
using System.Collections.Generic;
using ProcGen;

namespace DarknessNotIncluded.Exploration
{
  public static class RevealAllOfSpace
  {
    static HashSet<int> REVEALED_CELLS = new HashSet<int>();

    [HarmonyPatch(typeof(World)), HarmonyPatch("OnSpawn")]
    static class Patched_World_OnSpawn
    {
      static void Postfix()
      {
        Grid.OnReveal += (targetCell) =>
        {
          if (REVEALED_CELLS.Contains(targetCell)) return;
          if (!IsSpaceBiomeAndLitBySunlight(targetCell)) return;

          var world = ClusterManager.Instance.GetWorld(Grid.WorldIdx[targetCell]);
          var maxSize = world.WorldSize.x * world.WorldSize.y;
          var cells = GameUtil.FloodCollectCells(targetCell, IsSpaceBiomeAndLitBySunlight, maxSize);

          foreach (var cell in cells)
          {
            REVEALED_CELLS.Add(cell);
            Grid.Reveal(cell);
          }
        };
      }

      static bool IsSpaceBiomeAndLitBySunlight(int cell)
      {
        var isSpaceBiome = Game.Instance.world.zoneRenderData.GetSubWorldZoneType(cell) == SubWorld.ZoneType.Space;
        var isSpaceCell = Grid.Objects[cell, 2] == null;
        var isLitBySunlight = Grid.ExposedToSunlight[cell] > 0;

        return isSpaceBiome && isSpaceCell && isLitBySunlight;
      }
    }
  }
}
