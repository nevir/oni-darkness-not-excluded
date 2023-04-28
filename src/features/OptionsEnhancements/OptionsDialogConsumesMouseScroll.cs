using HarmonyLib;
using System.Reflection;

namespace DarknessNotIncluded.OptionsEnhancements
{
  static class OptionsDialogConsumesMouseScroll
  {
    [HarmonyPatch]
    static class Patched_OptionsDialog_ShowDialog
    {
      static MethodBase TargetMethod()
      {
        var OptionsDialog = AccessTools.TypeByName("PeterHan.PLib.Options.OptionsDialog");
        return AccessTools.Method(OptionsDialog, "ShowDialog");
      }

      static void Postfix(object __instance)
      {
        var screen = Traverse.Create(__instance).Field("dialog").GetValue<KScreen>();
        if (screen == null) return;
        screen.ConsumeMouseScroll = true;
      }
    }
  }
}
