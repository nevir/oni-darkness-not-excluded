using HarmonyLib;
using UnityEngine;

namespace DarknessNotIncluded.DuplicantLights
{
  public static class MinionLighting
  {
    private static MinionLightingConfig minionLightingConfig;

    private static Config.Observer configObserver = new Config.Observer((config) =>
    {
      minionLightingConfig = config.minionLightingConfig;
    });

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

      protected override MinionLightType GetActiveLightType()
      {
        var lightType = GetLightTypeForCurrentState();
        if (lightType == MinionLightType.None) return lightType;

        if (!minionLightingConfig.Get(lightType).enabled) lightType = MinionLightType.Intrinsic;
        if (!minionLightingConfig.Get(lightType).enabled) lightType = MinionLightType.None;

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
