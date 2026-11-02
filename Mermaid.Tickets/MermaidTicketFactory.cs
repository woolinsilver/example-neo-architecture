using System.Diagnostics;
using Framework.Tickets;
using SGStudios.Random.Generator;
using static Mermaid.Tickets.MermaidSymbol;

namespace Mermaid.Tickets;

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
