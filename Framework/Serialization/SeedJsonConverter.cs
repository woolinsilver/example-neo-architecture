using System.Text.Json;
using System.Text.Json.Serialization;
using SGStudios.Random;

namespace Framework.Serialization;

public class SeedJsonConverter : JsonConverter<Seed>
{
    public override Seed? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var hex = reader.GetString();
        return hex == null ? null : Seed.From(hex);
    }

    public override void Write(Utf8JsonWriter writer, Seed value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.HexString);
    }
}
