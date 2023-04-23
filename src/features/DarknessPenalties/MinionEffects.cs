using HarmonyLib;
using Klei.AI;
using System.Collections.Generic;

namespace DarknessNotIncluded.DarknessPenalties
{
  public static class MinionEffects
  {
    public static Effect DarkEffect;

    public static Effect DimEffect;

    [HarmonyPatch(typeof(ModifierSet)), HarmonyPatch("Initialize")]
    static class Patched_ModifierSet_Initialize
    {
      static void Postfix(ModifierSet __instance)
      {
        var effectsConfig = Config.Instance.minionEffectsConfig;

        List<AttributeModifier> dimModifiers = new List<AttributeModifier>();
        foreach (var modifier in TUNING.DUPLICANTSTATS.ALL_ATTRIBUTES)
        {
          dimModifiers.Add(new AttributeModifier(modifier, effectsConfig[MinionEffectType.Dim].statsModifier));
        }
        DimEffect = new Effect("Dim", "Dim", "The poor lighting conditions are causing this Duplicant to exhibit poorer coordination than usual.", 0, true, false, true)
        {
          SelfModifiers = dimModifiers
        };
        __instance.effects.Add(DimEffect);

        List<AttributeModifier> darkModifiers = new List<AttributeModifier>();
        foreach (var modifier in TUNING.DUPLICANTSTATS.ALL_ATTRIBUTES)
        {
          darkModifiers.Add(new AttributeModifier(modifier, effectsConfig[MinionEffectType.Dark].statsModifier));
        }
        DarkEffect = new Effect("Dark", "Dark", "This Duplicant can't see past its nose, and is struggling to perform even basic tasks.", 0, true, false, true)
        {
          SelfModifiers = dimModifiers
        };
        __instance.effects.Add(DarkEffect);
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
        var config = Config.Instance;
        var effectsConfig = config.minionEffectsConfig;
        var dimThreshold = effectsConfig[MinionEffectType.Dim].luxThreshold;
        var darkThreshold = effectsConfig[MinionEffectType.Dark].luxThreshold;

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
          .ParamTransition(lightLevel, active.dim, (smi, p) => p > darkThreshold && p <= dimThreshold)
          .ParamTransition(lightLevel, active.dark, (smi, p) => p <= darkThreshold);

        active.dim
          .ParamTransition(lightLevel, active.lit, (smi, p) => p > dimThreshold)
          .ParamTransition(lightLevel, active.dark, (smi, p) => p <= darkThreshold);
        if (effectsConfig[MinionEffectType.Dim].enabled)
        {
          active.dim.ToggleEffect(DimEffect.Id);
        }

        active.dark
          .ParamTransition(lightLevel, active.dim, (smi, p) => p > darkThreshold && p <= dimThreshold)
          .ParamTransition(lightLevel, active.lit, (smi, p) => p > dimThreshold);
        if (effectsConfig[MinionEffectType.Dim].enabled)
        {
          active.dark.ToggleEffect(DarkEffect.Id);
        }
      }

      private static void CheckActive(LightMonitor.Instance smi, float dt)
      {
        var config = Config.Instance;
        var minion = smi.GetComponent<MinionIdentity>();

        var isInactive = minion.IsSleeping() || GameClock.Instance.GetTimeInCycles() < config.gracePeriodCycles;

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
