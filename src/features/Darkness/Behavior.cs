using System;

namespace DarknessNotIncluded.Darkness
{
  public enum InspectionLevel
  {
    None,
    BasicDetails,
    FullDetails,
  }

  public static class Behavior
  {
    public static bool enabled = true;

    /// <summary>
    /// Returns the lux of a cell, if it is lit—otherwise returns the light
    /// level that we should display the cell at in the UI.
    ///
    /// Cells near lit cells are given some implied lux, up to two cells away,
    /// based on the lux of the nearest lit cell, with the following strengths:
    ///
    /// [  0%] [ 25%] [ 50%] [ 25%] [  0%]
    /// [ 25%] [ 50%] [ 75%] [ 50%] [ 25%]
    /// [ 50%] [ 75%] [100%] [ 75%] [ 50%]
    /// [ 25%] [ 50%] [ 75%] [ 50%] [ 25%]
    /// [  0%] [ 25%] [ 50%] [ 25%] [  0%]
    /// </summary>
    static public int ActualOrImpliedLightLevel(int cell)
    {
      var cellLux = Grid.LightIntensity[cell];
      if (cellLux > 0) return cellLux;

      // Consider orthogonally adjacent cells
      var nearbyLux = 0;
      ConsiderLux(ref nearbyLux, Grid.CellAbove(cell));
      ConsiderLux(ref nearbyLux, Grid.CellRight(cell));
      ConsiderLux(ref nearbyLux, Grid.CellBelow(cell));
      ConsiderLux(ref nearbyLux, Grid.CellLeft(cell));
      // We're implied to be 75% of adjacent cell lux (if lit)
      if (nearbyLux > 0) return (nearbyLux * 3) / 4;

      // Consider orthogonally diagonal cells
      var midLux = 0;
      ConsiderLux(ref midLux, Grid.CellUpRight(cell));
      ConsiderLux(ref midLux, Grid.CellDownRight(cell));
      ConsiderLux(ref midLux, Grid.CellDownLeft(cell));
      ConsiderLux(ref midLux, Grid.CellUpLeft(cell));
      // Consider orthogonally adjacent cells, 1 away
      ConsiderLux(ref midLux, Grid.CellAbove(Grid.CellAbove(cell)));
      ConsiderLux(ref midLux, Grid.CellRight(Grid.CellRight(cell)));
      ConsiderLux(ref midLux, Grid.CellBelow(Grid.CellBelow(cell)));
      ConsiderLux(ref midLux, Grid.CellLeft(Grid.CellLeft(cell)));
      // We're implied to be 50% of adjacent cell lux (if lit)
      if (midLux > 0) return midLux / 2;

      // Consider orthogonally diagonal cells, 1 away
      var farLux = 0;
      ConsiderLux(ref farLux, Grid.CellUpRight(Grid.CellAbove(cell)));
      ConsiderLux(ref farLux, Grid.CellUpRight(Grid.CellRight(cell)));
      ConsiderLux(ref farLux, Grid.CellDownRight(Grid.CellRight(cell)));
      ConsiderLux(ref farLux, Grid.CellDownRight(Grid.CellBelow(cell)));
      ConsiderLux(ref farLux, Grid.CellDownLeft(Grid.CellBelow(cell)));
      ConsiderLux(ref farLux, Grid.CellDownLeft(Grid.CellLeft(cell)));
      ConsiderLux(ref farLux, Grid.CellUpLeft(Grid.CellLeft(cell)));
      ConsiderLux(ref farLux, Grid.CellUpLeft(Grid.CellAbove(cell)));
      // We're implied to be 25% of adjacent cell lux (if lit)
      if (farLux > 0) return farLux / 4;

      return 0;
    }

    static private void ConsiderLux(ref int maxLux, int cell)
    {
      if (!Grid.IsValidCell(cell)) return;
      maxLux = Math.Max(maxLux, Grid.LightIntensity[cell]);
    }

    static public InspectionLevel InspectionLevelForCell(int cell)
    {
      if (!Grid.IsValidCell(cell)) return InspectionLevel.None;

      if (!Darkness.Behavior.enabled) return InspectionLevel.FullDetails;
      if (DebugHandler.RevealFogOfWar) return InspectionLevel.FullDetails;
      if (Grid.Visible[cell] <= 0) return InspectionLevel.None;

      var config = Config.Instance;
      if (!config.selectToolBlockedByDarkness) return InspectionLevel.FullDetails;
      if (GameClock.Instance.GetTimeInCycles() < config.gracePeriodCycles) return InspectionLevel.FullDetails;

      var lux = ActualOrImpliedLightLevel(cell);
      return lux > 0 ? InspectionLevel.FullDetails : InspectionLevel.BasicDetails;
    }
  }
}
