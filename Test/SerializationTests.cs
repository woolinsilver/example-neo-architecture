using System.Text.Json;
using Framework.Tickets;
using SGStudios.Random;
using Xunit;

namespace Test;

public class SerializationTests
{
    [Fact]
    public void Ticket_ShouldSerializeSeedAsHexString()
    {
        var ticket = new Ticket
        {
            Tier = "Gold",
            Seed = Seed.From(0x12345678),
            Prize = 100.00m
        };

        var json = JsonSerializer.Serialize(ticket);

        Assert.DoesNotContain("\"Seed\":{", json);
        Assert.Contains("\"Seed\":\"0x12345678\"", json);
    }
}
