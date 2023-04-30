using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace DarknessNotIncluded.Exploration
{
  public static class Behavior
  {
    public class BuildingLightingManager : KMonoBehaviour
    {
      public BuildingType buildingType;

      protected LightConfig lightConfig;
      protected Light2D light;
      protected GridVisibility gridVisibility;

      protected override void OnPrefabInit()
      {
        base.OnPrefabInit();

        light = gameObject.AddOrGet<Light2D>();
        gridVisibility = gameObject.AddOrGet<GridVisibility>();

        Config.ObserveFor(this, (config) =>
        {
          lightConfig = config.buildingLightingConfig[buildingType];

          lightConfig.ConfigureLight(light);

          // TODO: offset to center of building and increase radius by building 
          // size.
          gridVisibility.SetRadius(lightConfig.reveal);
          if (gridVisibility.isSpawned)
          {
            Traverse.Create(gridVisibility).Method("OnCellChange").GetValue();
          }
        });
      }
    }

    public class BuildingAnimatedLightingManager : BuildingLightingManager
    {
      private static Dictionary<BuildingType, Dictionary<string, Dictionary<int, Vector2>>> FRAME_OFFSETS = new Dictionary<BuildingType, Dictionary<string, Dictionary<int, Vector2>>>();

      public static void RegisterFrameOffsets(BuildingType buildingType, Dictionary<string, Dictionary<int, Vector2>> frameOffsets)
      {
        FRAME_OFFSETS[buildingType] = frameOffsets;
      }

      private string currentAnimName;
      private int currentFrame;

      [MyCmpGet]
      KBatchedAnimController animController;

      public void Update()
      {
        if (!FRAME_OFFSETS.ContainsKey(buildingType) || animController == null || animController.CurrentAnim == null) return;
        if (animController.CurrentAnim.name == currentAnimName && animController.currentFrame == currentFrame) return;
        currentAnimName = animController.CurrentAnim.name;
        currentFrame = animController.currentFrame;

        var frameOffsets = FRAME_OFFSETS[buildingType];
        if (frameOffsets.ContainsKey(currentAnimName) && frameOffsets[currentAnimName].ContainsKey(currentFrame))
        {
          light.enabled = lightConfig.enabled;
          light.Offset = frameOffsets[currentAnimName][currentFrame];
        }
        else
        {
          light.enabled = false;
        }
      }
    }
  }
}
