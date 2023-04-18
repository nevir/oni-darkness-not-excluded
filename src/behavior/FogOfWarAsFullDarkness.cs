using System;
using System.Collections.Generic;
using HarmonyLib;

namespace DarknessNotIncluded
{
  [HarmonyPatch(typeof(PropertyTextures)), HarmonyPatch("UpdateFogOfWar")]
  class FogOfWarAsFullDarkness
  {
    static bool Prefix(TextureRegion region, int x0, int y0, int x1, int y1)
    {
      var config = Config.Instance;
      var visible = Grid.Visible;
      var lightIntensity = Grid.LightIntensity;

      var gridYOffset = Grid.HeightInCells;
      if (ClusterManager.Instance != (UnityEngine.Object)null)
      {
        var activeWorld = ClusterManager.Instance.activeWorld;
        gridYOffset = activeWorld.WorldSize.Y + activeWorld.WorldOffset.Y - 1;
      }

      var minFogLevel = config.minimumFogLevel;
      var gameCycle = GameClock.Instance.GetTimeInCycles();
      if (gameCycle < config.gracePeriodCycles)
      {
        float scaledFogLevel = 1.0f - gameCycle / config.gracePeriodCycles;
        minFogLevel = Math.Max(minFogLevel, (int)(scaledFogLevel * (float)config.initialFogLevel));
      }
      var fogRange = 255 - minFogLevel;

      for (int y = y0; y <= y1; ++y)
      {
        for (int x = x0; x <= x1; ++x)
        {
          var cell = Grid.XYToCell(x, y);
          if (!Grid.IsActiveWorld(cell))
          {
            cell = Grid.XYToCell(x, gridYOffset);
          }

          if (!Grid.IsValidCell(cell))
          {
            region.SetBytes(x, y, (byte)0);
            continue;
          }

          if (visible[cell] == 0)
          {
            region.SetBytes(x, y, (byte)0);
            continue;
          }

          var lux = lightIntensity[cell];
          if (lux == 0)
          {
            var neighboringCells = GetNeighboringCells(cell);
            foreach (var neighbor in neighboringCells)
            {
              lux = Math.Max(lux, lightIntensity[neighbor]);
            }

            // And reduce for clean falloff.
            lux /= 2;
          }

          int fog = minFogLevel + (Math.Min(lux, config.fullyVisibleLuxThreshold) * fogRange) / config.fullyVisibleLuxThreshold;

          region.SetBytes(x, y, Math.Min((byte)fog, visible[cell]));

          // Hides tooltips
          // TODO: Figure out how to preserve original visibility changes.
          // TODO: Seems to Just Work? …except when returning to old locations.
          // TODO: Just hook tooltip display?
          // if (fog == 0 && visible[cell] != 0)
          // {
          //   visible[cell] = 0;
          // }
        }
      }

      // Completely override default behavior.
      // TODO: Is there a canonical way of doing this other than Prefix()?
      // TODO: Or is Prefix preferrable for some reason?d
      return false;
    }

    static List<int> GetNeighboringCells(int cell)
    {
      var neighboringCells = new List<int>();

      if (Grid.IsValidCell(Grid.CellAbove(cell))) neighboringCells.Add(Grid.CellAbove(cell));
      // if (Grid.IsValidCell(Grid.CellUpRight(cell))) neighboringCells.Add(Grid.CellUpRight(cell));
      if (Grid.IsValidCell(Grid.CellRight(cell))) neighboringCells.Add(Grid.CellRight(cell));
      // if (Grid.IsValidCell(Grid.CellDownRight(cell))) neighboringCells.Add(Grid.CellDownRight(cell));
      if (Grid.IsValidCell(Grid.CellBelow(cell))) neighboringCells.Add(Grid.CellBelow(cell));
      // if (Grid.IsValidCell(Grid.CellDownLeft(cell))) neighboringCells.Add(Grid.CellDownLeft(cell));
      if (Grid.IsValidCell(Grid.CellLeft(cell))) neighboringCells.Add(Grid.CellLeft(cell));
      // if (Grid.IsValidCell(Grid.CellUpLeft(cell))) neighboringCells.Add(Grid.CellUpLeft(cell));

      return neighboringCells;
    }
  }
}
