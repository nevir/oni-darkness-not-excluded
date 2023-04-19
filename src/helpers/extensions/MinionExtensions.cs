
namespace DarknessNotIncluded
{
  public static class MinionExtensions
  {
    public static bool IsSleeping(this MinionIdentity minionIdentity)
    {
      if (minionIdentity == null) return false;
      if (minionIdentity.gameObject == null) return false;

      var staminaMonitor = minionIdentity.GetSMI<StaminaMonitor.Instance>();
      if (staminaMonitor == null) return false;

      return staminaMonitor.IsSleeping();
    }
  }
}
