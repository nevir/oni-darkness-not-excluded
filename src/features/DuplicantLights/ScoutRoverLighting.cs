using HarmonyLib;
using UnityEngine;

namespace DarknessNotIncluded.DuplicantLights
{
  public static class ScoutRoverLighting
  {
    [HarmonyPatch(typeof(ScoutRoverConfig)), HarmonyPatch("CreatePrefab")]
    static class Patched_ScoutRoverConfig_CreatePrefab
    {
      static void Postfix(GameObject __result)
      {
        __result.AddOrGet<ScoutRoverLights>();
      }
    }

    public class ScoutRoverLights : Behavior.UnitLights
    {
      protected override MinionLightType GetActiveLightType(MinionLightingConfig minionLightingConfig)
      {
        return MinionLightType.Rover;
      }
    }
  }
}
