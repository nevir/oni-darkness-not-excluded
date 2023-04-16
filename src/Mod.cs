using HarmonyLib;
using KMod;
using System;

namespace DarknessNotIncluded
{
  public class Mod : UserMod2
  {
    public override void OnLoad(Harmony harmony)
    {
      Console.WriteLine($"Loaded [DarknessNotIncluded]");
      base.OnLoad(harmony);
    }
  }
}
