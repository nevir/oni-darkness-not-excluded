using HarmonyLib;
using UnityEngine;

namespace DarknessNotIncluded.Exploration
{
  static class MicrobeMusherLighting
  {
    [HarmonyPatch(typeof(MicrobeMusherConfig)), HarmonyPatch("ConfigureBuildingTemplate")]
    static class Patched_MicrobeMusherConfig_ConfigureBuildingTemplate
    {
      static void Postfix(GameObject go)
      {
        var light = go.AddOrGet<Light2D>();
        light.Offset = new Vector2(1.4f, 2.65f);
        light.drawOverlay = true;
        light.overlayColour = TUNING.LIGHT2D.LIGHT_OVERLAY;

        var lightingManager = go.AddOrGet<Behavior.BuildingLightingManager>();
        lightingManager.buildingType = BuildingType.MicrobeMusher;

        go.AddOrGet<LightWhileWorking>();
      }
    }

    public class LightWhileWorking : KMonoBehaviour, ISim33ms
    {
      [MyCmpGet]
      MicrobeMusher musher;

      [MyCmpGet]
      Behavior.BuildingLightingManager lightingManager;

      public void Sim33ms(float dt)
      {
        lightingManager.SetForceOff(!musher.HasWorker);
      }
    }
  }
}
