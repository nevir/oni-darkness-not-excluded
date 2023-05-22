using HarmonyLib;
using System;
using System.Reflection;

namespace DarknessNotIncluded.OptionsEnhancements
{
  static class OptionsDialog
  {
    public static void Show<TConfigType>(Type ConfigType, Action<TConfigType> onClose)
    {
      // Note that it is important that we reference the PLib instance that we
      // have bundled—options are managed by that (vs the shared instance).
      var OptionsDialog = Assembly.GetExecutingAssembly().GetType("PeterHan.PLib.Options.OptionsDialog");
      var dialog = AccessTools.Constructor(OptionsDialog, new Type[] { typeof(Type) }).Invoke(new object[] { ConfigType });
      var dialogTraverse = Traverse.Create(dialog);

      Action<object> closeHandler = (config) =>
      {
        onClose((TConfigType)config);
      };
      dialogTraverse.Property("OnClose").SetValue(closeHandler);

      dialogTraverse.Method("ShowDialog").GetValue();
    }
  }
}
