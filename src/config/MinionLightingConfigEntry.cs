using PeterHan.PLib.Options;
using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarknessNotIncluded
{
  public class MinionLightingConfigEntry : OptionsEntry
  {
    static Dictionary<MinionLightType, string> LABELS = new Dictionary<MinionLightType, string>()
    {
      { MinionLightType.Intrinsic, "Base Glow" },
      { MinionLightType.Mining1, STRINGS.DUPLICANTS.ROLES.JUNIOR_MINER.NAME },
      { MinionLightType.Mining2, STRINGS.DUPLICANTS.ROLES.MINER.NAME },
      { MinionLightType.Mining3, STRINGS.DUPLICANTS.ROLES.SENIOR_MINER.NAME },
      { MinionLightType.Mining4, STRINGS.DUPLICANTS.ROLES.MASTER_MINER.NAME },
      { MinionLightType.Science, "Science" },
      { MinionLightType.Rocketry, "Rocketry" },
      { MinionLightType.AtmoSuit, "Atmo Suit" },
      { MinionLightType.JetSuit, "Jet Suit" },
      { MinionLightType.LeadSuit, "Lead Suit" },
    };

    static Dictionary<MinionLightType, string> TOOLTIPS = new Dictionary<MinionLightType, string>()
    {
      { MinionLightType.Intrinsic, "Light that is emitted around Duplicants, when no other light sources are present." },
      { MinionLightType.Mining1, $"Light emitted by Duplicants wearing a {STRINGS.DUPLICANTS.ROLES.JUNIOR_MINER.NAME} mining hat." },
      { MinionLightType.Mining2, $"Light emitted by Duplicants wearing a {STRINGS.DUPLICANTS.ROLES.MINER.NAME} mining hat." },
      { MinionLightType.Mining3, $"Light emitted by Duplicants wearing a {STRINGS.DUPLICANTS.ROLES.SENIOR_MINER.NAME} mining hat." },
      { MinionLightType.Mining4, $"Light emitted by Duplicants wearing a {STRINGS.DUPLICANTS.ROLES.MASTER_MINER.NAME} mining hat." },
      { MinionLightType.Science, "Light emitted by Duplicants wearing a science hat." },
      { MinionLightType.Rocketry, "Light emitted by Duplicants wearing a rocketry helmet." },
      { MinionLightType.AtmoSuit, "Light emitted by Duplicants wearing an Atmo Suit" },
      { MinionLightType.JetSuit, "Light emitted by Duplicants wearing a Jet Suit" },
      { MinionLightType.LeadSuit, "Light emitted by Duplicants wearing a Lead Suit" },
    };

    public MinionLightingConfigEntry(string field, IOptionSpec spec) : base(field, spec) { }

    private Dictionary<MinionLightType, GameObject> enabledComponents;
    private Dictionary<MinionLightType, GameObject> luxComponents;
    private Dictionary<MinionLightType, GameObject> rangeComponents;

    public override void CreateUIEntry(PGridPanel parent, ref int parentRow)
    {
      var grid = new PGridPanel("MinionLightingConfig") { FlexSize = Vector2.right };

      enabledComponents = new Dictionary<MinionLightType, GameObject>();
      luxComponents = new Dictionary<MinionLightType, GameObject>();
      rangeComponents = new Dictionary<MinionLightType, GameObject>();

      grid.AddColumn(new GridColumnSpec());
      grid.AddColumn(new GridColumnSpec(flex: 1.0f));
      grid.AddColumn(new GridColumnSpec());
      grid.AddColumn(new GridColumnSpec());
      // TODO: Color selector.
      // grid.AddColumn(new GridColumnSpec());

      // Header

      grid.AddRow(new GridRowSpec(flex: 1.0f));
      grid.AddChild(new PLabel() { Text = "lux", TextStyle = PUITuning.Fonts.TextLightStyle }, new GridComponentSpec(0, 2));
      grid.AddChild(new PLabel() { Text = "range", TextStyle = PUITuning.Fonts.TextLightStyle }, new GridComponentSpec(0, 3));

      // Light Types

      int row = 1;
      foreach (MinionLightType type in Enum.GetValues(typeof(MinionLightType)))
      {
        if (type == MinionLightType.None) continue;
        var name = Enum.GetName(typeof(MinionLightType), type);

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

        var luxField = new PTextField($"{name}.lux")
        {
          Type = PTextField.FieldType.Integer,
          MinWidth = 48,
          OnTextChanged = (o, text) =>
          {
            if (int.TryParse(text, out int newLux))
            {
              this.value[type].lux = newLux;
              UpdateComponents();
            }
          }
        };
        luxField.AddOnRealize(o => luxComponents.Add(type, o));
        grid.AddChild(luxField, new GridComponentSpec(row, 2) { Margin = LABEL_MARGIN });

        var rangeField = new PTextField($"{name}.range")
        {
          Type = PTextField.FieldType.Integer,
          MinWidth = 48,
          OnTextChanged = (o, text) =>
          {
            if (int.TryParse(text, out int newRange))
            {
              this.value[type].range = newRange;
              UpdateComponents();
            }
          }
        };
        rangeField.AddOnRealize(o => rangeComponents.Add(type, o));
        grid.AddChild(rangeField, new GridComponentSpec(row, 3) { Margin = LABEL_MARGIN });

        // TODO: Color selector.
        // var colorButton = new PButton($"{name}.color")
        // {
        //   Sprite = Assets.GetSprite("icon_archetype_art"),
        //   SpriteSize = new Vector2(20, 20),
        // };
        // grid.AddChild(colorButton, new GridComponentSpec(row, 4));

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
        PlibUtils.SetFieldText(luxComponents[pair.Key], pair.Value.lux.ToString());
        PlibUtils.SetFieldText(rangeComponents[pair.Key], pair.Value.range.ToString());
      }
    }

    public override GameObject GetUIComponent()
    {
      throw new NotImplementedException();
    }

    private MinionLightingConfig value;

    public override object Value
    {
      get => value;
      set
      {
        if (value is MinionLightingConfig newValue)
        {
          this.value = newValue.DeepClone();
          UpdateComponents();
        }
        else
        {
          throw new ArgumentException("Expected a MinionLightingConfig");
        }
      }
    }
  }
}
