using HarmonyLib;
using PeterHan.PLib.Core;
using System;

namespace DarknessNotIncluded.Exploration
{
  /// <summary>
  /// Rather than revealing the map by default, we enhance the telepad to 
  /// have a built-in fog of war revealer (GridVisibility).
  ///
  /// This allows us to more easily control the FOW reveal for new maps.
  /// </summary>
  static class TelepadRevealsFogOfWar
  {
    static bool isGeneratingWorld = false;

    [HarmonyPatch(typeof(Telepad)), HarmonyPatch("OnSpawn")]
    static class Patched_Telepad_OnSpawn
    {
      static void Postfix(Telepad __instance)
      {
        var gridVisibility = __instance.gameObject.AddOrGet<GridVisibility>();
        // TODO: Configurable
        gridVisibility.radius = 1;
        gridVisibility.innerRadius = 1;
      }
    }

    // It's a lot less brittle to clean up after the spawner than try to 
    // modify its behavior.
    [HarmonyPatch(typeof(WorldGenSpawner)), HarmonyPatch("OnSpawn")]
    static class Patched_WorldGenSpawner_OnSpawn
    {
      static void Prefix(WorldGenSpawner __instance)
      {
        bool hasPlacedTemplates;
        if (!PPatchTools.TryGetFieldValue(__instance, "hasPlacedTemplates", out hasPlacedTemplates))
        {
          Console.WriteLine("Expected WorldGenSpawner to have a hasPlacedTemplates field, but found none!");
          return;
        }
        isGeneratingWorld = !hasPlacedTemplates;
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
