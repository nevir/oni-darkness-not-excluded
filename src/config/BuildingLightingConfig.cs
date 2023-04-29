using System.Collections.Generic;

namespace DarknessNotIncluded
{
  public enum BuildingType
  {
    None,
    PrintingPod,
  }

  public class BuildingLightingConfig : Dictionary<BuildingType, LightConfig>
  {
    public BuildingLightingConfig DeepClone()
    {
      var newConfig = new BuildingLightingConfig();
      foreach (var pair in this)
      {
        newConfig.Add(pair.Key, pair.Value.DeepClone());
      }
      return newConfig;
    }

    public LightConfig Get(BuildingType lightType)
    {
      return this.ContainsKey(lightType) ? this[lightType] : LightConfig.None;
    }
  }
}
