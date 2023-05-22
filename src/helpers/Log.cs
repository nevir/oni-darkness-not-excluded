using System;

namespace DarknessNotIncluded
{
  static class Log
  {
    static string LINE_FORMAT = $"[Darkness Not Excluded v{Mod.Version.Major}.{Mod.Version.Minor}.{Mod.Version.Build}] {{0}}";

    public static void Info(string message, params object[] details)
    {
      Debug.Log(Format(message, details));
    }

    public static void Warn(string message, params object[] details)
    {
      Debug.LogWarning(Format(message, details));
    }

    public static void Error(string message, params object[] details)
    {
      Debug.LogError(Format(message, details));
    }

    public static void Assert(string message, bool condition)
    {
      if (condition) return;

      Debug.LogError(Format("Assertion failed: {{0}}", message));
      throw new Exception($"Assertion failed: {message}");
    }

    static string Format(string message, params object[] details)
    {
      var fullMessage = message;
      foreach (var detail in details)
      {
        fullMessage += $" {detail}";
      }

      return String.Format(LINE_FORMAT, fullMessage);
    }
  }
}
