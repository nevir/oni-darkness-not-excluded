using HarmonyLib;
using UnityEngine;

namespace DarknessNotIncluded.Exploration
{
  static class HeadquartersLighting
  {
    [HarmonyPatch(typeof(HeadquartersConfig)), HarmonyPatch("ConfigureBuildingTemplate")]
    static class Patched_HeadquartersConfig_ConfigureBuildingTemplate
    {
      static void Postfix(GameObject go)
      {
        var lightingManager = go.AddComponent<Behavior.BuildingLightingManager>();
        lightingManager.buildingType = BuildingType.PrintingPod;
      }
    }

    // It's a lot less brittle to clean up after the spawner than try to 
    // modify its behavior.
    [HarmonyPatch(typeof(WorldGenSpawner)), HarmonyPatch("OnSpawn")]
    static class Patched_WorldGenSpawner_OnSpawn
    {
      static bool isGeneratingWorld = false;

      static void Prefix(bool ___hasPlacedTemplates)
      {
        isGeneratingWorld = !___hasPlacedTemplates;
      }

      static void Postfix()
      {
        if (!isGeneratingWorld) return;
        isGeneratingWorld = false;

        for (int i = 0; i < Grid.CellCount; ++i)
        {
          Grid.Visible[i] = 0;
        }
      }
    }
  }
}
