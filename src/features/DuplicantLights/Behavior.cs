using DarknessNotIncluded.Exploration;
using HarmonyLib;
using System;
using UnityEngine;

namespace DarknessNotIncluded.DuplicantLights
{
  public static class Behavior
  {
    private static bool disableLightsInBedrooms;
    private static bool disableLightsInLitAreas;
    private static MinionLightingConfig minionLightingConfig;

    private static Config.Observer configObserver = new Config.Observer((config) =>
    {
      disableLightsInBedrooms = config.disableDupeLightsInBedrooms;
      disableLightsInLitAreas = config.disableDupeLightsInLitAreas;
      minionLightingConfig = config.minionLightingConfig;
    });

    public abstract class UnitLights : KMonoBehaviour, ISim33ms
    {
      [MyCmpGet]
      private GridVisibility gridVisibility;

      public Light2D Light { get; set; }

      private MinionLightType currentLightType = MinionLightType.None;

      protected override void OnPrefabInit()
      {
        base.OnPrefabInit();

        Light = gameObject.AddComponent<Light2D>();
      }

      protected override void OnSpawn()
      {
        base.OnSpawn();
        if (gameObject == null) return;

        UpdateLights();
      }

      public void Sim33ms(float dt)
      {
        UpdateLights();
      }

      private void UpdateLights()
      {
        var lightType = GetActiveLightType();
        var lightConfig = minionLightingConfig.Get(lightType);

        // Update grid visibility based on the minion's internal state
        gridVisibility.SetRadius(lightConfig.reveal);

        // But actual lights may change based on behavior:
        if (disableLightsInBedrooms && lightType != MinionLightType.None)
        {
          if (MinionRoomState.SleepersInSameRoom(gameObject))
          {
            lightType = MinionLightType.None;
          }
        }

        if (disableLightsInLitAreas && lightType != MinionLightType.None)
        {
          var headCell = Grid.CellAbove(Grid.PosToCell(gameObject));
          var headLux = Grid.IsValidCell(headCell) ? Grid.LightIntensity[headCell] : 0;

          var dupeLux = Light.enabled ? Light.Lux : 0;
          var baseCellLux = Math.Max(0, headLux - dupeLux);
          var targetLux = lightConfig.lux;
          // Keep intrinsic lights on even if next to another dupe
          if (lightType == MinionLightType.Intrinsic) targetLux *= 2;

          if (baseCellLux >= targetLux)
          {
            lightType = MinionLightType.None;
          }
        }

        SetLightType(lightType);
      }

      private void SetLightType(MinionLightType lightType)
      {
        if (lightType == currentLightType) return;
        currentLightType = lightType;
        minionLightingConfig.Get(lightType).ConfigureLight(Light);
      }

      protected abstract MinionLightType GetActiveLightType();
    }
  }
}
