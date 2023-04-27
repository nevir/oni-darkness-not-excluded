using HarmonyLib;
using System;
using UnityEngine;

namespace DarknessNotIncluded.DuplicantLights
{
  public static class MinionLighting
  {
    [HarmonyPatch(typeof(MinionConfig)), HarmonyPatch("CreatePrefab")]
    static class Patched_MinionConfig_CreatePrefab
    {
      static void Postfix(GameObject __result)
      {
        __result.AddOrGet<MinionLights>();
      }
    }

    public class MinionLights : KMonoBehaviour, ISim33ms
    {
      [MyCmpGet]
      private MinionIdentity minion;

      [MyCmpGet]
      private MinionResume resume;

      [MyCmpGet]
      private GridVisibility gridVisibility;

      public Light2D Light { get; set; }

      private MinionLightType currentLightType = MinionLightType.None;

      protected override void OnPrefabInit()
      {
        base.OnPrefabInit();

        Light = minion.gameObject.AddComponent<Light2D>();
      }

      protected override void OnSpawn()
      {
        base.OnSpawn();
        if (minion.gameObject == null) return;

        UpdateLights();
      }

      public void Sim33ms(float dt)
      {
        UpdateLights();
      }

      private void UpdateLights()
      {
        var config = Config.Instance;
        var lightType = GetActiveLightType();

        // Update grid visibility based on the minion's internal state
        var reveal = lightType.Config().reveal;
        gridVisibility.radius = reveal;

        // But actual lights may change based on behavior:
        if (config.disableDupeLightsInBedrooms && lightType != MinionLightType.None)
        {
          if (MinionRoomState.SleepersInSameRoom(minion))
          {
            lightType = MinionLightType.None;
          }
        }

        if (config.disableDupeLightsInLitAreas && lightType != MinionLightType.None)
        {
          var headCell = Grid.CellAbove(Grid.PosToCell(minion.gameObject));
          var headLux = Grid.IsValidCell(headCell) ? Grid.LightIntensity[headCell] : 0;

          var dupeLux = Light.enabled ? Light.Lux : 0;
          var baseCellLux = Math.Max(0, headLux - dupeLux);
          var targetLux = lightType.Config().lux;
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

        var lightConfig = lightType.Config();

        Light.enabled = lightConfig.enabled;
        Light.shape = lightConfig.shape.LightShape();
        Light.Lux = lightConfig.lux;
        Light.Range = lightConfig.range;
        Light.Color = lightConfig.color;
        Light.FullRefresh();
      }

      private MinionLightType GetActiveLightType()
      {
        var lightType = GetLightTypeForCurrentState();
        if (lightType == MinionLightType.None) return lightType;

        if (!lightType.Config().enabled) lightType = MinionLightType.Intrinsic;
        if (!lightType.Config().enabled) lightType = MinionLightType.None;

        return lightType;
      }

      private MinionLightType GetLightTypeForCurrentState()
      {
        if (minion.IsSleeping()) return MinionLightType.None;

        var hat = resume.CurrentHat;
        var suit = minion.GetEquipment().GetSlot(Db.Get().AssignableSlots.Suit)?.assignable as Equippable;
        var suitPrefab = suit?.GetComponent<KPrefabID>();

        if (suitPrefab != null && suit?.isEquipped == true)
        {
          if (suitPrefab?.HasTag(GameTags.AtmoSuit) == true) return MinionLightType.AtmoSuit;
          else if (suitPrefab?.HasTag(GameTags.JetSuit) == true) return MinionLightType.JetSuit;
          else if (suitPrefab?.HasTag(GameTags.LeadSuit) == true) return MinionLightType.LeadSuit;
          else return MinionLightType.Intrinsic;
        }
        else if (hat?.StartsWith("hat_role_mining") == true)
        {
          if (hat == "hat_role_mining1") return MinionLightType.Mining1;
          else if (hat == "hat_role_mining2") return MinionLightType.Mining2;
          else if (hat == "hat_role_mining3") return MinionLightType.Mining3;
          else if (hat == "hat_role_mining4") return MinionLightType.Mining4;
          else return MinionLightType.Mining4;
        }
        else if (hat?.StartsWith("hat_role_research") == true)
        {
          return MinionLightType.Science;
        }
        else if (hat?.StartsWith("hat_role_astronaut") == true)
        {
          return MinionLightType.Rocketry;
        }
        else
        {
          return MinionLightType.Intrinsic;
        }
      }
    }
  }
}
