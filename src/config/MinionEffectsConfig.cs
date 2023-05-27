using System.Collections.Generic;

namespace DarknessNotIncluded
{
  public enum MinionEffectType
  {
    Dim,
    Dark,
  }

  public class MinionEffectsConfig : Dictionary<MinionEffectType, MinionEffectsConfig.EffectConfig>
  {
    public MinionEffectsConfig()
    {
      Add(MinionEffectType.Dim, new EffectConfig(true, 700, -2, -1));
      Add(MinionEffectType.Dark, new EffectConfig(true, 300, -4, -2));
    }

    public class EffectConfig
    {
      public bool enabled { get; set; }
      public int luxThreshold { get; set; }
      public int agilityModifier { get; set; }
      public int statsModifier { get; set; }

      public EffectConfig(bool enabled, int luxThreshold, int agilityModifier, int athleticsModifier)
      {
        this.enabled = enabled;
        this.luxThreshold = luxThreshold;
        this.agilityModifier = agilityModifier;
        this.statsModifier = athleticsModifier;
      }

      public EffectConfig DeepClone()
      {
        return new EffectConfig(enabled, luxThreshold, agilityModifier, statsModifier);
      }
    }

    public MinionEffectsConfig DeepClone()
    {
      var newConfig = new MinionEffectsConfig();
      foreach (var pair in this)
      {
        newConfig[pair.Key] = pair.Value.DeepClone();
      }
      return newConfig;
    }
  }
}
