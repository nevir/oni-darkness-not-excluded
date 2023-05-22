using HarmonyLib;
using KMod;
using PeterHan.PLib;
using PeterHan.PLib.Core;
using System;

namespace DarknessNotIncluded
{
  public class Mod : UserMod2
  {
    public static Version Version = typeof(Mod).Assembly.GetName().Version;

    public override void OnLoad(Harmony harmony)
    {
      Log.Info("Loading Mod");

      base.OnLoad(harmony);

      PUtil.InitLibrary();

      Log.Info($"Bundled PLib version: {PVersion.VERSION}");
      Log.Info($" Active PLib version: {PLibUtils.ActiveVersion()} (via assembly: {PLibUtils.ActiveAssembly().FullName})");

      Config.Initialize(this);
      CustomLightShapes.Initialize();

      Log.Info("Loaded");
    }
  }
}
