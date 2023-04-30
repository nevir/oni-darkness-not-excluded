using System.Collections.Generic;
using TUNING;

namespace DarknessNotIncluded
{
  public enum BuildingType
  {
    None,
    PrintingPod,
    MicrobeMusher,
    HydrogenGenerator,
  }

  public class BuildingLightingConfig : Dictionary<BuildingType, LightConfig>
  {
    public BuildingLightingConfig()
    {
      Add(BuildingType.PrintingPod, new LightConfig(true, 2000, 5, 0, LightShape.Circle, LIGHT2D.LIGHT_YELLOW));
      Add(BuildingType.MicrobeMusher, new LightConfig(true, 1000, 1, 0, LightShape.Circle, LIGHT2D.LIGHT_YELLOW));
      Add(BuildingType.HydrogenGenerator, new LightConfig(true, 1000, 1, 0, LightShape.Circle, LIGHT2D.LIGHT_YELLOW));
    }

    public BuildingLightingConfig DeepClone()
    {
      var newConfig = new BuildingLightingConfig();
      foreach (var pair in this)
      {
        newConfig[pair.Key] = pair.Value.DeepClone();
      }
      return newConfig;
    }

    public LightConfig Get(BuildingType lightType)
    {
      return this.ContainsKey(lightType) ? this[lightType] : LightConfig.None;
    }
  }
}
