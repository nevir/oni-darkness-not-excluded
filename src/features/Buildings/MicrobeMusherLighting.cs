using HarmonyLib;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace DarknessNotIncluded.Exploration
{
  static class MicrobeMusherLighting
  {
    [HarmonyPatch(typeof(MicrobeMusherConfig)), HarmonyPatch("ConfigureBuildingTemplate")]
    static class Patched_MicrobeMusherConfig_ConfigureBuildingTemplate
    {
      static void Postfix(GameObject go)
      {
        Console.WriteLine($"Patched_MicrobeMusherConfig_ConfigureBuildingTemplate: {go}");

        var light = go.AddOrGet<Light2D>();
        light.Offset = new Vector2(1.4f, 2.65f);
        light.drawOverlay = true;
        light.overlayColour = TUNING.LIGHT2D.LIGHT_OVERLAY;

        var lightingManager = go.AddOrGet<Behavior.BuildingAnimatedLightingManager>();
        lightingManager.buildingType = BuildingType.MicrobeMusher;

        Behavior.BuildingAnimatedLightingManager.RegisterFrameOffsets(BuildingType.MicrobeMusher, new Dictionary<string, Dictionary<int, Vector2>>() {
          {
            "working_loop", new Dictionary<int, Vector2>() {
              {  0, new Vector2(1.400f, 2.650f) },
              {  1, new Vector2(1.385f, 2.650f) },
              {  2, new Vector2(1.375f, 2.650f) },
              {  3, new Vector2(1.365f, 2.650f) },
              {  4, new Vector2(1.355f, 2.650f) },
              {  5, new Vector2(1.358f, 2.650f) },
              {  6, new Vector2(1.360f, 2.647f) },
              {  7, new Vector2(1.365f, 2.643f) },
              {  8, new Vector2(1.370f, 2.638f) },
              {  9, new Vector2(1.374f, 2.638f) },
              { 10, new Vector2(1.378f, 2.639f) },
              { 11, new Vector2(1.383f, 2.640f) },
              { 12, new Vector2(1.380f, 2.644f) },
              { 13, new Vector2(1.378f, 2.648f) },
              { 14, new Vector2(1.375f, 2.648f) },
              { 15, new Vector2(1.371f, 2.654f) },
              { 16, new Vector2(1.368f, 2.655f) },
              { 17, new Vector2(1.364f, 2.658f) },
              { 18, new Vector2(1.360f, 2.659f) },
              { 19, new Vector2(1.355f, 2.659f) },
              { 20, new Vector2(1.349f, 2.659f) },
              { 21, new Vector2(1.345f, 2.659f) },
              { 22, new Vector2(1.342f, 2.658f) },
              { 23, new Vector2(1.350f, 2.661f) },
              { 24, new Vector2(1.372f, 2.666f) },
              { 25, new Vector2(1.392f, 2.675f) },
              { 26, new Vector2(1.406f, 2.675f) },
              { 27, new Vector2(1.405f, 2.671f) },
              { 28, new Vector2(1.405f, 2.660f) },
              { 29, new Vector2(1.403f, 2.656f) },
            }
          }
        });
      }
    }
  }
}
