using Framework.Domain;
using Mermaid.Tickets;

namespace Mermaid.Profile;

// Note:
//
// Tier.Identifier would go to the frontend via Ticket.Tier,
// it's part of the contract with the frontend.
//
// MermaidTier.Outcome will go to MermaidTicketFactory,
// it's purely an internal detail of the backend.

public static class MermaidPaytable
{
    public static readonly Paytable<MermaidTier> Instance = new()
    {
        Stake = 1.00m,
        Tiers =
        [
            new MermaidTier
            {
                Identifier = nameof(MermaidOutcome.Lose),
                Weight = 70,
                Prize = 0.00m,
                Outcome = MermaidOutcome.Lose
            },
            new MermaidTier
            {
                Identifier = nameof(MermaidOutcome.ThreeTridents),
                Weight = 25,
                Prize = 2.00m,
                Outcome = MermaidOutcome.ThreeTridents
            },
            new MermaidTier
            {
                Identifier = nameof(MermaidOutcome.ThreePearls),
                Weight = 5,
                Prize = 8.00m,
                Outcome = MermaidOutcome.ThreePearls
            }
        ]
    };
}
