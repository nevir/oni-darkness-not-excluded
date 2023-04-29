using HarmonyLib;
using UnityEngine;
using System;

namespace DarknessNotIncluded.Exploration
{
  public static class Behavior
  {
    public class BuildingLightingManager : KMonoBehaviour
    {
      public BuildingType buildingType;

      private Light2D light;
      private GridVisibility gridVisibility;

      protected override void OnPrefabInit()
      {
        base.OnPrefabInit();

        Console.WriteLine($"BuildingLightingManager [{gameObject}] OnPrefabInit");

        light = gameObject.AddOrGet<Light2D>();
        gridVisibility = gameObject.AddOrGet<GridVisibility>();

        Config.ObserveFor(this, (config) =>
        {
          Console.WriteLine($"BuildingLightingManager [{gameObject}] Config update");
          var buildingConfig = config.buildingLightingConfig[buildingType];

          buildingConfig.ConfigureLight(light);

          // TODO: offset to center of building and increase radius by building 
          // size.
          gridVisibility.SetRadius(buildingConfig.reveal);
          if (gridVisibility.isSpawned)
          {
            Traverse.Create(gridVisibility).Method("OnCellChange").GetValue();
          }
        });
      }
    }
  }
}
