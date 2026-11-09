using Framework.Domain;

namespace Mermaid.Tickets;

// The paytable(s) in the profile must comprise these tiers.
// So we could put anything here which the TicketFactory might need, even methods.

public class MermaidTier : Tier
{
    public required MermaidOutcome Outcome { get; init; }
}
