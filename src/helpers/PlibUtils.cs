using HarmonyLib;
using System;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace DarknessNotIncluded
{
  public static class PLibUtils
  {
    public static void SetFieldText(GameObject field, string newText)
    {
      if (field == null) return;
      var input = field.GetComponentInChildren<TMP_InputField>();
      if (input == null) return;
      input.text = newText;
    }

    public static Assembly ActiveAssembly()
    {
      return Global.Instance?.gameObject?.GetComponent("PRegistryComponent")?.GetType()?.Assembly;
    }

    public static Type ActiveType(string name)
    {
      return ActiveAssembly().GetType($"PeterHan.PLib.{name}");
    }

    public static string ActiveVersion()
    {
      var PVersion = ActiveType("PVersion");

      Log.Info($"PVersion: {PVersion}");
      // return PVersion "???";
      return PVersion.GetField("VERSION").GetValue(null) as string;
    }
  }
}
