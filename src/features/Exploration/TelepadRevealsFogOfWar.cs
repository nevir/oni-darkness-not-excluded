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

    [HarmonyPatch(typeof(Telepad)), HarmonyPatch("OnPrefabInit")]
    static class Patched_Telepad_OnPrefabInit
    {
      static void Postfix(Telepad __instance)
      {
        var config = Config.Instance;
        var gridVisibility = __instance.gameObject.AddOrGet<GridVisibility>();

        // +1 because the telepad is not 1 cell wide.
        var radius = Math.Max(config.telepadRevealRadius, 0) + 1;
        gridVisibility.radius = radius;
        gridVisibility.innerRadius = radius;
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
