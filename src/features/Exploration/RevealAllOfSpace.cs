using HarmonyLib;
using System;

namespace DarknessNotIncluded.Exploration
{
  public static class RevealAllOfSpace
  {
    [HarmonyPatch(typeof(World)), HarmonyPatch("OnSpawn")]
    static class Patched_World_OnSpawn
    {
      static void Postfix()
      {
        Grid.OnReveal += (targetCell) =>
        {
          if (Grid.Revealed[targetCell]) return;
          if (Grid.ExposedToSunlight[targetCell] <= 0) return;

          var world = ClusterManager.Instance.GetWorld(Grid.WorldIdx[targetCell]);

          for (float x = world.minimumBounds.x; x <= world.maximumBounds.x; x++)
          {
            for (float y = world.minimumBounds.y; y < world.maximumBounds.y; y++)
            {
              var cell = Grid.PosToCell(new Vector2f(x, y));
              if (Grid.ExposedToSunlight[targetCell] <= 0) continue;
              Grid.Spawnable[cell] = 255;
              Grid.Visible[cell] = 255;
              Grid.Revealed[cell] = true;
            }
          }
        };
      }
    }
  }
}
