using PeterHan.PLib.Options;
using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarknessNotIncluded
{
  public class MinionEffectsConfigEntry : OptionsEntry
  {
    static Dictionary<MinionEffectType, string> LABELS = new Dictionary<MinionEffectType, string>()
    {
      { MinionEffectType.Dim, "Dim" },
      { MinionEffectType.Dark, "Dark" },
    };

    static Dictionary<MinionEffectType, string> TOOLTIPS = new Dictionary<MinionEffectType, string>()
    {
      { MinionEffectType.Dim, "How should dupes be effected by dim light?" },
      { MinionEffectType.Dark, "How should dupes be effected by darkness?" },
    };

    public MinionEffectsConfigEntry(string field, IOptionSpec spec) : base(field, spec) { }

    private Dictionary<MinionEffectType, GameObject> enabledComponents;
    private Dictionary<MinionEffectType, GameObject> luxThresholdComponents;
    private Dictionary<MinionEffectType, GameObject> statsModifierComponents;

    public override void CreateUIEntry(PGridPanel parent, ref int parentRow)
    {
      var grid = new PGridPanel("MinionLightingConfig") { FlexSize = Vector2.right };

      enabledComponents = new Dictionary<MinionEffectType, GameObject>();
      luxThresholdComponents = new Dictionary<MinionEffectType, GameObject>();
      statsModifierComponents = new Dictionary<MinionEffectType, GameObject>();

      grid.AddColumn(new GridColumnSpec());
      grid.AddColumn(new GridColumnSpec(flex: 1.0f));
      grid.AddColumn(new GridColumnSpec());
      grid.AddColumn(new GridColumnSpec());

      // Header

      grid.AddRow(new GridRowSpec(flex: 1.0f));
      grid.AddChild(new PLabel() { Text = "lux", TextStyle = PUITuning.Fonts.TextLightStyle }, new GridComponentSpec(0, 2));
      grid.AddChild(new PLabel() { Text = "stats", TextStyle = PUITuning.Fonts.TextLightStyle }, new GridComponentSpec(0, 3));

      // Light Types

      int row = 1;
      foreach (MinionEffectType type in Enum.GetValues(typeof(MinionEffectType)))
      {
        var name = Enum.GetName(typeof(MinionEffectType), type);

        grid.AddRow(new GridRowSpec(flex: 1.0f));

        var checkBox = new PCheckBox($"{name}.enabled")
        {
          OnChecked = (o, state) =>
          {
            this.value[type].enabled = state != PCheckBox.STATE_CHECKED;
            UpdateComponents();
          }
        };
        checkBox.SetKleiBlueStyle();
        checkBox.AddOnRealize(o => enabledComponents.Add(type, o));
        grid.AddChild(checkBox, new GridComponentSpec(row, 0) { Margin = LABEL_MARGIN });

        var label = new PLabel($"{name}.label")
        {
          Text = LABELS[type],
          ToolTip = TOOLTIPS[type],
          TextStyle = PUITuning.Fonts.TextLightStyle,
        };
        grid.AddChild(label, new GridComponentSpec(row, 1) { Margin = LABEL_MARGIN, Alignment = TextAnchor.MiddleLeft });

        var luxField = new PTextField($"{name}.luxThreshold")
        {
          Type = PTextField.FieldType.Integer,
          MinWidth = 48,
          OnTextChanged = (o, text) =>
          {
            if (int.TryParse(text, out int newLux))
            {
              this.value[type].luxThreshold = newLux;
              UpdateComponents();
            }
          }
        };
        luxField.AddOnRealize(o => luxThresholdComponents.Add(type, o));
        grid.AddChild(luxField, new GridComponentSpec(row, 2) { Margin = LABEL_MARGIN });

        var rangeField = new PTextField($"{name}.statsModifier")
        {
          Type = PTextField.FieldType.Integer,
          MinWidth = 48,
          OnTextChanged = (o, text) =>
          {
            if (int.TryParse(text, out int newRange))
            {
              this.value[type].statsModifier = newRange;
              UpdateComponents();
            }
          }
        };
        rangeField.AddOnRealize(o => statsModifierComponents.Add(type, o));
        grid.AddChild(rangeField, new GridComponentSpec(row, 3) { Margin = LABEL_MARGIN });

        row++;
      }

      parent.AddChild(grid, new GridComponentSpec(parentRow, 0));

      UpdateComponents();
    }

    void UpdateComponents()
    {
      if (this.value == null) return;

      foreach (var pair in this.value)
      {
        if (!enabledComponents.ContainsKey(pair.Key)) continue;

        PCheckBox.SetCheckState(enabledComponents[pair.Key], pair.Value.enabled ? PCheckBox.STATE_CHECKED : PCheckBox.STATE_UNCHECKED);
        PlibUtils.SetFieldText(luxThresholdComponents[pair.Key], pair.Value.luxThreshold.ToString());
        PlibUtils.SetFieldText(statsModifierComponents[pair.Key], pair.Value.statsModifier.ToString());
      }
    }

    public override GameObject GetUIComponent()
    {
      throw new NotImplementedException();
    }

    private MinionEffectsConfig value;

    public override object Value
    {
      get => value;
      set
      {
        if (value is MinionEffectsConfig newValue)
        {
          this.value = newValue.DeepClone();
          UpdateComponents();
        }
        else
        {
          throw new ArgumentException("Expected a MinionEffectsConfig");
        }
      }
    }
  }
}
