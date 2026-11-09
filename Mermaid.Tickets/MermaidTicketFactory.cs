using System.Diagnostics;
using Framework.Tickets;
using SGStudios.Random.Generator;
using static Mermaid.Tickets.MermaidSymbol;

namespace Mermaid.Tickets;

// This is wired into DI as a singleton, so it's long-lived.
//
// We can ask DI for other things we need, for example
// - something like IMermaidFeatures provided by the profile, or
// - some other services from Tickets project.
//
// We can calculate stuff (like combos) during initialization and hold it in memory.
// We could read "offline tickets" from disk and hold those in memory.

public class MermaidTicketFactory(IMermaidFeatures features) : ITicketFactory<MermaidTier, MermaidTicket>
{
    private static readonly MermaidSymbol[][] Losers = GenerateLosingSymbols();

    public MermaidTicket CreateTicket(MermaidTier tier, IDeterministicGenerator generator)
    {
        MermaidSymbol[] symbols = tier.Outcome switch
        {
            MermaidOutcome.Lose => generator.Select(Losers),
            MermaidOutcome.ThreeTridents => [TRIDENT, TRIDENT, TRIDENT],
            MermaidOutcome.ThreePearls => [PEARL, PEARL, PEARL],
            _ => throw new UnreachableException()
        };

        return new MermaidTicket
        {
            Tier = tier.Identifier,
            Seed = generator.Seed,
            Symbols = symbols,
            Prize = tier.Prize,
            Fish = features.Fish,
        };
    }

    private static MermaidSymbol[][] GenerateLosingSymbols()
    {
        List<MermaidSymbol[]> losers = [];

        foreach (MermaidSymbol s1 in EverySymbol())
            foreach (MermaidSymbol s2 in EverySymbol())
                foreach (MermaidSymbol s3 in EverySymbol())
                {
                    if (AllAre(TRIDENT, s1, s2, s3)) continue;
                    if (AllAre(PEARL, s1, s2, s3)) continue;

                    losers.Add([s1, s2, s3]);
                }

        return losers.ToArray();
    }

    private static bool AllAre(MermaidSymbol expected, params MermaidSymbol[] symbols)
    {
        return symbols.All(symbol => symbol == expected);
    }

    private static MermaidSymbol[] EverySymbol()
    {
        return Enum.GetValues<MermaidSymbol>();
    }
}
