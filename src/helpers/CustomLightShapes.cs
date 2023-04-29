using PeterHan.PLib.Lighting;
using System.Collections.Generic;

namespace DarknessNotIncluded
{
  public static class LightShapeExtensions
  {
    public static global::LightShape LightShape(this LightShape shape)
    {
      switch (shape)
      {
        case DarknessNotIncluded.LightShape.Circle: return global::LightShape.Circle;
        case DarknessNotIncluded.LightShape.SmoothCircle: return CustomLightShapes.SmoothCircle.KleiLightShape;
        case DarknessNotIncluded.LightShape.MinionPill: return CustomLightShapes.MinionPill.KleiLightShape;
        case DarknessNotIncluded.LightShape.MinionDirectedCone: return CustomLightShapes.MinionDirectedCone.KleiLightShape;
        default: return global::LightShape.Circle;
      }
    }
  }

  public static class CustomLightShapes
  {
    public static ILightShape SmoothCircle;
    public static ILightShape MinionPill;
    public static ILightShape MinionDirectedCone;

    public static void Initialize()
    {
      PLightManager lightManager = new PLightManager();
      SmoothCircle = lightManager.Register("nevir.DarknessNotExcluded.SmoothCircle", CustomLightShapes.SmoothCircleCaster);
      MinionPill = lightManager.Register("nevir.DarknessNotExcluded.MinionPill", CustomLightShapes.MinionPillCaster);
      MinionDirectedCone = lightManager.Register("nevir.DarknessNotExcluded.MinionDirectedCone", CustomLightShapes.MinionDirectedConeCaster);
    }

    public static void SmoothCircleCaster(LightingArgs args)
    {
      int sourceCell = args.SourceCell;
      int range = args.Range;
      var brightness = args.Brightness;

      CastSmoothCircle(brightness, range, sourceCell);
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
      // Make sure the cell the minion stands in (their feet) is also max 
      // brightness.
      brightness[sourceCell] = 1.0f;

      var octants = new OctantBuilder(brightness, Grid.CellAbove(sourceCell))
      {
        Falloff = 0.5f,
        SmoothLight = true
      };

      switch (minionOrientation.orientation)
      {
        case MinionOrientation.Orientation.Left:
          octants.AddOctant(range, DiscreteShadowCaster.Octant.W_SW);
          octants.AddOctant(range, DiscreteShadowCaster.Octant.W_NW);
          break;

        case MinionOrientation.Orientation.UpLeft:
          octants.AddOctant(range, DiscreteShadowCaster.Octant.W_NW);
          octants.AddOctant(range, DiscreteShadowCaster.Octant.N_NW);
          break;

        case MinionOrientation.Orientation.Up:
          octants.AddOctant(range, DiscreteShadowCaster.Octant.N_NW);
          octants.AddOctant(range, DiscreteShadowCaster.Octant.N_NE);
          break;

        case MinionOrientation.Orientation.UpRight:
          octants.AddOctant(range, DiscreteShadowCaster.Octant.N_NE);
          octants.AddOctant(range, DiscreteShadowCaster.Octant.E_NE);
          break;

        case MinionOrientation.Orientation.Right:
          octants.AddOctant(range, DiscreteShadowCaster.Octant.E_NE);
          octants.AddOctant(range, DiscreteShadowCaster.Octant.E_SE);
          break;

        case MinionOrientation.Orientation.DownRight:
          octants.AddOctant(range, DiscreteShadowCaster.Octant.E_SE);
          octants.AddOctant(range, DiscreteShadowCaster.Octant.S_SE);
          break;

        case MinionOrientation.Orientation.Down:
        default:
          octants.AddOctant(range, DiscreteShadowCaster.Octant.S_SE);
          octants.AddOctant(range, DiscreteShadowCaster.Octant.S_SW);
          break;

        case MinionOrientation.Orientation.DownLeft:
          octants.AddOctant(range, DiscreteShadowCaster.Octant.S_SW);
          octants.AddOctant(range, DiscreteShadowCaster.Octant.W_SW);
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
