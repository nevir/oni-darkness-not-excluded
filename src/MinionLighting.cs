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
      HeadLight = _minionIdentity.gameObject.AddComponent<Light2D>();
      HeadLight.Color = Color.white;
      HeadLight.Offset = new Vector2(0f, 1f);
      HeadLight.Range = 1;
      HeadLight.Lux = 100;
      HeadLight.shape = LightShape.Circle;
      HeadLight.enabled = true;

      LegLight = _minionIdentity.gameObject.AddComponent<Light2D>();
      LegLight.Color = Color.white;
      LegLight.Offset = new Vector2(0f, 0f);
      LegLight.Range = 1;
      LegLight.Lux = 100;
      LegLight.shape = LightShape.Circle;
      LegLight.enabled = true;
    }
  }
}
