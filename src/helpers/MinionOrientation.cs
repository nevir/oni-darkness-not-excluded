using HarmonyLib;
using UnityEngine;

namespace DarknessNotIncluded
{
  public class MinionOrientation : KMonoBehaviour, ISim33ms
  {
    public enum Orientation
    {
      Unknown,
      Left,
      Right,
      Up,
      Down,
    }

    [MyCmpGet]
    private MinionIdentity minion;
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
      if (!navigator.IsMoving())
      {
        return navigator.IsFacingLeft ? Orientation.Left : Orientation.Right;
      }

      var currCell = Grid.PosToCell(minion);
      var nextCell = navigator?.path.nodes?.Count > 0 ? navigator.path.nodes[0].cell : 0;

      if (Grid.CellRow(nextCell) > Grid.CellRow(currCell)) return Orientation.Down;
      if (Grid.CellRow(nextCell) < Grid.CellRow(currCell)) return Orientation.Up;
      if (Grid.CellColumn(nextCell) > Grid.CellColumn(currCell)) return Orientation.Left;
      if (Grid.CellColumn(nextCell) < Grid.CellColumn(currCell)) return Orientation.Right;

      return Orientation.Unknown;
    }

    [HarmonyPatch(typeof(MinionConfig)), HarmonyPatch("CreatePrefab")]
    static class Patched_MinionConfig_CreatePrefab
    {
      static void Postfix(GameObject __result)
      {
        __result.AddOrGet<MinionOrientation>();
      }
    }
  }
}
