using HarmonyLib;
using Klei.AI;

namespace DarknessNotIncluded
{
  public static class MinionDarkEffects
  {
    public static Effect DarkEffect;

    public static Effect DimEffect;

    [HarmonyPatch(typeof(ModifierSet)), HarmonyPatch("Initialize")]
    static class Patched_ModifierSet_Initialize
    {
      public static void Postfix(ModifierSet __instance)
      {
        DarkEffect = new Effect("Dark", "Dark", "It's so dark the minion is stumbling over their toes", 0, true, false, true)
        {
          SelfModifiers = {
            new AttributeModifier("Athletics", -5)
          }
        };
        DimEffect = new Effect("Dim", "Dim", "Not great light", 0, true, false, true)
        {
          SelfModifiers = {
            new AttributeModifier("Athletics", -2)
          }
        };

        __instance.effects.Add(DarkEffect);
        __instance.effects.Add(DimEffect);
      }
    }

    [HarmonyPatch(typeof(RationalAi)), HarmonyPatch("InitializeStates")]
    static class Patched_RationalAi_InitializeStates
    {
      static void Postfix(RationalAi __instance)
      {
        __instance.alive.ToggleStateMachine(smi => new LightMonitor.Instance(smi.master));
      }
    }

    public class LightMonitor : GameStateMachine<LightMonitor, LightMonitor.Instance>
    {
      public State lit;
      public State dim;
      public State dark;

      public FloatParameter lightLevel;

      public override void InitializeStates(out StateMachine.BaseState default_state)
      {
        default_state = lit;

        root.Update(CheckLightLevel, UpdateRate.SIM_1000ms);

        var darkThreshold = 200;
        var dimThreshold = 1000;

        lit
          .ParamTransition(lightLevel, dim, (smi, p) => p > darkThreshold && p <= dimThreshold)
          .ParamTransition(lightLevel, dark, (smi, p) => p <= darkThreshold);

        dim
          .ParamTransition(lightLevel, lit, (smi, p) => p > dimThreshold)
          .ParamTransition(lightLevel, dark, (smi, p) => p <= darkThreshold)
          .ToggleEffect(DimEffect.Id);

        dark
          .ParamTransition(lightLevel, dim, (smi, p) => p > darkThreshold && p <= dimThreshold)
          .ParamTransition(lightLevel, lit, (smi, p) => p > dimThreshold)
          .ToggleEffect(DarkEffect.Id);
      }

      private static void CheckLightLevel(LightMonitor.Instance smi, float dt)
      {
        KPrefabID component = smi.GetComponent<KPrefabID>();
        if (component != null && component.HasTag(GameTags.Shaded))
        {
          double num1 = smi.sm.lightLevel.Set(0.0f, smi);
        }
        else
        {
          int cell = Grid.PosToCell(smi.gameObject);
          if (!Grid.IsValidCell(cell)) return;
          smi.sm.lightLevel.Set(Grid.LightIntensity[cell], smi);
        }
      }

      public new class Instance : GameInstance
      {
        public Effects effects;

        public Instance(IStateMachineTarget master)
          : base(master)
        {
          this.effects = this.GetComponent<Effects>();
        }
      }
    }
  }
}
