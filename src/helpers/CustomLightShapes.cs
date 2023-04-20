using PeterHan.PLib.Lighting;
using System.Collections.Generic;

namespace DarknessNotIncluded
{
  public static class CustomLightShapes
  {
    public static ILightShape MinionPill;
    public static ILightShape MinionDirectedCone;

    public static void Initialize()
    {
      PLightManager lightManager = new PLightManager();
      MinionPill = lightManager.Register("nevir.DarknessNotExcluded.MinionPill", CustomLightShapes.MinionPillCaster);
      MinionDirectedCone = lightManager.Register("nevir.DarknessNotExcluded.MinionDirectedCone", CustomLightShapes.MinionDirectedConeCaster);
    }

    public static void MinionPillCaster(LightingArgs args)
    {
      int sourceCell = args.SourceCell;
      int range = args.Range;
      var brightness = args.Brightness;

      CastSmoothCircle(brightness, range, sourceCell);
      CastSmoothCircle(brightness, range, Grid.CellAbove(sourceCell));
    }

    public static void MinionDirectedConeCaster(LightingArgs args)
    {
      var minionOrientation = args.Source.GetComponent<MinionOrientation>();
      var facing = args.Source.GetComponent<Facing>();
      var animController = args.Source.GetComponent<KBatchedAnimController>();
      int sourceCell = args.SourceCell;
      int range = args.Range;
      var brightness = args.Brightness;

      MinionPillCaster(args);
      MultiplyBrightness(brightness, 0.35f);

      var octants = new OctantBuilder(brightness, Grid.CellAbove(sourceCell))
      {
        Falloff = 0.5f,
        SmoothLight = true
      };

      switch (minionOrientation.orientation)
      {
        case MinionOrientation.Orientation.Left:
          octants.AddOctant(range, DiscreteShadowCaster.Octant.W_NW);
          octants.AddOctant(range, DiscreteShadowCaster.Octant.W_SW);
          break;

        case MinionOrientation.Orientation.Right:
          octants.AddOctant(range, DiscreteShadowCaster.Octant.E_NE);
          octants.AddOctant(range, DiscreteShadowCaster.Octant.E_SE);
          break;

        case MinionOrientation.Orientation.Up:
          octants.AddOctant(range, DiscreteShadowCaster.Octant.N_NW);
          octants.AddOctant(range, DiscreteShadowCaster.Octant.N_NE);
          break;

        case MinionOrientation.Orientation.Down:
        default:
          octants.AddOctant(range, DiscreteShadowCaster.Octant.S_SW);
          octants.AddOctant(range, DiscreteShadowCaster.Octant.S_SE);
          break;
      }
    }

    // Helpers

    static void CastSmoothCircle(IDictionary<int, float> brightness, int range, int sourceCell)
    {
      var octants = new OctantBuilder(brightness, sourceCell)
      {
        Falloff = 0.5f,
        SmoothLight = true
      };
      octants.AddOctant(range, DiscreteShadowCaster.Octant.E_NE);
      octants.AddOctant(range, DiscreteShadowCaster.Octant.E_SE);
      octants.AddOctant(range, DiscreteShadowCaster.Octant.N_NE);
      octants.AddOctant(range, DiscreteShadowCaster.Octant.N_NW);
      octants.AddOctant(range, DiscreteShadowCaster.Octant.S_SE);
      octants.AddOctant(range, DiscreteShadowCaster.Octant.S_SW);
      octants.AddOctant(range, DiscreteShadowCaster.Octant.W_NW);
      octants.AddOctant(range, DiscreteShadowCaster.Octant.W_SW);
    }

    static void MultiplyBrightness(IDictionary<int, float> brightness, float factor)
    {
      foreach (var key in new List<int>(brightness.Keys))
      {
        brightness[key] *= factor;
      }
    }
  }
}
