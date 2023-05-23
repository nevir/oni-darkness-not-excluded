using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;

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

    public class MinionLights : Behavior.UnitLights
    {
      [MyCmpGet]
      private MinionIdentity minion;

      [MyCmpGet]
      private MinionResume resume;

      protected override MinionLightType GetActiveLightType(MinionLightingConfig minionLightingConfig)
      {
        var lightType = GetLightTypeForCurrentState(minionLightingConfig);
        if (lightType == MinionLightType.None) return lightType;

        if (!minionLightingConfig.Get(lightType).enabled) lightType = MinionLightType.Intrinsic;
        if (!minionLightingConfig.Get(lightType).enabled) lightType = MinionLightType.None;

        return lightType;
      }

      private MinionLightType GetLightTypeForCurrentState(MinionLightingConfig minionLightingConfig)
      {
        if (minion == null) return MinionLightType.None;
        if (!minion.isSpawned) return MinionLightType.None;
        if (minion.IsSleeping()) return MinionLightType.None;

        var hat = resume.CurrentHat;
        var suit = minion.GetEquipment().GetSlot(Db.Get().AssignableSlots.Suit)?.assignable as Equippable;
        var suitPrefab = suit?.GetComponent<KPrefabID>();

        var possibleLightTypes = new List<MinionLightType>();

        if (suitPrefab != null && suit?.isEquipped == true)
        {
          if (suitPrefab?.HasTag(GameTags.AtmoSuit) == true) possibleLightTypes.Add(MinionLightType.AtmoSuit);
          if (suitPrefab?.HasTag(GameTags.JetSuit) == true) possibleLightTypes.Add(MinionLightType.JetSuit);
          if (suitPrefab?.HasTag(GameTags.LeadSuit) == true) possibleLightTypes.Add(MinionLightType.LeadSuit);
        }

        if (hat?.StartsWith("hat_role_mining") == true)
        {
          if (hat == "hat_role_mining1") possibleLightTypes.Add(MinionLightType.Mining1);
          else if (hat == "hat_role_mining2") possibleLightTypes.Add(MinionLightType.Mining2);
          else if (hat == "hat_role_mining3") possibleLightTypes.Add(MinionLightType.Mining3);
          else if (hat == "hat_role_mining4") possibleLightTypes.Add(MinionLightType.Mining4);
          else possibleLightTypes.Add(MinionLightType.Mining4);
        }
        else if (hat?.StartsWith("hat_role_research") == true)
        {
          possibleLightTypes.Add(MinionLightType.Science);
        }
        else if (hat?.StartsWith("hat_role_astronaut") == true)
        {
          possibleLightTypes.Add(MinionLightType.Rocketry);
        }

        possibleLightTypes = possibleLightTypes.FindAll(type => minionLightingConfig.Get(type).enabled);
        possibleLightTypes.Sort((a, b) => minionLightingConfig.Get(b).lux - minionLightingConfig.Get(a).lux);

        return possibleLightTypes.Count > 0 ? possibleLightTypes[0] : MinionLightType.Intrinsic;
      }
    }
  }
}
