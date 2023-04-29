using HarmonyLib;
using PeterHan.PLib.UI;
using System.Collections.Generic;

namespace DarknessNotIncluded
{
  public static class PPanelExtensions
  {
    public static IUIComponent GetChildNamed(this PPanel panel, string name)
    {
      if (panel == null) return null;
      var children = Traverse.Create(panel).Field("children").GetValue<ICollection<IUIComponent>>();

      foreach (var child in children)
      {
        if (child.Name == name) return child;
      }

      return null;
    }
  }
}
