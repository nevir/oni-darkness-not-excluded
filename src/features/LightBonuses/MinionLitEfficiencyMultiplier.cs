using System.Collections.Generic;
using HarmonyLib;
using System.Reflection.Emit;

namespace DarknessNotIncluded.LightBonuses
{
  public static class MinionLitEfficiencyMultiplier
  {
    [HarmonyPatch(typeof(Workable))]
    [HarmonyPatch("GetEfficiencyMultiplier")]
    static class Workable_GetEfficiencyMultiplier_Patch
    {
      static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
      {
        var indexLightIntensity = typeof(Grid.LightIntensityIndexer).GetMethod("get_Item");
        var newInstructions = new List<CodeInstruction>(instructions);
        for (var i = 1; i < newInstructions.Count - 1; i++)
        {
          // We're searching for:
          // 
          //  if (Grid.LightIntensity[cell] > 0)
          //
          // IL:
          //
          //   call System.Int32 LightIntensityIndexer::get_Item(System.Int32 i)
          //   ldc.i4.0 NULL
          //   ble.s Label3
          var prevInstruction = newInstructions[i - 1];
          var currInstruction = newInstructions[i];
          var nextInstruction = newInstructions[i + 1];

          if (prevInstruction.Calls(indexLightIntensity)
           && currInstruction.opcode == OpCodes.Ldc_I4_0
           && nextInstruction.opcode == OpCodes.Ble_S)
          {
            // Rewrite to:
            //
            //  if (Grid.LightIntensity[cell] > Workable_GetEfficiencyMultiplier_Patch.GetLitWorkspaceLuxForPatch())
            //
            // IL:
            //
            //   call System.Int32 LightIntensityIndexer::get_Item(System.Int32 i)
            //   call static Workable_GetEfficiencyMultiplier_Patch Workable_GetEfficiencyMultiplier_Patch.GetLitWorkspaceLuxForPatch()
            //   ble.s Label3
            newInstructions[i] = CodeInstruction.Call(typeof(Workable_GetEfficiencyMultiplier_Patch), "GetLitWorkspaceLuxForPatch");
            break;
          }
        }

        return newInstructions;
      }

      static int GetLitWorkspaceLuxForPatch()
      {
        return Config.Instance.litWorkspaceLux - 1;
      }
    }
  }
}
