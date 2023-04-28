using HarmonyLib;
using Klei.AI;
using System.Collections.Generic;

namespace DarknessNotIncluded.DarknessPenalties
{
  public static class MinionEffects
  {
    private static MinionEffectsConfig.EffectConfig dimConfig;
    private static MinionEffectsConfig.EffectConfig darkConfig;
    private static float gracePeriodCycles;

    private static ConfigObserver configObserver = new ConfigObserver((config) =>
    {
      var effectsConfig = config.minionEffectsConfig;

      dimConfig = config.minionEffectsConfig[MinionEffectType.Dim];
      darkConfig = config.minionEffectsConfig[MinionEffectType.Dark];
      gracePeriodCycles = config.gracePeriodCycles;
    });

    public static Effect DarkEffect;

    public static Effect DimEffect;

    [HarmonyPatch(typeof(ModifierSet)), HarmonyPatch("Initialize")]
    static class Patched_ModifierSet_Initialize
    {
      static List<AttributeModifier> BuildDimModifiers()
      {
        List<AttributeModifier> modifiers = new List<AttributeModifier>();
        foreach (var modifier in TUNING.DUPLICANTSTATS.ALL_ATTRIBUTES)
        {
          modifiers.Add(new AttributeModifier(modifier, dimConfig.statsModifier));
        }
        return modifiers;
      }

      static List<AttributeModifier> BuildDarkModifiers()
      {
        List<AttributeModifier> modifiers = new List<AttributeModifier>();
        foreach (var modifier in TUNING.DUPLICANTSTATS.ALL_ATTRIBUTES)
        {
          modifiers.Add(new AttributeModifier(modifier, darkConfig.statsModifier));
        }
        return modifiers;
      }

      static void Postfix(ModifierSet __instance)
      {

        DimEffect = new Effect("Dim", "Dim", "The poor lighting conditions are causing this Duplicant to exhibit poorer coordination than usual.", 0, true, false, true)
        {
          SelfModifiers = BuildDimModifiers()
        };
        __instance.effects.Add(DimEffect);

        DarkEffect = new Effect("Dark", "Dark", "This Duplicant can't see past its nose, and is struggling to perform even basic tasks.", 0, true, false, true)
        {
          SelfModifiers = BuildDarkModifiers()
        };
        __instance.effects.Add(DarkEffect);

        new ConfigObserver((config) =>
        {
          DimEffect.SelfModifiers = BuildDimModifiers();
          DarkEffect.SelfModifiers = BuildDarkModifiers();
        });
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
      public class LightStates : State
      {
        public State lit;
        public State dim;
        public State dark;
      }

      public State inactive;
      public LightStates active;

      public BoolParameter isActive;
      public FloatParameter lightLevel;


      public override void InitializeStates(out StateMachine.BaseState default_state)
      {
        default_state = inactive;

        root
          .Update(CheckActive, UpdateRate.SIM_1000ms);

        inactive
          .ParamTransition(isActive, active, (smi, p) => p);

        active
          .DefaultState(active.lit)
          .Update(CheckLightLevel, UpdateRate.SIM_1000ms)
          .ParamTransition(isActive, inactive, (smi, p) => !p);

        active.lit
          .ParamTransition(lightLevel, active.dim, (smi, p) => p > darkConfig.luxThreshold && p <= dimConfig.luxThreshold)
          .ParamTransition(lightLevel, active.dark, (smi, p) => p <= darkConfig.luxThreshold);

        active.dim
          .ParamTransition(lightLevel, active.lit, (smi, p) => p > dimConfig.luxThreshold)
          .ParamTransition(lightLevel, active.dark, (smi, p) => p <= darkConfig.luxThreshold)
          .Enter(smi =>
          {
            if (!dimConfig.enabled) return;
            smi.effects.Add(DimEffect.Id, false);
          })
          .Exit(smi =>
          {
            smi.effects.Remove(DimEffect.Id);
          });

        active.dark
          .ParamTransition(lightLevel, active.dim, (smi, p) => p > darkConfig.luxThreshold && p <= dimConfig.luxThreshold)
          .ParamTransition(lightLevel, active.lit, (smi, p) => p > dimConfig.luxThreshold)
          .Enter(smi =>
          {
            if (!darkConfig.enabled) return;
            smi.effects.Add(DarkEffect.Id, false);
          })
          .Exit(smi =>
          {
            smi.effects.Remove(DarkEffect.Id);
          });
      }

      private static void CheckActive(LightMonitor.Instance smi, float dt)
      {
        var minion = smi.GetComponent<MinionIdentity>();

        var isInactive = minion.IsSleeping() || GameClock.Instance.GetTimeInCycles() < gracePeriodCycles;

        smi.sm.isActive.Set(!isInactive, smi);
      }

      private static void CheckLightLevel(LightMonitor.Instance smi, float dt)
      {
        KPrefabID component = smi.GetComponent<KPrefabID>();
        if (component != null && component.HasTag(GameTags.Shaded))
        {
          smi.sm.lightLevel.Set(0.0f, smi);
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
