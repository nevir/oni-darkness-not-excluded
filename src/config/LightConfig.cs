using Newtonsoft.Json;
using UnityEngine;

namespace DarknessNotIncluded
{
  public enum CustomLightShape
  {
    SmoothCircle,
    MinionPill,
    MinionDirectedCone,
  }

  public class LightConfig
  {
    public bool enabled { get; set; }
    public int lux { get; set; }
    public int range { get; set; }
    public int reveal { get; set; }
    public CustomLightShape shape { get; set; }
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color color { get; set; }

    public LightConfig(bool enabled, int lux, int range, int reveal, CustomLightShape shape, Color color)
    {
      this.enabled = enabled;
      this.lux = lux;
      this.range = range;
      this.reveal = reveal;
      this.shape = shape;
      this.color = color;
    }

    public LightConfig DeepClone()
    {
      return new LightConfig(enabled, lux, range, reveal, shape, color);
    }

    public static LightConfig None = new LightConfig(false, 0, 0, 0, CustomLightShape.SmoothCircle, new Color(0, 0, 0, 0));
  }
}
