using SGStudios.Random.Generator;

namespace Framework.Service;

/// <summary>
/// The random data required to generate one ticket.
/// </summary>
public class TicketRandomData
{
    public required uint Number { get; init; }
    public required IDeterministicGenerator Generator { get; init; }
}
