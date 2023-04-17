using Newtonsoft.Json;

namespace DarknessNotIncluded
{
  [JsonObject(MemberSerialization.OptOut)]
  class Config
  {
    public float gracePeriodCycles { get; set; }

    public int initialFogLevel { get; set; }

    public int minimumFogLevel { get; set; }

    public int fullyVisibleLuxThreshold { get; set; }

    public Config()
    {
      initialFogLevel = 200;
      minimumFogLevel = 0;
      gracePeriodCycles = 0.2f;
      fullyVisibleLuxThreshold = 1800;
    }

    public static Config Instance = new Config();
  }
}
