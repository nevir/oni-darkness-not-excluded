using HarmonyLib;
using UnityEngine;

namespace DarknessNotIncluded
{
  [HarmonyPatch(typeof(MinionConfig)), HarmonyPatch("CreatePrefab")]
  static class MinionLighting
  {
    static void Postfix(GameObject __result)
    {
      __result.AddOrGet<MinionIntrinsicLight>();
      __result.AddOrGet<MinionMinerHatLight>();
    }
  }

  class MinionIntrinsicLight : KMonoBehaviour
  {
    [MyCmpGet]
    private MinionIdentity _minionIdentity;

    public Light2D HeadLight { get; set; }
    public Light2D LegLight { get; set; }

    protected override void OnPrefabInit()
    {
      var config = Config.Instance;

      HeadLight = _minionIdentity.gameObject.AddComponent<Light2D>();
      HeadLight.Color = config.dupeIntrinsicLightColor;
      HeadLight.Offset = new Vector2(0f, 1f);
      HeadLight.Range = config.dupeIntrinsicLightRadius;
      HeadLight.Lux = config.dupeIntrinsicLightLux / 2;
      HeadLight.shape = LightShape.Circle;
      HeadLight.enabled = config.dupeIntrinsicLightEnabled;

      LegLight = _minionIdentity.gameObject.AddComponent<Light2D>();
      LegLight.Color = config.dupeIntrinsicLightColor;
      LegLight.Offset = new Vector2(0f, 0f);
      LegLight.Range = config.dupeIntrinsicLightRadius;
      LegLight.Lux = config.dupeIntrinsicLightLux / 2;
      LegLight.shape = LightShape.Circle;
      LegLight.enabled = config.dupeIntrinsicLightEnabled;
    }
  }

  class MinionMinerHatLight : KMonoBehaviour, ISim33ms
  {
    [MyCmpGet]
    private MinionIdentity _minionIdentity;

    public Light2D Light { get; set; }

    private string currentHat;

    protected override void OnPrefabInit()
    {
      var resume = _minionIdentity.GetComponent<MinionResume>();
      currentHat = resume.CurrentHat;

      Light = _minionIdentity.gameObject.AddComponent<Light2D>();
      Light.Offset = new Vector2(0f, 1.0f);
      Light.IntensityAnimation = 1f;
      UpdateLightConfig();
    }

    public void Sim33ms(float dt)
    {
      var hat = _minionIdentity.GetComponent<MinionResume>().CurrentHat;
      if (hat == currentHat) return;

      currentHat = hat;
      UpdateLightConfig();
    }

    private void UpdateLightConfig()
    {
      var config = Config.Instance;

      switch (currentHat)
      {
        case "hat_role_mining1":
          Light.enabled = config.miningHatTier1Enabled;
          Light.Lux = config.miningHatTier1Lux;
          Light.Range = config.miningHatTier1Radius;
          Light.Color = config.miningHatTier1Color;
          break;

        case "hat_role_mining2":
          Light.enabled = config.miningHatTier2Enabled;
          Light.Lux = config.miningHatTier2Lux;
          Light.Range = config.miningHatTier2Radius;
          Light.Color = config.miningHatTier2Color;
          break;

        case "hat_role_mining3":
          Light.enabled = config.miningHatTier3Enabled;
          Light.Lux = config.miningHatTier3Lux;
          Light.Range = config.miningHatTier3Radius;
          Light.Color = config.miningHatTier3Color;
          break;

        case "hat_role_mining4":
          Light.enabled = config.miningHatTier4Enabled;
          Light.Lux = config.miningHatTier4Lux;
          Light.Range = config.miningHatTier4Radius;
          Light.Color = config.miningHatTier4Color;
          break;

        default:
          Light.enabled = false;
          break;
      }
    }
  }
}
