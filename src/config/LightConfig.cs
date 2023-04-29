using Newtonsoft.Json;
using UnityEngine;

namespace DarknessNotIncluded
{
  public enum LightShape
  {
    Circle,
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
    public LightShape shape { get; set; }
    [JsonConverter(typeof(ColorJsonConverter))]
    public Color color { get; set; }

    public LightConfig(bool enabled, int lux, int range, int reveal, LightShape shape, Color color)
    {
      this.enabled = enabled;
      this.lux = lux;
      this.range = range;
      this.reveal = reveal;
      this.shape = shape;
      this.color = color;
    }

    public void ConfigureLight(Light2D light)
    {
      light.enabled = this.enabled;
      light.shape = this.shape.LightShape();
      light.Lux = this.lux;
      light.Range = this.range;
      light.Color = this.color;
      light.FullRefresh();
    }

    public LightConfig DeepClone()
    {
      return new LightConfig(enabled, lux, range, reveal, shape, color);
    }

    public static LightConfig None = new LightConfig(false, 0, 0, 0, LightShape.SmoothCircle, new Color(0, 0, 0, 0));
  }
}
