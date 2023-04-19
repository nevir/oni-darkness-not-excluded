using System.Collections.Generic;

namespace DarknessNotIncluded
{
  public static class DarknessGridUtils
  {
    public static List<int> GetOrthogonallyAdjacentCells(int cell)
    {
      var neighboringCells = new List<int>();

      if (Grid.IsValidCell(Grid.CellAbove(cell))) neighboringCells.Add(Grid.CellAbove(cell));
      if (Grid.IsValidCell(Grid.CellRight(cell))) neighboringCells.Add(Grid.CellRight(cell));
      if (Grid.IsValidCell(Grid.CellBelow(cell))) neighboringCells.Add(Grid.CellBelow(cell));
      if (Grid.IsValidCell(Grid.CellLeft(cell))) neighboringCells.Add(Grid.CellLeft(cell));

      return neighboringCells;
    }
  }
}
