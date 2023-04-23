using HarmonyLib;
using System;
using UnityEngine;

namespace DarknessNotIncluded.Exploration
{
  public class FogOfWarRevealer : KMonoBehaviour
  {
    public int radius = 0;

    protected override void OnSpawn()
    {
      Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(this.transform, new System.Action(this.OnCellChange), "GridVisibility.FogOfWarRevealer");
      this.OnCellChange();
    }

    // TODO: optimize perf
    private void OnCellChange()
    {
      Console.WriteLine("FogOfWarRevealer OnCellChange");
      if (radius <= 0) return;

      int x;
      int y;
      Grid.PosToXY(this.transform.GetPosition(), out x, out y);
      GridVisibility.Reveal(x, y, this.radius, this.radius - 2);
    }
  }

  [HarmonyPatch(typeof(Telepad)), HarmonyPatch("OnSpawn")]
  static class Patched_Telepad_OnSpawn
  {
    static void Postfix(Telepad __instance)
    {
      Console.WriteLine("telepad spawn");
      var revealer = __instance.gameObject.AddOrGet<FogOfWarRevealer>();
      revealer.radius = 10;
    }
  }
}
