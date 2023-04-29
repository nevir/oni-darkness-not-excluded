using HarmonyLib;
using UnityEngine;
using System;

namespace DarknessNotIncluded.Exploration
{
  static class MicrobeMusherLighting
  {
    [HarmonyPatch(typeof(MicrobeMusherConfig)), HarmonyPatch("ConfigureBuildingTemplate")]
    static class Patched_MicrobeMusherConfig_ConfigureBuildingTemplate
    {
      static void Postfix(GameObject go)
      {
        var light = go.AddOrGet<Light2D>();
        light.Offset = new Vector2(1.4f, 2.65f);
        light.drawOverlay = true;
        light.overlayColour = TUNING.LIGHT2D.LIGHT_OVERLAY;

        var lightingManager = go.AddOrGet<Behavior.BuildingLightingManager>();
        lightingManager.buildingType = BuildingType.MicrobeMusher;

        // Look at Workable's WorkTimeRemaining to determine status.
      }
    }
  }
}
