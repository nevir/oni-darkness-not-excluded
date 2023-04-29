using HarmonyLib;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

namespace DarknessNotIncluded.Plants
{
  public static class IlluminationVulnerableSupportsMaxLux
  {
    public static string DESCRIPTOR_LABEL = "Less than {Lux}";
    public static string DESCRIPTOR_TOOLTIP = $"This plant prefers darkness; but will tolerate a {UI.PRE_KEYWORD}Light{UI.PST_KEYWORD} source emitting at most {{Lux}}";

    public class MaxLux : KMonoBehaviour
    {
      public int maxLux;
    }

    public static void SetMaxLux(this IlluminationVulnerable instance, int maxLux)
    {
      instance.SetPrefersDarkness(true);
      instance.gameObject.AddOrGet<MaxLux>().maxLux = maxLux;
    }

    public static int GetMaxLux(this IlluminationVulnerable instance)
    {
      return instance.gameObject.FindComponent<MaxLux>()?.maxLux ?? 0;
    }

    [HarmonyPatch(typeof(IlluminationVulnerable)), HarmonyPatch("OnPrefabInit")]
    static class Patched_IlluminationVulnerable_OnPrefabInit
    {
      static void Postfix(IlluminationVulnerable __instance)
      {
        var illuminationVulnerable = __instance.gameObject.AddOrGet<IlluminationVulnerable>();
      }
    }

    [HarmonyPatch(typeof(IlluminationVulnerable)), HarmonyPatch("IsCellSafe")]
    static class Patched_IlluminationVulnerable_IsCellSafe
    {
      static bool Prefix(IlluminationVulnerable __instance, int cell, ref bool __result)
      {
        var maxLux = __instance.GetMaxLux();
        if (maxLux <= 0) return true;

        if (!Grid.IsValidCell(cell))
        {
          __result = false;
          return false;
        }

        __result = Grid.LightIntensity[cell] <= maxLux;
        return false;
      }
    }

    [HarmonyPatch(typeof(IlluminationVulnerable)), HarmonyPatch("GetDescriptors")]
    static class Patched_IlluminationVulnerable_GetDescriptors
    {
      static bool Prefix(IlluminationVulnerable __instance, ref List<Descriptor> __result)
      {
        var maxLux = __instance.GetMaxLux();
        if (maxLux <= 0) return true;

        var formattedLux = GameUtil.GetFormattedLux(maxLux);
        __result = new List<Descriptor>()
        {
          new Descriptor(DESCRIPTOR_LABEL.Replace("{Lux}", formattedLux), DESCRIPTOR_TOOLTIP.Replace("{Lux}", formattedLux), Descriptor.DescriptorType.Requirement)
        };
        return false;
      }
    }

    [HarmonyPatch(typeof(MinionVitalsPanel)), HarmonyPatch("GetIlluminationLabel")]
    static class Patched_MinionVitalsPanel_GetIlluminationLabel
    {
      static bool Prefix(GameObject go, ref string __result)
      {
        var illuminationVulnerable = go.GetComponent<IlluminationVulnerable>();
        if (illuminationVulnerable == null) return true;
        var maxLux = illuminationVulnerable.GetMaxLux();
        if (maxLux <= 0) return true;

        __result = Db.Get().Amounts.Illumination.Name + "\n    • " + DESCRIPTOR_LABEL.Replace("{Lux}", GameUtil.GetFormattedLux(maxLux));
        return false;
      }
    }

    [HarmonyPatch(typeof(MinionVitalsPanel)), HarmonyPatch("GetIlluminationTooltip")]
    static class Patched_MinionVitalsPanel_GetIlluminationTooltip
    {
      static bool Prefix(GameObject go, ref string __result)
      {
        var illuminationVulnerable = go.GetComponent<IlluminationVulnerable>();
        if (illuminationVulnerable == null) return true;
        var maxLux = illuminationVulnerable.GetMaxLux();
        if (maxLux <= 0) return true;

        __result = DESCRIPTOR_TOOLTIP.Replace("{Lux}", GameUtil.GetFormattedLux(maxLux));
        return false;
      }
    }
  }

}
