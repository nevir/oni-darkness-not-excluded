using HarmonyLib;
using System.Collections.Generic;

namespace DarknessNotIncluded.DarknessPenalties
{
  public static class MinionSleepDisturbanceTime
  {
    private static Dictionary<int, int> DISTURBED_TICKS = new Dictionary<int, int>();

    private static int sleepingDisturbedTicks;
    private static Config.Observer configObserver = new Config.Observer((config) =>
    {
      sleepingDisturbedTicks = config.sleepingDisturbedTicks;
    });

    [HarmonyPatch(typeof(SleepChore.States)), HarmonyPatch("InitializeStates")]
    static class Patched_SleepChore_States_InitializeStates
    {
      static void Prefix(SleepChore.States __instance)
      {
        __instance.sleep.normal.Enter(smi =>
        {
          var sleeperId = __instance.sleeper.Get(smi).GetInstanceID();
          DISTURBED_TICKS[sleeperId] = 0;
        });
      }
    }

    [HarmonyPatch(typeof(SleepChore.StatesInstance)), HarmonyPatch("CheckLightLevel")]
    static class Patched_SleepChore_StatesInstance_CheckLightLevel
    {
      static bool Prefix(SleepChore.StatesInstance __instance)
      {
        var smi = Traverse.Create(__instance).Property("smi").GetValue<SleepChore.StatesInstance>();
        var sleeper = __instance.sm.sleeper.Get(smi);
        int cell = Grid.PosToCell(sleeper);
        if (!Grid.IsValidCell(cell)) return false;

        var sleeperId = sleeper.GetInstanceID();
        var isDark = SleepChore.IsDarkAtCell(cell);
        var isNyctophobic = __instance.sm.needsNightLight.Get(smi);

        if (isDark == isNyctophobic)
        {
          DISTURBED_TICKS[sleeperId] += 1;
        }
        else if (DISTURBED_TICKS[sleeperId] > 0)
        {
          DISTURBED_TICKS[sleeperId] -= 1;
        }

        if (DISTURBED_TICKS[sleeperId] <= sleepingDisturbedTicks) return false;

        if (isNyctophobic)
        {
          sleeper.Trigger((int)GameHashes.SleepDisturbedByFearOfDark);
        }
        else if (!__instance.IsLoudSleeper() && !__instance.IsGlowStick())
        {
          sleeper.Trigger((int)GameHashes.SleepDisturbedByLight);
        }

        return false;
      }
    }
  }

}
