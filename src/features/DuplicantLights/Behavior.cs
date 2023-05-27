using DarknessNotIncluded.Exploration;
using System;

namespace DarknessNotIncluded.DuplicantLights
{
  public static class Behavior
  {
    public abstract class UnitLights : KMonoBehaviour, ISim33ms
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

      [MyCmpGet]
      private GridVisibility gridVisibility;

      public Light2D Light { get; set; }

      private MinionLightType currentLightType = MinionLightType.None;

      protected override void OnPrefabInit()
      {
        base.OnPrefabInit();

        Light = gameObject.AddComponent<Light2D>();

        Config.ObserveFor(this, (config) =>
        {
          UpdateLights(true);
        });
      }

      protected override void OnSpawn()
      {
        base.OnSpawn();
        UpdateLights();
      }

      public void Sim33ms(float dt)
      {
        UpdateLights();
      }

      private void UpdateLights(bool force = false)
      {
        if (gameObject == null) return;

        var lightType = GetActiveLightType(minionLightingConfig);
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

        SetLightType(lightType, force);
      }

      private void SetLightType(MinionLightType lightType, bool force)
      {
        if (lightType == currentLightType && !force) return;
        currentLightType = lightType;
        minionLightingConfig.Get(lightType).ConfigureLight(Light);
      }

      protected abstract MinionLightType GetActiveLightType(MinionLightingConfig minionLightingConfig);
    }
  }
}
