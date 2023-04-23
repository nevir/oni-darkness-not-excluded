using HarmonyLib;

namespace DarknessNotIncluded.DarknessPenalties
{
  public static class MinionMaxSleepLux
  {
    [HarmonyPatch(typeof(SleepChore)), HarmonyPatch(nameof(SleepChore.IsDarkAtCell))]
    static class Patched_SleepChore_IsDarkAtCell
    {
      static void Postfix(int cell, ref bool __result)
      {
        __result = Grid.LightIntensity[cell] <= Config.Instance.maxSleepingLux;
      }
    }
  }

}
