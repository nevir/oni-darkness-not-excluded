using HarmonyLib;
using System.Reflection;
using PeterHan.PLib.UI;
using System;
using UnityEngine;

namespace DarknessNotIncluded.OptionsEnhancements
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public class OptionPreset : Attribute
  {
    public string Name { get; }

    public OptionPreset(string name)
    {
      Name = name;
    }
  }

  static class OptionsDialogPresets
  {
    [HarmonyPatch]
    static class Patched_OptionsDialog_AddModInfoScreen
    {
      static MethodBase TargetMethod()
      {
        var OptionsDialog = AccessTools.TypeByName("PeterHan.PLib.Options.OptionsDialog");
        return AccessTools.Method(OptionsDialog, "AddModInfoScreen");
      }

      static void Postfix(object __instance, PDialog optionsDialog)
      {
        var body = optionsDialog.Body;
        var infoPanel = (PPanel)body.GetChildNamed("ModInfo");
        // TODO: Support horizontal mode
        if (infoPanel == null) return;

        infoPanel.Margin = new RectOffset(0, 10, 0, 0);

        var modImage = (PLabel)infoPanel.GetChildNamed("ModImage");
        if (modImage != null)
        {
          modImage.Margin = new RectOffset(0, 0, 0, 0);
        }

        var container = new PPanel("PresetsContainer")
        {
          Margin = new RectOffset(0, 0, 20, 0),
          Direction = PanelDirection.Vertical,
          FlexSize = new Vector2(1.0f, 0.0f),
        };
        infoPanel.AddChild(container);

        var presetsHeader = new PLabel("PresetsLabel")
        {
          Text = "Presets",
          TextStyle = PUITuning.Fonts.UILightStyle,
          TextAlignment = TextAnchor.UpperCenter,
          Margin = new RectOffset(0, 0, 0, 10),
        };
        container.AddChild(presetsHeader);

        var OptionsType = Traverse.Create(__instance).Field("optionsType").GetValue<Type>();

        var defaultConfigButton = new PButton($"DefaultConfig")
        {
          Text = "Default",
          FlexSize = new Vector2(1.0f, 0.0f),
          Margin = PDialog.BUTTON_MARGIN,
          OnClick = (source) =>
          {
            SetConfig(__instance, AccessTools.Constructor(OptionsType).Invoke(new object[] { }));
          }
        }.SetKleiPinkStyle();
        container.AddChild(defaultConfigButton);

        foreach (var method in OptionsType.GetMethods((BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)))
        {
          var presetAttribute = method.GetCustomAttribute<OptionPreset>();
          if (presetAttribute == null) continue;

          var presetButton = new PButton($"Preset.{method.Name}")
          {
            Text = presetAttribute.Name,
            FlexSize = new Vector2(1.0f, 0.0f),
            Margin = PDialog.BUTTON_MARGIN,
            OnClick = (source) =>
            {
              SetConfig(__instance, method.Invoke(null, new object[] { }));
            }
          }.SetKleiPinkStyle();
          container.AddChild(presetButton);
        }
      }

      static void SetConfig(object optionsDialog, object config)
      {
        Traverse.Create(optionsDialog).Field("options").SetValue(config);
        Traverse.Create(optionsDialog).Method("UpdateOptions").GetValue();
      }
    }
  }
}
