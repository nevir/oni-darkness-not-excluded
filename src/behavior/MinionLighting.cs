using HarmonyLib;
using System;
using UnityEngine;

namespace DarknessNotIncluded
{
  static class MinionLighting
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

      public Light2D HeadLight { get; set; }
      public Light2D BodyLight { get; set; }

      private MinionLightType currentLightType = MinionLightType.None;

      protected override void OnPrefabInit()
      {
        base.OnPrefabInit();

        HeadLight = minion.gameObject.AddComponent<Light2D>();
        HeadLight.shape = LightShape.Circle;
        HeadLight.Offset = new Vector2(0f, 1.0f);

        BodyLight = minion.gameObject.AddComponent<Light2D>();
        BodyLight.shape = LightShape.Circle;
        BodyLight.Offset = new Vector2(0f, 0.0f);
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
          var dupeLux = (HeadLight.enabled ? HeadLight.Lux : 0) + (BodyLight.enabled ? BodyLight.Lux : 0);
          var baseCellLux = Math.Max(0, Grid.LightIntensity[headCell] - dupeLux);
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

        HeadLight.enabled = lightConfig.enabled;
        HeadLight.Lux = lightConfig.lux / 2;
        HeadLight.Range = lightConfig.range;
        HeadLight.Color = lightConfig.color;
        HeadLight.FullRefresh();

        BodyLight.enabled = lightConfig.enabled;
        BodyLight.Lux = lightConfig.lux / 2;
        BodyLight.Range = lightConfig.range;
        BodyLight.Color = lightConfig.color;
        BodyLight.FullRefresh();
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

        var resume = minion.GetComponent<MinionResume>();
        var hat = resume.CurrentHat;
        var suit = minion.GetEquipment().GetSlot(Db.Get().AssignableSlots.Suit)?.assignable as Equippable;
        var suitPrefab = suit?.GetComponent<KPrefabID>();
        Console.WriteLine($"suit: {suit} suitPrefab: {suitPrefab}");

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
