
namespace DarknessNotIncluded
{
  public static class MinionUtils
  {
    public static bool IsSleeping(MinionIdentity minionIdentity)
    {
      var staminaMonitor = minionIdentity.GetSMI<StaminaMonitor.Instance>();
      if (staminaMonitor == null) return false;

      return staminaMonitor.IsSleeping();
    }
  }
}
