using System.Diagnostics;
using SGStudios.Primitives;

namespace Framework.Domain;

public class Paytable<TTier> where TTier : Tier
{
    public required decimal Stake { get; init; }
    public required IReadOnlyList<TTier> Tiers { get; init; }

    public ulong TotalWeight => Tiers.Sum(row => row.Weight);

    public decimal RTP => CalculateRTP();

    public TTier SelectTier(uint random)
    {
        if (random >= TotalWeight)
            throw new ArgumentOutOfRangeException(nameof(random), random, $"Random value must be less than TotalWeight ({TotalWeight}).");

        var accumulated = 0UL;
        foreach (var row in Tiers)
        {
            accumulated += row.Weight;
            if (random < accumulated)
                return row;
        }

        throw new UnreachableException();
    }

    /// <returns>between 0 - 1</returns>
    private decimal CalculateRTP()
    {
        decimal rtp = 0m;
        foreach (var tier in Tiers)
        {
            var multiplier = tier.Prize / Stake;
            var probability = (decimal)tier.Weight / TotalWeight;
            var contribution = multiplier * probability;
            rtp += contribution;
        }

        return rtp;
    }
}
