using System;
using HarmonyLib;

namespace DarknessNotIncluded.Darkness
{
  static class FadeUnlitCells
  {
    [HarmonyPatch(typeof(PropertyTextures)), HarmonyPatch("UpdateFogOfWar")]
    static class Patched_PropertyTextures_UpdateFogOfWar
    {
      static bool Prefix(TextureRegion region, int x0, int y0, int x1, int y1)
      {
        if (Game.Instance.SandboxModeActive) return true;

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

            var lux = Behavior.ActualOrImpliedLightLevel(cell);
            int fog = minFogLevel + (Math.Min(lux, config.fullyVisibleLuxThreshold) * fogRange) / config.fullyVisibleLuxThreshold;

            region.SetBytes(x, y, Math.Min((byte)fog, visible[cell]));
          }
        }

        return false;
      }
    }
  }
}
