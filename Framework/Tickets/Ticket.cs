using System.Text.Json.Serialization;
using Framework.Serialization;
using SGStudios.Random;

namespace Framework.Tickets;

public class Ticket
{
    public required string Tier { get; init; }

    [JsonConverter(typeof(SeedJsonConverter))]
    public required Seed Seed { get; init; }

    public required decimal Prize { get; init; }
}
