using PeterHan.PLib.Options;
using PeterHan.PLib.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarknessNotIncluded
{
  public class BuildingLightingConfigEntry : OptionsEntry
  {
    static Dictionary<BuildingType, string> LABELS = new Dictionary<BuildingType, string>()
    {
      { BuildingType.PrintingPod, "Printing Pod" },
      { BuildingType.MicrobeMusher, "Microbe Musher" },
    };

    static Dictionary<BuildingType, string> TOOLTIPS = new Dictionary<BuildingType, string>()
    {
      { BuildingType.PrintingPod, "Light that is emitted by the Printing Pod." },
      { BuildingType.MicrobeMusher, "Light that is emitted by the Microbe Musher while it is operating." },
    };

    public BuildingLightingConfigEntry(string field, IOptionSpec spec) : base(field, spec) { }

    private Dictionary<BuildingType, GameObject> enabledComponents;
    private Dictionary<BuildingType, GameObject> luxComponents;
    private Dictionary<BuildingType, GameObject> rangeComponents;
    private Dictionary<BuildingType, GameObject> revealComponents;

    public override void CreateUIEntry(PGridPanel parent, ref int parentRow)
    {
      var grid = new PGridPanel("BuildingLightingConfig") { FlexSize = Vector2.right };

      enabledComponents = new Dictionary<BuildingType, GameObject>();
      luxComponents = new Dictionary<BuildingType, GameObject>();
      rangeComponents = new Dictionary<BuildingType, GameObject>();
      revealComponents = new Dictionary<BuildingType, GameObject>();

      grid.AddColumn(new GridColumnSpec());
      grid.AddColumn(new GridColumnSpec(flex: 1.0f));
      grid.AddColumn(new GridColumnSpec());
      grid.AddColumn(new GridColumnSpec());
      grid.AddColumn(new GridColumnSpec());
      // TODO: Color selector.
      // grid.AddColumn(new GridColumnSpec());

      // Header

      grid.AddRow(new GridRowSpec(flex: 1.0f));
      grid.AddChild(new PLabel() { Text = "lux", TextStyle = PUITuning.Fonts.TextLightStyle }, new GridComponentSpec(0, 2));
      grid.AddChild(new PLabel() { Text = "range", TextStyle = PUITuning.Fonts.TextLightStyle }, new GridComponentSpec(0, 3));
      grid.AddChild(new PLabel() { Text = "reveal", TextStyle = PUITuning.Fonts.TextLightStyle }, new GridComponentSpec(0, 4));

      // Light Types

      int row = 1;
      foreach (BuildingType type in Enum.GetValues(typeof(BuildingType)))
      {
        if (type == BuildingType.None) continue;
        var name = Enum.GetName(typeof(BuildingType), type);

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

        var revealField = new PTextField($"{name}.reveal")
        {
          Type = PTextField.FieldType.Integer,
          MinWidth = 48,
          OnTextChanged = (o, text) =>
          {
            if (int.TryParse(text, out int newReveal))
            {
              this.value[type].reveal = newReveal;
              UpdateComponents();
            }
          }
        };
        revealField.AddOnRealize(o => revealComponents.Add(type, o));
        grid.AddChild(revealField, new GridComponentSpec(row, 4) { Margin = LABEL_MARGIN });

        // TODO: Color selector.
        // var colorButton = new PButton($"{name}.color")
        // {
        //   Sprite = Assets.GetSprite("icon_archetype_art"),
        //   SpriteSize = new Vector2(20, 20),
        // };
        // grid.AddChild(colorButton, new GridComponentSpec(row, 4));

        row++;
      }

      parent.AddChild(grid, new GridComponentSpec(parentRow, 0) { ColumnSpan = 2 });

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
        PlibUtils.SetFieldText(revealComponents[pair.Key], pair.Value.reveal.ToString());
      }
    }

    public override GameObject GetUIComponent()
    {
      throw new NotImplementedException();
    }

    private BuildingLightingConfig value;

    public override object Value
    {
      get => value;
      set
      {
        if (value is BuildingLightingConfig newValue)
        {
          this.value = newValue.DeepClone();
          UpdateComponents();
        }
        else
        {
          throw new ArgumentException("Expected a BuildingLightingConfig");
        }
      }
    }
  }
}
