using System;
using System.Collections.Generic;

namespace DarknessNotIncluded
{
  public class ConfigObserver
  {
    static List<Action<Config>> observers = new List<Action<Config>>();

    public ConfigObserver(Action<Config> observer)
    {
      observers.Add(observer);
      observer(Config.Instance);
    }

    public static void UpdateObservers(Config config)
    {
      foreach (var observer in observers)
      {
        observer(config);
      }
    }
  }
}
