using HarmonyLib;
using KMod;

namespace DarknessNotIncluded
{
  public class Mod : UserMod2
  {
    public override void OnLoad(Harmony harmony)
    {
      base.OnLoad(harmony);

      Config.Initialize(this);
      CustomLightShapes.Initialize();
    }
  }
}
