namespace Framework.Domain;

public class Tier
{
    public required string Identifier { get; init; }
    public required ulong Weight { get; init; }
    public required decimal Prize { get; init; }
}
