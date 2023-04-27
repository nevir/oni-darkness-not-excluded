using HarmonyLib;
using UnityEngine;
using PeterHan.PLib.UI;
using System.Reflection;
using System;

namespace DarknessNotIncluded.InGameConfig
{
  static class DarknessToggleButton
  {
    static LocString DARKNESS_ACTIVE_TOOLTIP = "<b>Disable Darkness</b>\n\nGetting to be a little too much? It's ok, we won't judge you …too much.";
    static LocString DARKNESS_DISABLED_TOOLTIP = "<b>Re-Enable Darkness</b>\n\nAh! Welcome back to the challenge.";

    [HarmonyPatch(typeof(TopLeftControlScreen)), HarmonyPatch("OnActivate")]
    static class Patched_TopLeftControlScreen_OnActivate
    {
      static void Postfix(RectTransform ___secondaryRow, MultiToggle ___kleiItemDropButton)
      {
        var button = new PButton("DarknessToggle")
        {
          ToolTip = DARKNESS_ACTIVE_TOOLTIP,
          Sprite = Assets.GetSprite("icon_category_lights_disabled"),
          SpriteSize = new Vector2(24, 24),
          Margin = new RectOffset(3, 3, 3, 3),
          Color = PUITuning.Colors.ButtonBlueStyle,
        }
        .AddOnRealize((realized) =>
        {
          var kButton = realized.GetComponent<KButton>();
          kButton.onBtnClick += (btn) =>
          {
            if (btn == KKeyCode.Mouse0)
            {
              ToggleDarkness(kButton);
            }
            else if (btn == KKeyCode.Mouse1)
            {
              ShowOptions();
            }
          };
        });
        button.AddTo(___secondaryRow.gameObject);
      }

      static void ToggleDarkness(KButton button)
      {
        Darkness.Behavior.enabled = !Darkness.Behavior.enabled;

        button.GetComponent<ToolTip>().SetSimpleTooltip(Darkness.Behavior.enabled ? DARKNESS_ACTIVE_TOOLTIP : DARKNESS_DISABLED_TOOLTIP);
        button.bgImage.colorStyleSetting = Darkness.Behavior.enabled ? PUITuning.Colors.ButtonBlueStyle : PUITuning.Colors.ButtonPinkStyle;
        button.fgImage.sprite = Assets.GetSprite(Darkness.Behavior.enabled ? "icon_category_lights_disabled" : "icon_category_lights");
      }

      static void ShowOptions()
      {
        var OptionsDialog = Type.GetType("PeterHan.PLib.Options.OptionsDialog");
        var dialog = OptionsDialog.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(Type) }, null).Invoke(new object[] { typeof(Config) });

        Action<object> onClose = (object config) =>
        {
          Config.Instance = config as Config;
        };
        OptionsDialog.GetProperty("OnClose").SetValue(dialog, onClose);

        OptionsDialog.GetMethod("ShowDialog").Invoke(dialog, BindingFlags.NonPublic, null, new object[] { }, null);

        var screen = (KScreen)OptionsDialog.GetField("dialog", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(dialog);
        screen.ConsumeMouseScroll = true;
      }
    }
  }
}
