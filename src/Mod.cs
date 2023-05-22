using HarmonyLib;
using KMod;
using PeterHan.PLib;
using PeterHan.PLib.Core;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace DarknessNotIncluded
{
  public class Mod : UserMod2
  {
    public static Version Version = typeof(Mod).Assembly.GetName().Version;

    public override void OnLoad(Harmony harmony)
    {
      base.OnLoad(harmony);

      PUtil.InitLibrary();

      Log.Info($"Bundled PLib version: {PVersion.VERSION}");
      Log.Info($" Active PLib version: {PLibUtils.ActiveVersion()} (via assembly: {PLibUtils.ActiveAssembly().FullName})");

      Config.Initialize(this);
      CustomLightShapes.Initialize();
    }
  }
}
