using HarmonyLib;
using UnityEngine;

namespace DarknessNotIncluded
{

  [HarmonyPatch(typeof(MinionConfig)), HarmonyPatch("CreatePrefab")]
  static class MinionLighting
  {
    static void Postfix(GameObject __result)
    {
      __result.AddOrGet<MinionLights>();
    }
  }

  public class MinionLights : KMonoBehaviour, ISim33ms
  {
    [MyCmpGet]
    private MinionIdentity _minionIdentity;

    public Light2D HeadLight { get; set; }
    public Light2D BodyLight { get; set; }

    private MinionLightType currentLightType;

    protected override void OnPrefabInit()
    {
      base.OnPrefabInit();

      HeadLight = _minionIdentity.gameObject.AddComponent<Light2D>();
      HeadLight.shape = LightShape.Circle;
      HeadLight.Offset = new Vector2(0f, 1.0f);

      BodyLight = _minionIdentity.gameObject.AddComponent<Light2D>();
      BodyLight.shape = LightShape.Circle;
      BodyLight.Offset = new Vector2(0f, 0.0f);

      UpdateLights();
    }

    protected override void OnSpawn()
    {
      base.OnSpawn();

      UpdateLights();
    }

    public void Sim33ms(float dt)
    {
      UpdateLights();
    }

    private void UpdateLights()
    {
      var config = Config.Instance;
      var lightType = GetActiveLightType();

      if (config.disableDupeLightsInBedrooms && lightType != MinionLightType.None)
      {
        Room currentRoom = Game.Instance.roomProber.GetRoomOfGameObject(this._minionIdentity.gameObject);
        var isInBedroom = currentRoom != null && currentRoom.roomType.category == Db.Get().RoomTypeCategories.Sleep;
        if (isInBedroom)
        {
          lightType = MinionLightType.None;
        }
      }

      if (config.disableDupeLightsInLitAreas && lightType != MinionLightType.None)
      {
        var headCell = Grid.CellAbove(Grid.PosToCell(_minionIdentity.gameObject));
        var dupeLux = (HeadLight.enabled ? HeadLight.Lux : 0) + (BodyLight.enabled ? BodyLight.Lux : 0);
        var baseCellLux = Grid.LightIntensity[headCell] - dupeLux;
        var targetLux = lightType.Config().lux;
        // Keep intrinsic lights on even if next to another dupe
        if (lightType == MinionLightType.Intrinsic) targetLux *= 2;

        if (baseCellLux >= targetLux)
        {
          lightType = MinionLightType.None;
        }
      }

      SetLightType(lightType);
    }

    private void SetLightType(MinionLightType lightType)
    {
      if (lightType == currentLightType) return;
      currentLightType = lightType;

      var lightConfig = lightType.Config();

      HeadLight.enabled = lightConfig.enabled;
      HeadLight.Lux = lightConfig.lux / 2;
      HeadLight.Range = lightConfig.range;
      HeadLight.Color = lightConfig.color;

      BodyLight.enabled = lightConfig.enabled;
      BodyLight.Lux = lightConfig.lux / 2;
      BodyLight.Range = lightConfig.range;
      BodyLight.Color = lightConfig.color;
    }

    private MinionLightType GetActiveLightType()
    {
      var lightingConfig = Config.Instance.minionLightingConfig;
      var lightType = GetLightTypeForCurrentState();

      if (!lightingConfig[lightType].enabled) lightType = MinionLightType.Intrinsic;
      if (!lightingConfig[lightType].enabled) lightType = MinionLightType.None;

      return lightType;
    }

    private MinionLightType GetLightTypeForCurrentState()
    {
      var resume = _minionIdentity.GetComponent<MinionResume>();
      var hat = resume.CurrentHat;

      if (hat != null && hat.StartsWith("hat_role_mining"))
      {
        if (hat == "hat_role_mining1") return MinionLightType.Mining1;
        else if (hat == "hat_role_mining2") return MinionLightType.Mining2;
        else if (hat == "hat_role_mining3") return MinionLightType.Mining3;
        else if (hat == "hat_role_mining4") return MinionLightType.Mining4;
        else return MinionLightType.Mining4;
      }
      else if (hat != null && hat.StartsWith("hat_role_research"))
      {
        return MinionLightType.Science;
      }
      else if (hat != null && hat.StartsWith("hat_role_astronaut"))
      {
        return MinionLightType.Rocketry;
      }
      else
      {
        return MinionLightType.Intrinsic;
      }
    }
  }
}
