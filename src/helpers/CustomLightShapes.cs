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
        case DarknessNotIncluded.LightShape.Pill: return CustomLightShapes.Pill.KleiLightShape;
        case DarknessNotIncluded.LightShape.DirectedCone: return CustomLightShapes.DirectedCone.KleiLightShape;
        default: return global::LightShape.Circle;
      }
    }
  }

  public static class CustomLightShapes
  {
    public static ILightShape SmoothCircle;
    public static ILightShape Pill;
    public static ILightShape DirectedCone;

    public static void Initialize()
    {
      PLightManager lightManager = new PLightManager();
      SmoothCircle = lightManager.Register("nevir.DarknessNotExcluded.SmoothCircle", CustomLightShapes.SmoothCircleCaster);
      Pill = lightManager.Register("nevir.DarknessNotExcluded.Pill", CustomLightShapes.PillCaster);
      DirectedCone = lightManager.Register("nevir.DarknessNotExcluded.DirectedCone", CustomLightShapes.DirectedConeCaster);
    }

    public static void SmoothCircleCaster(LightingArgs args)
    {
      int sourceCell = args.SourceCell;
      int range = args.Range;
      var brightness = args.Brightness;

      CastSmoothCircle(brightness, range, sourceCell);
    }

    public static void PillCaster(LightingArgs args)
    {
      int sourceCell = args.SourceCell;
      int range = args.Range;
      var brightness = args.Brightness;

      CastSmoothCircle(brightness, range, sourceCell);
      CastSmoothCircle(brightness, range, Grid.CellAbove(sourceCell));
    }

    public static void DirectedConeCaster(LightingArgs args)
    {
      var unitOrientation = args.Source.AddOrGet<UnitOrientation>();
      var facing = args.Source.GetComponent<Facing>();
      var animController = args.Source.GetComponent<KBatchedAnimController>();
      int sourceCell = args.SourceCell;
      int range = args.Range;
      var brightness = args.Brightness;

      PillCaster(args);
      MultiplyBrightness(brightness, 0.35f);
      // Make sure the cell the minion stands in (their feet) is also max 
      // brightness.
      brightness[sourceCell] = 1.0f;

      var octants = new OctantBuilder(brightness, Grid.CellAbove(sourceCell))
      {
        Falloff = 0.5f,
        SmoothLight = true
      };

      switch (unitOrientation.orientation)
      {
        case UnitOrientation.Orientation.Left:
          octants.AddOctant(range, DiscreteShadowCaster.Octant.W_SW);
          octants.AddOctant(range, DiscreteShadowCaster.Octant.W_NW);
          break;

        case UnitOrientation.Orientation.UpLeft:
          octants.AddOctant(range, DiscreteShadowCaster.Octant.W_NW);
          octants.AddOctant(range, DiscreteShadowCaster.Octant.N_NW);
          break;

        case UnitOrientation.Orientation.Up:
          octants.AddOctant(range, DiscreteShadowCaster.Octant.N_NW);
          octants.AddOctant(range, DiscreteShadowCaster.Octant.N_NE);
          break;

        case UnitOrientation.Orientation.UpRight:
          octants.AddOctant(range, DiscreteShadowCaster.Octant.N_NE);
          octants.AddOctant(range, DiscreteShadowCaster.Octant.E_NE);
          break;

        case UnitOrientation.Orientation.Right:
          octants.AddOctant(range, DiscreteShadowCaster.Octant.E_NE);
          octants.AddOctant(range, DiscreteShadowCaster.Octant.E_SE);
          break;

        case UnitOrientation.Orientation.DownRight:
          octants.AddOctant(range, DiscreteShadowCaster.Octant.E_SE);
          octants.AddOctant(range, DiscreteShadowCaster.Octant.S_SE);
          break;

        case UnitOrientation.Orientation.Down:
        default:
          octants.AddOctant(range, DiscreteShadowCaster.Octant.S_SE);
          octants.AddOctant(range, DiscreteShadowCaster.Octant.S_SW);
          break;

        case UnitOrientation.Orientation.DownLeft:
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
