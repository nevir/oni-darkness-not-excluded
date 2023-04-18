using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;

namespace DarknessNotIncluded
{
  [HarmonyPatch(typeof(Game)), HarmonyPatch("OnPrefabInit")]
  public class MinionLightingConfig
  {
    public MinionLightingConfig(bool enabled, int lux, int range, Color color)
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

    public static Dictionary<MinionLights.MinionLightType, MinionLightingConfig> byType;

    static void Prefix()
    {
      var config = Config.Instance;

      MinionLightingConfig.byType = new Dictionary<MinionLights.MinionLightType, MinionLightingConfig> {
        { MinionLights.MinionLightType.None, new MinionLightingConfig(false, 0, 0, Color.clear) },
        { MinionLights.MinionLightType.Intrinsic, new MinionLightingConfig(config.dupeIntrinsicLightEnabled, config.dupeIntrinsicLightLux, config.dupeIntrinsicLightRadius, config.dupeIntrinsicLightColor) },
        { MinionLights.MinionLightType.Mining1, new MinionLightingConfig(config.miningHatTier1Enabled, config.miningHatTier1Lux, config.miningHatTier1Radius, config.miningHatTier1Color)},
        { MinionLights.MinionLightType.Mining2, new MinionLightingConfig(config.miningHatTier2Enabled, config.miningHatTier2Lux, config.miningHatTier2Radius, config.miningHatTier2Color)},
        { MinionLights.MinionLightType.Mining3, new MinionLightingConfig(config.miningHatTier3Enabled, config.miningHatTier3Lux, config.miningHatTier3Radius, config.miningHatTier3Color)},
        { MinionLights.MinionLightType.Mining4, new MinionLightingConfig(config.miningHatTier4Enabled, config.miningHatTier4Lux, config.miningHatTier4Radius, config.miningHatTier4Color)},
        { MinionLights.MinionLightType.MiningUnknown, new MinionLightingConfig(config.miningHatTier4Enabled, config.miningHatTier4Lux, config.miningHatTier4Radius, config.miningHatTier4Color)},
        { MinionLights.MinionLightType.Science, new MinionLightingConfig(config.scienceHatEnabled, config.scienceHatLux, config.scienceHatRadius, config.scienceHatColor)},
        { MinionLights.MinionLightType.Rocketry, new MinionLightingConfig(config.rocketryHatEnabled, config.rocketryHatLux, config.rocketryHatRadius, config.rocketryHatColor)},
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
      BodyLight.Offset = new Vector2(0f, 0.0f);

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
      var lightingConfig = MinionLightingConfig.byType[lightType];

      if (config.disableDupeLightsInBedrooms)
      {
        Room currentRoom = Game.Instance.roomProber.GetRoomOfGameObject(this._minionIdentity.gameObject);
        var isInBedroom = currentRoom != null && currentRoom.roomType.category == Db.Get().RoomTypeCategories.Sleep;
        if (isInBedroom)
        {
          lightType = MinionLightType.None;
        }
      }

      if (config.disableDupeLightsInLitAreas && lightType != MinionLightType.None)
      {
        var headCell = Grid.CellAbove(Grid.PosToCell(_minionIdentity.gameObject));
        var dupeLux = (HeadLight.enabled ? HeadLight.Lux : 0) + (BodyLight.enabled ? BodyLight.Lux : 0);
        var baseCellLux = Grid.LightIntensity[headCell] - dupeLux;
        var targetLux = lightingConfig.enabled ? lightingConfig.lux : 0;
        // Keep intrinsic lights on even if next to another dupe
        if (lightType == MinionLightType.Intrinsic) targetLux *= 2;

        if (baseCellLux >= targetLux)
        {
          lightType = MinionLightType.None;
        }
      }

      SetLightType(lightType);
    }

    private void SetLightType(MinionLightType lightType)
    {
      if (lightType == currentLightType) return;
      currentLightType = lightType;

      var lightingConfig = MinionLightingConfig.byType[lightType];

      HeadLight.enabled = lightingConfig.enabled;
      HeadLight.Lux = lightingConfig.lux / 2;
      HeadLight.Range = lightingConfig.range;
      HeadLight.Color = lightingConfig.color;

      BodyLight.enabled = lightingConfig.enabled;
      BodyLight.Lux = lightingConfig.lux / 2;
      BodyLight.Range = lightingConfig.range;
      BodyLight.Color = lightingConfig.color;
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
