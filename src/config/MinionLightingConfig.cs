using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace DarknessNotIncluded
{
  public enum MinionLightShape
  {
    Pill,
    DirectedCone,
  }

  public enum MinionLightType
  {
    None,
    Intrinsic,
    Mining1,
    Mining2,
    Mining3,
    Mining4,
    Science,
    Rocketry,
    AtmoSuit,
    JetSuit,
    LeadSuit,
  }

  public static class MinionLightShapeExtensions
  {
    public static LightShape LightShape(this MinionLightShape shape)
    {
      if (shape == MinionLightShape.DirectedCone)
      {
        return CustomLightShapes.MinionDirectedCone.KleiLightShape;
      }
      else
      {
        return CustomLightShapes.MinionPill.KleiLightShape;
      }
    }
  }

  public static class MinionLightTypeExtensions
  {
    public static MinionLightingConfig.LightConfig Config(this MinionLightType value)
    {
      var lightingConfig = DarknessNotIncluded.Config.Instance.minionLightingConfig;
      return lightingConfig.ContainsKey(value) ? lightingConfig[value] : MinionLightingConfig.LightConfig.None;
    }
  }

  public class MinionLightingConfig : Dictionary<MinionLightType, MinionLightingConfig.LightConfig>
  {
    public class LightConfig
    {
      public bool enabled { get; set; }
      public int lux { get; set; }
      public int range { get; set; }
      public MinionLightShape shape { get; set; }
      [JsonConverter(typeof(ColorJsonConverter))]
      public Color color { get; set; }

      public LightConfig(bool enabled, int lux, int range, MinionLightShape shape, Color color)
      {
        this.enabled = enabled;
        this.lux = lux;
        this.range = range;
        this.shape = shape;
        this.color = color;
      }

      public LightConfig DeepClone()
      {
        return new LightConfig(enabled, lux, range, shape, color);
      }

      public static LightConfig None = new LightConfig(false, 0, 0, MinionLightShape.Pill, new Color(0, 0, 0, 0));
    }

    public MinionLightingConfig DeepClone()
    {
      var newConfig = new MinionLightingConfig();
      foreach (var pair in this)
      {
        newConfig.Add(pair.Key, pair.Value.DeepClone());
      }
      return newConfig;
    }
  }
}
