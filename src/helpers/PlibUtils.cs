using UnityEngine;
using TMPro;

namespace DarknessNotIncluded
{
  public static class PlibUtils
  {
    public static void SetFieldText(GameObject field, string newText)
    {
      if (field == null) return;
      var input = field.GetComponentInChildren<TMP_InputField>();
      if (input == null) return;
      input.text = newText;
    }
  }
}
