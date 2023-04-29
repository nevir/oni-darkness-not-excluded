using HarmonyLib;
using UnityEngine;
using PeterHan.PLib.UI;
using STRINGS;

namespace DarknessNotIncluded.InGameConfig
{
  static class DarknessToggleButton
  {
    static LocString DARKNESS_ACTIVE_TOOLTIP = $"<style=\"consumed\">Disable Darkness</style>\n\nGetting to be a little too much? It's ok, we won't judge you …too much.\n\n{UI.PRE_KEYWORD}Right-click to view <b>Darkness Not Excluded</b> options{UI.PST_KEYWORD}";
    static LocString DARKNESS_DISABLED_TOOLTIP = $"<style=\"produced\">Re-Enable Darkness</style>\n\nAh! Welcome back to the challenge.\n\n{UI.PRE_KEYWORD}Right-click to view <b>Darkness Not Excluded</b> options{UI.PST_KEYWORD}";

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
              OptionsEnhancements.OptionsDialog.Show<Config>(typeof(Config), (config) => Config.Set(config));
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
    }
  }
}
