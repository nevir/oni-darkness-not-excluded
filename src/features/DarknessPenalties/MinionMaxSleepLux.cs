using HarmonyLib;

namespace DarknessNotIncluded.DarknessPenalties
{
  public static class MinionMaxSleepLux
  {
    private static int maxSleepingLux;

    private static Config.Observer configObserver = new Config.Observer((config) =>
    {
      maxSleepingLux = config.maxSleepingLux;
    });

    [HarmonyPatch(typeof(SleepChore)), HarmonyPatch("IsDarkAtCell")]
    static class Patched_SleepChore_IsDarkAtCell
    {
      static void Postfix(int cell, ref bool __result)
      {
        __result = Grid.LightIntensity[cell] <= maxSleepingLux;
      }
    }
  }

}
