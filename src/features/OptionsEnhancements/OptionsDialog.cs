using HarmonyLib;
using System;

namespace DarknessNotIncluded.OptionsEnhancements
{
  static class OptionsDialog
  {
    public static void Show<TConfigType>(Type ConfigType, Action<TConfigType> onClose)
    {
      var OptionsDialog = AccessTools.TypeByName("PeterHan.PLib.Options.OptionsDialog");
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
