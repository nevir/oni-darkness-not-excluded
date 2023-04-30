using HarmonyLib;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace DarknessNotIncluded.Exploration
{
  static class HydrogenGeneratorLighting
  {
    [HarmonyPatch(typeof(HydrogenGeneratorConfig)), HarmonyPatch("DoPostConfigureComplete")]
    static class Patched_HydrogenGeneratorConfig_DoPostConfigureComplete
    {
      static void Postfix(GameObject go)
      {
        var light = go.AddOrGet<Light2D>();
        light.drawOverlay = true;
        light.overlayColour = TUNING.LIGHT2D.LIGHT_OVERLAY;

        var lightingManager = go.AddOrGet<Behavior.BuildingAnimatedLightingManager>();
        lightingManager.buildingType = BuildingType.HydrogenGenerator;

        Behavior.BuildingAnimatedLightingManager.RegisterFrameOffsets(BuildingType.HydrogenGenerator, new Dictionary<string, Dictionary<int, Vector2>>() {
          {
            "working_loop", new Dictionary<int, Vector2>() {
              { 34, new Vector2(1.450f, 2.650f) },
              { 35, new Vector2(1.450f, 2.650f) },
              { 36, new Vector2(1.450f, 2.650f) },
              { 37, new Vector2(1.450f, 2.550f) },
              { 38, new Vector2(1.450f, 2.450f) },
              { 39, new Vector2(1.450f, 2.425f) },
              { 40, new Vector2(1.450f, 2.400f) },
              { 41, new Vector2(1.450f, 2.350f) },
              { 42, new Vector2(1.450f, 2.300f) },
              { 43, new Vector2(1.450f, 2.250f) },
              { 44, new Vector2(1.450f, 2.125f) },
              { 45, new Vector2(1.450f, 2.050f) },
              { 46, new Vector2(1.450f, 2.000f) },
              { 47, new Vector2(1.450f, 1.950f) },
              { 48, new Vector2(1.450f, 1.850f) },
              { 49, new Vector2(1.450f, 1.750f) },
              { 50, new Vector2(1.450f, 1.700f) },
              { 51, new Vector2(1.450f, 1.650f) },
              { 52, new Vector2(1.450f, 1.600f) },
              { 53, new Vector2(1.450f, 1.550f) },
              { 54, new Vector2(1.450f, 1.558f) },
              { 55, new Vector2(1.450f, 1.555f) },
              { 56, new Vector2(1.450f, 1.500f) },
              { 57, new Vector2(1.450f, 1.450f) },
              { 58, new Vector2(1.450f, 1.450f) },
              { 59, new Vector2(1.450f, 1.450f) },
            }
          }
        });
      }
    }
  }
}
