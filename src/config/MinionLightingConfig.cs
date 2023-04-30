using System.Collections.Generic;
using TUNING;
using UnityEngine;

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
    AtmoSuit,
    JetSuit,
    LeadSuit,
    Rover,
  }

  public class MinionLightingConfig : Dictionary<MinionLightType, LightConfig>
  {
    public MinionLightingConfig()
    {
      Add(MinionLightType.Intrinsic, new LightConfig(true, 200, 2, 0, LightShape.Pill, Color.white));
      Add(MinionLightType.Mining1, new LightConfig(true, 800, 3, 6, LightShape.DirectedCone, LIGHT2D.LIGHT_YELLOW));
      Add(MinionLightType.Mining2, new LightConfig(true, 1000, 4, 7, LightShape.DirectedCone, LIGHT2D.LIGHT_YELLOW));
      Add(MinionLightType.Mining3, new LightConfig(true, 1200, 5, 8, LightShape.DirectedCone, Color.white));
      Add(MinionLightType.Mining4, new LightConfig(true, 1400, 6, 9, LightShape.DirectedCone, Color.white));
      Add(MinionLightType.Science, new LightConfig(true, 800, 3, 0, LightShape.Pill, Color.white));
      Add(MinionLightType.Rocketry, new LightConfig(true, 800, 4, 0, LightShape.DirectedCone, Color.white));
      Add(MinionLightType.AtmoSuit, new LightConfig(true, 600, 3, 0, LightShape.Pill, LIGHT2D.LIGHT_YELLOW));
      Add(MinionLightType.JetSuit, new LightConfig(true, 800, 5, 7, LightShape.DirectedCone, LIGHT2D.LIGHT_YELLOW));
      Add(MinionLightType.LeadSuit, new LightConfig(true, 400, 3, 0, LightShape.Pill, LIGHT2D.LIGHT_YELLOW));
      Add(MinionLightType.Rover, new LightConfig(true, 1400, 6, 0, LightShape.DirectedCone, Color.white));
    }

    public MinionLightingConfig DeepClone()
    {
      var newConfig = new MinionLightingConfig();
      foreach (var pair in this)
      {
        newConfig[pair.Key] = pair.Value.DeepClone();
      }
      return newConfig;
    }

    public LightConfig Get(MinionLightType lightType)
    {
      return this.ContainsKey(lightType) ? this[lightType] : LightConfig.None;
    }
  }
}
