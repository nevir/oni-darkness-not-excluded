using HarmonyLib;
using PeterHan.PLib.Core;
using System;

namespace DarknessNotIncluded.Exploration
{
  static class DisableDefaultFogOfWarReveal
  {
    static int worldGenCell = Grid.InvalidCell;

    [HarmonyPatch(typeof(GridVisibility)), HarmonyPatch("OnCellChange")]
    static class Patched_GridVisibility_OnCellChange
    {
      static bool Prefix()
      {
        return false;
      }
    }

    // It's a lot less brittle to clean up after the spawner than try to 
    // modify its behavior.
    [HarmonyPatch(typeof(WorldGenSpawner)), HarmonyPatch("OnSpawn")]
    static class Patched_WorldGenSpawner_OnSpawn
    {
      // static void Prefix(WorldGenSpawner __instance)
      // {
      //   bool hasPlacedTemplates = true; // Assume yes.
      //   PPatchTools.TryGetFieldValue(__instance, "hasPlacedTemplates", out hasPlacedTemplates);

      //   if (!hasPlacedTemplates)
      //   {
      //     var clusterLayout = SaveLoader.Instance.ClusterLayout;
      //     var spawnPos = clusterLayout.currentWorld.SpawnData.baseStartPos + clusterLayout.currentWorld.WorldOffset;
      //     worldGenCell = Grid.PosToCell(spawnPos);

      //     __instance.gameObject.AddOrGet<WorldGenInitialReveal>();
      //   }
      // }

      static void Postfix()
      {
        Console.WriteLine("WorldGenSpawner OnSpawn");
        // if (worldGenCell == Grid.InvalidCell) return;

        for (int i = 0; i < Grid.CellCount; ++i)
        {
          Grid.Visible[i] = 0;
        }

        // var clusterLayout = SaveLoader.Instance.ClusterLayout;
        // var spawnPos = clusterLayout.currentWorld.SpawnData.baseStartPos + clusterLayout.currentWorld.WorldOffset;
        // GameUtil.FloodCollectCells(Grid.PosToCell(spawnPos), (cell) =>
        // {
        //   var element = Grid.Element[cell];
        //   Console.WriteLine($"spawn test cell: {cell} lux: {Grid.LightIntensity[cell]} element: {element}");
        //   return false;
        // });
      }
    }

    // public class WorldGenInitialReveal : KMonoBehaviour, ISim33ms
    // {
    //   public void Sim33ms(float dt)
    //   {
    //     var lightIntensity = Grid.LightIntensity;
    //     if (worldGenCell == Grid.InvalidCell) return;
    //     if (lightIntensity[worldGenCell] <= 0) return;

    //     GameUtil.FloodCollectCells(worldGenCell, (cell) =>
    //     {
    //       var isLit = lightIntensity[cell] > 0;
    //       if (isLit) Grid.Visible[cell] = 255;

    //       return isLit;
    //     });

    //     // And we're done with the initial spawn.
    //     worldGenCell = Grid.InvalidCell;
    //   }
    // }

    // [HarmonyPatch(typeof(WorldGenSpawner)), HarmonyPatch("OnSpawn")]
    // static class Patched_WorldGenSpawner_OnSpawn
    // {
    //   static bool Prefix(Cluster clusterLayout)
    //   {
    //     foreach (WorldGen world in clusterLayout.worlds)
    //     {
    //       Game.Instance.Reset(world.SpawnData, world.WorldOffset);
    //     }

    //     for (int i = 0; i < Grid.CellCount; ++i)
    //     {
    //       Grid.Revealed[i] = false;
    //       Grid.Spawnable[i] = 0;
    //     }

    //     var spawnPos = clusterLayout.currentWorld.SpawnData.baseStartPos + clusterLayout.currentWorld.WorldOffset;
    //     Vector2I vector2I = clusterLayout.currentWorld.SpawnData.baseStartPos + clusterLayout.currentWorld.WorldOffset;
    //     GridVisibility.Reveal(vector2I.x, vector2I.y, 5, 3.5f);

    //     var spawnCell = Grid.PosToCell(spawnPos);

    //     GameUtil.FloodCollectCells(spawnCell, (cell) =>
    //     {
    //       var element = Grid.Element[cell];
    //       Console.WriteLine($"spawn test cell: {cell} lux: {Grid.LightIntensity[cell]} element: {element}");
    //       return false;
    //     });

    //     return false;
    //   }
    // }
  }
}
