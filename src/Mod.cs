using HarmonyLib;
using KMod;
using PeterHan.PLib.Options;
using System;

namespace DarknessNotIncluded
{
  public class Mod : UserMod2
  {
    public override void OnLoad(Harmony harmony)
    {
      Console.WriteLine($"Loading [DarknessNotIncluded]");
      base.OnLoad(harmony);

      new POptions().RegisterOptions(this, typeof(Config));
      Console.WriteLine($"Loaded [DarknessNotIncluded]");
    }
  }
}
