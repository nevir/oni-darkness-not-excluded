using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace DarknessNotIncluded
{
  public static class MinionRoomState
  {
    public static HashSet<MinionIdentity> EMPTY_HASH_SET = new HashSet<MinionIdentity>();

    public static Dictionary<int, HashSet<MinionIdentity>> byCavityIndex = new Dictionary<int, HashSet<MinionIdentity>>();

    public static bool SleepersInSameRoom(GameObject gameObject)
    {
      var cell = Grid.PosToCell(gameObject);
      var cavity = Game.Instance.roomProber.GetCavityForCell(cell);

      foreach (var minion in MinionsInRoom(cavity))
      {
        if (minion.IsSleeping()) return true;
      }
      return false;
    }

    public static HashSet<MinionIdentity> MinionsInRoom(CavityInfo cavity)
    {
      if (cavity != null && byCavityIndex.ContainsKey(cavity.handle.index))
      {
        return byCavityIndex[cavity.handle.index];
      }
      else
      {
        return EMPTY_HASH_SET;
      }
    }

    [HarmonyPatch(typeof(MinionConfig)), HarmonyPatch("CreatePrefab")]
    static class Patched_MinionConfig_CreatePrefab
    {
      static void Postfix(GameObject __result)
      {
        __result.AddOrGet<Observer>();
      }
    }

    public class Observer : KMonoBehaviour, ISim33ms
    {
      [MyCmpGet]
      private MinionIdentity _minionIdentity;

      private int currentCavityIndex = -1;

      public void Sim33ms(float dt)
      {
        var cell = Grid.PosToCell(_minionIdentity);
        var cavity = Game.Instance.roomProber.GetCavityForCell(cell);
        var cavityIndex = cavity == null || cavity.room == null ? -1 : cavity.handle.index;
        if (cavityIndex == currentCavityIndex) return;

        if (byCavityIndex.ContainsKey(currentCavityIndex))
        {
          byCavityIndex[currentCavityIndex].Remove(_minionIdentity);
          if (byCavityIndex[currentCavityIndex].Count == 0)
          {
            byCavityIndex.Remove(currentCavityIndex);
          }
        }

        if (cavityIndex == -1) return;

        if (!byCavityIndex.ContainsKey(cavityIndex))
        {
          byCavityIndex.Add(cavityIndex, new HashSet<MinionIdentity>());
        }
        byCavityIndex[cavityIndex].Add(_minionIdentity);
      }
    }
  }
}
