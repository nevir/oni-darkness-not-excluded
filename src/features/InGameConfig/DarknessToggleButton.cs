using HarmonyLib;
using UnityEngine;
using PeterHan.PLib.UI;

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
          OnClick = (source) =>
          {
            Darkness.FadeUnlitCells.doNotFade = !Darkness.FadeUnlitCells.doNotFade;
            var darknessActive = !Darkness.FadeUnlitCells.doNotFade;

            var kButton = source.GetComponent<KButton>();
            kButton.GetComponent<ToolTip>().SetSimpleTooltip(darknessActive ? DARKNESS_ACTIVE_TOOLTIP : DARKNESS_DISABLED_TOOLTIP);
            kButton.bgImage.colorStyleSetting = darknessActive ? PUITuning.Colors.ButtonBlueStyle : PUITuning.Colors.ButtonPinkStyle;
            kButton.fgImage.sprite = Assets.GetSprite(darknessActive ? "icon_category_lights_disabled" : "icon_category_lights");
          }
        };
        button.AddTo(___secondaryRow.gameObject);
      }
    }
  }
}
