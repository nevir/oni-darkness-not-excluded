using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace DarknessNotIncluded
{
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
  }

  public static class MinionLightTypeExtension
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
      public LightConfig(bool enabled, int lux, int range, Color color)
      {
        this.enabled = enabled;
        this.lux = lux;
        this.range = range;
        this.color = color;
      }

      public bool enabled { get; set; }
      public int lux { get; set; }
      public int range { get; set; }
      [JsonConverter(typeof(ColorJsonConverter))]
      public Color color { get; set; }

      public LightConfig DeepClone()
      {
        return new LightConfig(enabled, lux, range, color);
      }

      public static LightConfig None = new LightConfig(false, 0, 0, new Color(0, 0, 0, 0));
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
