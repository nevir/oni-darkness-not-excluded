using DarknessNotIncluded.OptionsEnhancements;
using HarmonyLib;
using KMod;
using Newtonsoft.Json;
using PeterHan.PLib;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace DarknessNotIncluded
{
  [JsonObject(MemberSerialization.OptOut)]
  [ModInfo("https://github.com/nevir/oni-darkness-not-excluded", "preview.png")]
  [ConfigFile(SharedConfigLocation: true)]
  public class Config
  {
    // Darkness

    [Option("Lux threshold", "At what lux should a tile be fully visible?", "Darkness")]
    public int fullyVisibleLuxThreshold { get; set; }

    [Option("Unlit tiles hide details", "Whether unlit tiles should only show basic details when hovered", "Darkness")]
    public bool selectToolBlockedByDarkness { get; set; }

    [Option("Darkness grace period (cycles)", "How many cycles should it take to go from initial darkness to maximum darkness?", "Darkness")]
    public float gracePeriodCycles { get; set; }

    [Option("Initial darkness level", "How dark should 0 lux tiles be at the start of the game?\n0 = pitch black, 255 = fully visible", "Darkness")]
    [Limit(0, 255)]
    public int initialFogLevel { get; set; }

    [Option("Maximum darkness level", "How dark should 0 lux tiles be after the initial grace period?\n0 = pitch black, 255 = fully visible", "Darkness")]
    [Limit(0, 255)]
    public int minimumFogLevel { get; set; }

    // Exploration

    [Option("Perform actions on unexplored cells", "Whether you are able to drag actions (like dig) on unexplored cells", "Exploration")]
    public bool dragToolIgnoresVisibility { get; set; }

    [Option("Printing Pod reveal (radius)", "How many tiles should be revealed around the starting point?", "Exploration")]
    public int telepadRevealRadius { get; set; }

    // Buildings

    [DynamicOption(typeof(BuildingLightingConfigEntry), "Building Lights")]
    [Option("Configuration for building lights", "Configuration for building lights", "Building Lights")]
    public BuildingLightingConfig buildingLightingConfig { get; set; }

    // Plants

    [Option("Max lux tolerated by Dusk Caps", "How much light Dusk Caps can handle before having stunted growth.", "Plants")]
    public int duskCapPlantMaxLux { get; set; }

    [Option("Max lux tolerated by Bog Buckets", "How much light Bog Buckets can handle before having stunted growth.", "Plants")]
    public int bogBucketPlantMaxLux { get; set; }

    // Light Bonuses

    [Option("Decor bonus threshold (lux)", "At what lux should decor get a bonus for being well lit?", "Light Bonuses")]
    public int decorBonusThresholdLux { get; set; }

    [Option("Lux required for Lit Workspace bonus", "The minimum amount of lux before a Duplicant will receive the Lit Workspace speed bonus", "Light Bonuses")]
    public int litWorkspaceLux { get; set; }

    // Darkness Penalties

    [Option("Maximum lux tolerated while sleeping", "The maximum lux that a dupe can handle before being rudely woken up.", "Darkness Penalties")]
    public int maxSleepingLux { get; set; }

    [DynamicOption(typeof(MinionEffectsConfigEntry), "Darkness Penalties")]
    [Option("Configuration for dupe effects", "Configuration for dupe effects", "Darkness Penalties")]
    public MinionEffectsConfig minionEffectsConfig { get; set; }

    // Duplicant Lights

    [Option("Disable lights around sleeping dupes", "Whether dupes should turn their lights off when entering a bedroom.", "Duplicant Lights")]
    public bool disableDupeLightsInBedrooms { get; set; }

    [Option("Disable lights in lit areas", "Whether dupes should turn their lights off when entering an area with at least the same level of brightness as their light.", "Duplicant Lights")]
    public bool disableDupeLightsInLitAreas { get; set; }

    [DynamicOption(typeof(MinionLightingConfigEntry), "Duplicant Lights")]
    [Option("Configuration for dupe lights", "Configuration for dupe lights", "Duplicant Lights")]
    public MinionLightingConfig minionLightingConfig { get; set; }

    // Presets

    [OptionPreset("The Deep Dark")]
    public static Config TheDeepDark()
    {
      return new Config()
      {
        gracePeriodCycles = 0.0f,
        initialFogLevel = 0,
        minimumFogLevel = 0,
      };
    }

    [OptionPreset("Visuals Only")]
    public static Config VisualsOnly()
    {
      return new Config(true, true)
      {
        selectToolBlockedByDarkness = false,
        dragToolIgnoresVisibility = false,
        telepadRevealRadius = 18,
      };
    }

    public Config() : this(false, false) { }

    public Config(bool disableMinionEffects, bool defaultMinionReveal)
    {
      // Darkness
      fullyVisibleLuxThreshold = 1000;
      selectToolBlockedByDarkness = true;
      gracePeriodCycles = 3.0f;
      initialFogLevel = 200;
      minimumFogLevel = 50;

      // Exploration
      dragToolIgnoresVisibility = true;
      telepadRevealRadius = 0;

      // Buildings
      buildingLightingConfig = new BuildingLightingConfig {
        { BuildingType.PrintingPod, new LightConfig(true, 2000, 5, 0, LightShape.Circle, LIGHT2D.LIGHT_YELLOW) },
        { BuildingType.MicrobeMusher, new LightConfig(true, 1000, 1, 0, LightShape.Circle, LIGHT2D.LIGHT_YELLOW) },
        { BuildingType.HydrogenGenerator, new LightConfig(true, 1000, 1, 0, LightShape.Circle, LIGHT2D.LIGHT_YELLOW) },
      };

      // Plants
      duskCapPlantMaxLux = 500;
      bogBucketPlantMaxLux = 500;

      // Light Bonuses
      decorBonusThresholdLux = 1000;
      litWorkspaceLux = 1000;

      // Darkness Penalties
      maxSleepingLux = 500;

      minionEffectsConfig = new MinionEffectsConfig {
        { MinionEffectType.Dim, new MinionEffectsConfig.EffectConfig(true, 700, -2, -1) },
        { MinionEffectType.Dark, new MinionEffectsConfig.EffectConfig(true, 300, -5, -2) },
      };
      if (disableMinionEffects)
      {
        foreach (var effect in minionEffectsConfig)
        {
          effect.Value.enabled = false;
          effect.Value.agilityModifier = 0;
          effect.Value.statsModifier = 0;
        }
      }

      // Duplicant Lights

      disableDupeLightsInLitAreas = true;
      disableDupeLightsInBedrooms = true;
      minionLightingConfig = new MinionLightingConfig {
        { MinionLightType.Intrinsic, new LightConfig(true,  200,  2, 0, LightShape.Pill,         Color.white) },
        { MinionLightType.Mining1,   new LightConfig(true,  800,  3, 6, LightShape.DirectedCone, LIGHT2D.LIGHT_YELLOW)},
        { MinionLightType.Mining2,   new LightConfig(true,  1000, 4, 7, LightShape.DirectedCone, LIGHT2D.LIGHT_YELLOW)},
        { MinionLightType.Mining3,   new LightConfig(true,  1200, 5, 8, LightShape.DirectedCone, Color.white)},
        { MinionLightType.Mining4,   new LightConfig(true,  1400, 6, 9, LightShape.DirectedCone, Color.white)},
        { MinionLightType.Science,   new LightConfig(true,  800,  3, 0, LightShape.Pill,         Color.white)},
        { MinionLightType.Rocketry,  new LightConfig(true,  800,  4, 0, LightShape.DirectedCone, Color.white)},
        { MinionLightType.AtmoSuit,  new LightConfig(true,  600,  3, 0, LightShape.Pill,         LIGHT2D.LIGHT_YELLOW)},
        { MinionLightType.JetSuit,   new LightConfig(true,  800,  5, 7, LightShape.DirectedCone, LIGHT2D.LIGHT_YELLOW)},
        { MinionLightType.LeadSuit,  new LightConfig(true,  400,  3, 0, LightShape.Pill,         LIGHT2D.LIGHT_YELLOW)},
        { MinionLightType.Rover,     new LightConfig(true,  1400, 6, 0, LightShape.DirectedCone, Color.white) },
      };

      if (defaultMinionReveal)
      {
        foreach (var lightConfig in minionLightingConfig)
        {
          lightConfig.Value.reveal = 30;
        }
      }
    }

    private static List<Action<Config>> observers = new List<Action<Config>>();

    public static void ObserveFor(object target, Action<Config> observer)
    {
      var targetRef = new WeakReference(target);
      Action<Config> wrappedObserver = null;
      wrappedObserver = (config) =>
      {
        if (targetRef.Target == null)
        {
          observers.Remove(wrappedObserver);
          return;
        }
        observer(config);
      };

      observers.Add(wrappedObserver);
      observer(Get());
    }

    public class Observer
    {
      public Observer(Action<Config> observer)
      {
        observers.Add(observer);
        observer(Get());
      }
    }

    private static Config instance;
    private static Config Get()
    {
      if (instance == null)
      {
        instance = POptions.ReadSettings<Config>() ?? new Config();
      }
      return instance;
    }
    public static void Set(Config newConfig)
    {
      instance = newConfig;
      foreach (var observer in observers)
      {
        observer(newConfig);
      }
    }

    public static void Initialize(UserMod2 mod)
    {
      new POptions().RegisterOptions(mod, typeof(Config));
    }

    [HarmonyPatch(typeof(Game)), HarmonyPatch("OnPrefabInit")]
    static class Patched_Game_OnPrefabInit
    {
      static void Prefix()
      {
        Config.instance = POptions.ReadSettings<Config>();
      }
    }
  }
}
