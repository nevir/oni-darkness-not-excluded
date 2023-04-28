using System;
using HarmonyLib;

namespace DarknessNotIncluded.Darkness
{
  static class FadeUnlitCells
  {
    private static float gracePeriodCycles;
    private static int initialFogLevel;
    private static int minimumFogLevel;
    private static int fullyVisibleLuxThreshold;

    private static Config.Observer configObserver = new Config.Observer((config) =>
    {
      gracePeriodCycles = config.gracePeriodCycles;
      initialFogLevel = config.initialFogLevel;
      minimumFogLevel = config.minimumFogLevel;
      fullyVisibleLuxThreshold = config.fullyVisibleLuxThreshold;
    });

    [HarmonyPatch(typeof(PropertyTextures)), HarmonyPatch("UpdateFogOfWar")]
    static class Patched_PropertyTextures_UpdateFogOfWar
    {
      static bool Prefix(TextureRegion region, int x0, int y0, int x1, int y1)
      {
        if (Game.Instance.SandboxModeActive) return true;

        var visible = Grid.Visible;

        var gridYOffset = Grid.HeightInCells;
        if (ClusterManager.Instance != (UnityEngine.Object)null)
        {
          var activeWorld = ClusterManager.Instance.activeWorld;
          gridYOffset = activeWorld.WorldSize.Y + activeWorld.WorldOffset.Y - 1;
        }

        var minFogLevel = minimumFogLevel;
        var gameCycle = GameClock.Instance.GetTimeInCycles();
        if (gameCycle < gracePeriodCycles)
        {
          float scaledFogLevel = 1.0f - gameCycle / gracePeriodCycles;
          minFogLevel = Math.Max(minFogLevel, (int)(scaledFogLevel * (float)initialFogLevel));
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

            if (!Behavior.enabled)
            {
              region.SetBytes(x, y, (byte)255);
            }
            else
            {
              var lux = Behavior.ActualOrImpliedLightLevel(cell);
              int fog = minFogLevel + (Math.Min(lux, fullyVisibleLuxThreshold) * fogRange) / fullyVisibleLuxThreshold;
              region.SetBytes(x, y, (byte)fog);
            }
          }
        }

        return false;
      }
    }
  }
}
