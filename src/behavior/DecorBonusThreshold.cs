using HarmonyLib;

namespace DarknessNotIncluded
{
  public static class DecorBonusThreshold
  {
    [HarmonyPatch(typeof(DecorProvider)), HarmonyPatch(nameof(DecorProvider.GetLightDecorBonus))]
    static class Patched_DecorProvider_GetLightDecorBonus
    {
      static void Postfix(int cell, ref int __result)
      {
        __result = Grid.LightIntensity[cell] >= Config.Instance.decorBonusThresholdLux ? TUNING.DECOR.LIT_BONUS : 0;
      }
    }
  }

}
