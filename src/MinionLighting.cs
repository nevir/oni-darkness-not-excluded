using HarmonyLib;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace DarknessNotIncluded
{
  [HarmonyPatch(typeof(Game)), HarmonyPatch("OnPrefabInit")]
  public static class MinionLightingConfig
  {
    public struct MinionLightConfig
    {
      public MinionLightConfig(bool enabled, int lux, int range, Color color)
      {
        this.enabled = enabled;
        this.lux = lux;
        this.range = range;
        this.color = color;
      }

      public bool enabled { get; }
      public int lux { get; }
      public int range { get; }
      public Color color { get; }
    }

    public struct MinionLightsConfig
    {
      public MinionLightsConfig(MinionLightConfig head, MinionLightConfig body)
      {
        this.head = head;
        this.body = body;
      }

      public MinionLightConfig head { get; }
      public MinionLightConfig body { get; }
    }

    public static Dictionary<MinionLights.MinionLightType, MinionLightsConfig> lightConfigsByType;

    static void Prefix()
    {
      var config = Config.Instance;

      var nonLight = new MinionLightConfig(false, 0, 0, Color.clear);

      MinionLightingConfig.lightConfigsByType = new Dictionary<MinionLights.MinionLightType, MinionLightsConfig> {
        { MinionLights.MinionLightType.None, new MinionLightsConfig(
          nonLight,
          nonLight
        ) },
        { MinionLights.MinionLightType.Intrinsic, new MinionLightsConfig(
          new MinionLightConfig(config.dupeIntrinsicLightEnabled, config.dupeIntrinsicLightLux / 2, config.dupeIntrinsicLightRadius, config.dupeIntrinsicLightColor),
          new MinionLightConfig(config.dupeIntrinsicLightEnabled, config.dupeIntrinsicLightLux / 2, config.dupeIntrinsicLightRadius, config.dupeIntrinsicLightColor)
        ) },
        { MinionLights.MinionLightType.Mining1, new MinionLightsConfig(
          new MinionLightConfig(config.miningHatTier1Enabled, config.miningHatTier1Lux, config.miningHatTier1Radius, config.miningHatTier1Color),
          nonLight
        ) },
        { MinionLights.MinionLightType.Mining2, new MinionLightsConfig(
          new MinionLightConfig(config.miningHatTier2Enabled, config.miningHatTier2Lux, config.miningHatTier2Radius, config.miningHatTier2Color),
          nonLight
        ) },
        { MinionLights.MinionLightType.Mining3, new MinionLightsConfig(
          new MinionLightConfig(config.miningHatTier3Enabled, config.miningHatTier3Lux, config.miningHatTier3Radius, config.miningHatTier3Color),
          nonLight
        ) },
        { MinionLights.MinionLightType.Mining4, new MinionLightsConfig(
          new MinionLightConfig(config.miningHatTier4Enabled, config.miningHatTier4Lux, config.miningHatTier4Radius, config.miningHatTier4Color),
          nonLight
        ) },
        { MinionLights.MinionLightType.MiningUnknown, new MinionLightsConfig(
          new MinionLightConfig(config.miningHatTier4Enabled, config.miningHatTier4Lux, config.miningHatTier4Radius, config.miningHatTier4Color),
          nonLight
        ) },
        { MinionLights.MinionLightType.Science, new MinionLightsConfig(
          new MinionLightConfig(config.scienceHatEnabled, config.scienceHatLux, config.scienceHatRadius, config.scienceHatColor),
          nonLight
        ) },
        { MinionLights.MinionLightType.Rocketry, new MinionLightsConfig(
          new MinionLightConfig(config.rocketryHatEnabled, config.rocketryHatLux, config.rocketryHatRadius, config.rocketryHatColor),
          nonLight
        ) },
      };
    }
  }

  [HarmonyPatch(typeof(MinionConfig)), HarmonyPatch("CreatePrefab")]
  static class MinionLighting
  {
    static void Postfix(GameObject __result)
    {
      __result.AddOrGet<MinionLights>();
    }
  }

  public class MinionLights : KMonoBehaviour, ISim33ms
  {
    public enum MinionLightType
    {
      None,
      Intrinsic,
      Mining1,
      Mining2,
      Mining3,
      Mining4,
      MiningUnknown,
      Science,
      Rocketry,
    }

    [MyCmpGet]
    private MinionIdentity _minionIdentity;

    public Light2D HeadLight { get; set; }
    // Only used for intrinsic lighting
    public Light2D BodyLight { get; set; }

    private MinionLightType currentLightType;

    protected override void OnPrefabInit()
    {
      base.OnPrefabInit();

      HeadLight = _minionIdentity.gameObject.AddComponent<Light2D>();
      HeadLight.shape = LightShape.Circle;
      HeadLight.Offset = new Vector2(0f, 1.0f);

      BodyLight = _minionIdentity.gameObject.AddComponent<Light2D>();
      BodyLight.shape = LightShape.Circle;
      BodyLight.Offset = new Vector2(0f, 1.0f);

      UpdateLights();
    }

    protected override void OnSpawn()
    {
      base.OnSpawn();

      UpdateLights();
    }

    public void Sim33ms(float dt)
    {
      UpdateLights();
    }

    private void UpdateLights()
    {
      var config = Config.Instance;
      var lightType = GetCurrentLightType();
      var initialProperties = MinionLightingConfig.lightConfigsByType[lightType];

      if (config.disableDupeLightsInLitAreas && lightType != MinionLightType.None)
      {
        var headCell = Grid.CellAbove(Grid.PosToCell(_minionIdentity.gameObject));
        var dupeLux = (HeadLight.enabled ? HeadLight.Lux : 0) + (BodyLight.enabled ? BodyLight.Lux : 0);
        var baseCellLux = Grid.LightIntensity[headCell] - dupeLux;
        var targetLux = (initialProperties.head.enabled ? initialProperties.head.lux : 0) + (initialProperties.body.enabled ? initialProperties.body.lux : 0);
        // Keep intrinsic lights on even if next to another dupe
        if (lightType == MinionLightType.Intrinsic) targetLux *= 2;

        if (baseCellLux >= targetLux)
        {
          lightType = MinionLightType.None;
        }
      }

      if (lightType == currentLightType) return;
      currentLightType = lightType;

      var properties = MinionLightingConfig.lightConfigsByType[lightType];

      HeadLight.enabled = properties.head.enabled;
      HeadLight.Lux = properties.head.lux;
      HeadLight.Range = properties.head.range;
      HeadLight.Color = properties.head.color;

      BodyLight.enabled = properties.body.enabled;
      BodyLight.Lux = properties.body.lux;
      BodyLight.Range = properties.body.range;
      BodyLight.Color = properties.body.color;
    }

    private MinionLightType GetCurrentLightType()
    {
      var config = Config.Instance;

      var resume = _minionIdentity.GetComponent<MinionResume>();
      var hat = resume.CurrentHat;

      if (hat != null && hat.StartsWith("hat_role_mining"))
      {
        if (hat == "hat_role_mining1")
        {
          return config.miningHatTier1Enabled ? MinionLightType.Mining1 : MinionLightType.Intrinsic;
        }
        else if (hat == "hat_role_mining2")
        {
          return config.miningHatTier2Enabled ? MinionLightType.Mining2 : MinionLightType.Intrinsic;
        }
        else if (hat == "hat_role_mining3")
        {
          return config.miningHatTier3Enabled ? MinionLightType.Mining3 : MinionLightType.Intrinsic;
        }
        else if (hat == "hat_role_mining4")
        {
          return config.miningHatTier4Enabled ? MinionLightType.Mining4 : MinionLightType.Intrinsic;
        }
        else
        {
          return config.miningHatTier4Enabled ? MinionLightType.MiningUnknown : MinionLightType.Intrinsic;
        }
      }
      else if (hat != null && hat.StartsWith("hat_role_research"))
      {
        return config.scienceHatEnabled ? MinionLightType.Science : MinionLightType.Intrinsic;
      }
      else if (hat != null && hat.StartsWith("hat_role_astronaut"))
      {
        return config.scienceHatEnabled ? MinionLightType.Rocketry : MinionLightType.Intrinsic;
      }
      else
      {
        return MinionLightType.Intrinsic;
      }
    }
  }
}
