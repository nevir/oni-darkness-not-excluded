using HarmonyLib;

namespace DarknessNotIncluded.Exploration
{
  public static class Behavior
  {
    public class BuildingLightingManager : KMonoBehaviour
    {
      public BuildingType buildingType;

      private bool forceOff = false;

      private Light2D light;
      private GridVisibility gridVisibility;
      private LightConfig buildingConfig;

      protected override void OnPrefabInit()
      {
        base.OnPrefabInit();

        light = gameObject.AddOrGet<Light2D>();
        gridVisibility = gameObject.AddOrGet<GridVisibility>();

        Config.ObserveFor(this, (config) =>
        {
          buildingConfig = config.buildingLightingConfig[buildingType];

          buildingConfig.ConfigureLight(light, forceOff);

          // TODO: offset to center of building and increase radius by building 
          // size.
          gridVisibility.SetRadius(buildingConfig.reveal);
          if (gridVisibility.isSpawned)
          {
            Traverse.Create(gridVisibility).Method("OnCellChange").GetValue();
          }
        });
      }

      public void SetForceOff(bool forceOff)
      {
        this.forceOff = forceOff;
        var enabled = forceOff ? false : buildingConfig.enabled;
        if (light.enabled != enabled)
        {
          light.enabled = enabled;
          light.FullRefresh();
        }
      }
    }
  }
}
