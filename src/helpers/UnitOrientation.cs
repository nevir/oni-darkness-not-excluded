namespace DarknessNotIncluded
{
  public class UnitOrientation : KMonoBehaviour, ISim33ms
  {
    public enum Orientation
    {
      Unknown,
      Left,
      UpLeft,
      Up,
      UpRight,
      Right,
      DownRight,
      Down,
      DownLeft,
    }

    [MyCmpGet]
    private Navigator navigator;

    public Orientation orientation = Orientation.Unknown;

    public void Sim33ms(float dt)
    {
      var newOrientation = GetCurrentOrientation();
      if (newOrientation != Orientation.Unknown)
      {
        orientation = newOrientation;
      }
    }

    private Orientation GetCurrentOrientation()
    {
      if (!navigator.IsMoving() || !navigator.path.IsValid())
      {
        return navigator.IsFacingLeft ? Orientation.Left : Orientation.Right;
      }

      var currCell = Grid.PosToCell(gameObject);
      var nextCell = navigator.path.nodes[1].cell;
      var vert = Grid.CellRow(nextCell) - Grid.CellRow(currCell); // up > 0 > down
      var horiz = Grid.CellColumn(nextCell) - Grid.CellColumn(currCell); // right > 0 > left

      if (horiz < 0 && vert == 0) return Orientation.Left;
      if (horiz < 0 && vert > 0) return Orientation.UpLeft;
      if (horiz == 0 && vert > 0) return Orientation.Up;
      if (horiz > 0 && vert > 0) return Orientation.UpRight;
      if (horiz > 0 && vert == 0) return Orientation.Right;
      if (horiz > 0 && vert < 0) return Orientation.DownRight;
      if (horiz == 0 && vert < 0) return Orientation.Down;
      if (horiz < 0 && vert < 0) return Orientation.DownLeft;

      return Orientation.Unknown;
    }
  }
}
