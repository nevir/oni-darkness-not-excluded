using HarmonyLib;
using UnityEngine;
using System;

namespace DarknessNotIncluded
{
  [HarmonyPatch(typeof(MinionConfig)), HarmonyPatch("CreatePrefab")]
  static class MinionLighting
  {
    static void Postfix(GameObject __result)
    {
      __result.AddOrGet<MinionLights>();
    }
  }

  class MinionLights : KMonoBehaviour, ISim33ms
  {
    enum MinionLightType
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

      SetLightType(GetCurrentLightType());
    }

    protected override void OnSpawn()
    {
      base.OnSpawn();

      SetLightType(GetCurrentLightType());
    }

    public void Sim33ms(float dt)
    {
      SetLightType(GetCurrentLightType());
    }

    private void SetLightType(MinionLightType lightType)
    {
      if (lightType == currentLightType) return;
      currentLightType = lightType;

      UpdateLights(lightType);
    }

    private void UpdateLights(MinionLightType lightType)
    {
      var config = Config.Instance;

      switch (lightType)
      {
        case MinionLightType.None:
          HeadLight.enabled = false;
          BodyLight.enabled = false;
          break;

        case MinionLightType.Intrinsic:
          HeadLight.enabled = true;
          HeadLight.Lux = config.dupeIntrinsicLightLux / 2;
          HeadLight.Range = config.dupeIntrinsicLightRadius;
          HeadLight.Color = config.dupeIntrinsicLightColor;
          BodyLight.enabled = true;
          BodyLight.Lux = config.dupeIntrinsicLightLux / 2;
          BodyLight.Range = config.dupeIntrinsicLightRadius;
          BodyLight.Color = config.dupeIntrinsicLightColor;
          break;

        case MinionLightType.Mining1:
          HeadLight.enabled = true;
          HeadLight.Lux = config.miningHatTier1Lux;
          HeadLight.Range = config.miningHatTier1Radius;
          HeadLight.Color = config.miningHatTier1Color;
          BodyLight.enabled = false;
          break;

        case MinionLightType.Mining2:
          HeadLight.enabled = true;
          HeadLight.Lux = config.miningHatTier2Lux;
          HeadLight.Range = config.miningHatTier2Radius;
          HeadLight.Color = config.miningHatTier2Color;
          BodyLight.enabled = false;
          break;

        case MinionLightType.Mining3:
          HeadLight.enabled = true;
          HeadLight.Lux = config.miningHatTier3Lux;
          HeadLight.Range = config.miningHatTier3Radius;
          HeadLight.Color = config.miningHatTier3Color;
          BodyLight.enabled = false;
          break;

        case MinionLightType.Mining4:
        case MinionLightType.MiningUnknown:
          HeadLight.enabled = true;
          HeadLight.Lux = config.miningHatTier4Lux;
          HeadLight.Range = config.miningHatTier4Radius;
          HeadLight.Color = config.miningHatTier4Color;
          BodyLight.enabled = false;
          break;

        case MinionLightType.Science:
          HeadLight.enabled = true;
          HeadLight.Lux = config.scienceHatLux;
          HeadLight.Range = config.scienceHatRadius;
          HeadLight.Color = config.scienceHatColor;
          BodyLight.enabled = false;
          break;

        case MinionLightType.Rocketry:
          HeadLight.enabled = true;
          HeadLight.Lux = config.rocketryHatLux;
          HeadLight.Range = config.rocketryHatRadius;
          HeadLight.Color = config.rocketryHatColor;
          BodyLight.enabled = false;
          break;
      }
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
