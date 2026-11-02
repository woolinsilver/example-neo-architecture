using Framework.Domain;

namespace Mermaid.Tickets;

public class MermaidTier : Tier
{
    public required MermaidOutcome Outcome { get; init; }
}
