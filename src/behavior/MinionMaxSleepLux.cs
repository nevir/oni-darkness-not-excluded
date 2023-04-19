using HarmonyLib;

namespace DarknessNotIncluded
{
  [HarmonyPatch(typeof(SleepChore)), HarmonyPatch(nameof(SleepChore.IsDarkAtCell))]
  static class Patch
  {
    public static void Postfix(int cell, ref bool __result)
    {
      __result = Grid.LightIntensity[cell] <= Config.Instance.maxSleepingLux;
    }
  }
}
