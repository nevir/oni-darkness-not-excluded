using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Globalization;

namespace DarknessNotIncluded
{
  public class ColorJsonConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(Color);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      var jsonValue = reader.Value;
      if (jsonValue is string stringValue && ColorSerializer.FromString(stringValue, out Color color))
      {
        return color;
      }
      else
      {
        throw new JsonException($"received invalid JSON when parsing Color: {jsonValue} ({jsonValue.GetType()})");
      }
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      if (value is Color color)
      {
        writer.WriteValue(ColorSerializer.ToString(color));
      }
      else
      {
        throw new JsonException($"expected to receive an instance of Color; instead got: {value}");
      }
    }
  }

  public static class ColorSerializer
  {
    public static bool FromString(string colorString, out Color outColor)
    {
      bool success = true;
      if (colorString.Length == 8)
      {
        success |= int.TryParse(colorString.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int r);
        success |= int.TryParse(colorString.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int g);
        success |= int.TryParse(colorString.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int b);
        success |= int.TryParse(colorString.Substring(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int a);

        if (success)
        {
          outColor = new Color(r, g, b, a);
          return true;
        }
      }

      outColor = Color.clear;
      return false;
    }

    public static string ToString(Color color)
    {
      string result = "";
      result += ((int)color.r).ToString("X2");
      result += ((int)color.g).ToString("X2");
      result += ((int)color.b).ToString("X2");
      result += ((int)color.a).ToString("X2");

      return result;
    }
  }
}
