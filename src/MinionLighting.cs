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
}
