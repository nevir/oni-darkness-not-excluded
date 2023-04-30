using System.Collections.Generic;

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
    public MinionLightingConfig DeepClone()
    {
      var newConfig = new MinionLightingConfig();
      foreach (var pair in this)
      {
        newConfig.Add(pair.Key, pair.Value.DeepClone());
      }
      return newConfig;
    }

    public LightConfig Get(MinionLightType lightType)
    {
      return this.ContainsKey(lightType) ? this[lightType] : LightConfig.None;
    }
  }
}
