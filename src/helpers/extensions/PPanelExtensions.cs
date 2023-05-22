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
      var children = Traverse.Create(panel).Field("children").GetValue<IEnumerable<object>>();

      foreach (var child in children)
      {
        var childName = Traverse.Create(child).Property("Name").GetValue<string>();
        if (childName == name) return child as IUIComponent;
      }

      return null;
    }
  }
}
