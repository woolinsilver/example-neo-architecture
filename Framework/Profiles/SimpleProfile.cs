using Framework.Domain;

namespace Framework.Profiles;

/// <summary>
/// <para>
/// This is a profile which maps stakes (per ticket) to paytables.
/// </para>
/// <para>
/// You could have a more complicated profile which implemented happy hour.
/// </para>
/// </summary>
public abstract class SimpleProfile<TTier> : ProfileBase<TTier> where TTier : Tier
{
    private readonly IReadOnlyDictionary<decimal, Paytable<TTier>> _paytables;

    protected SimpleProfile(IEnumerable<Paytable<TTier>> paytables)
    {
        var list = paytables.ToList();
        if (list.Count == 0)
            throw new ArgumentException("At least one paytable must be provided.", nameof(paytables));

        _paytables = list.ToDictionary(p => p.Stake);
    }

    protected override IEnumerable<decimal> PermittedStakes => _paytables.Keys;

    public override Paytable<TTier> SelectPaytable(decimal stake, DateTimeOffset time)
    {
        if (!_paytables.TryGetValue(stake, out var paytable))
            throw new ArgumentException($"No paytable found for stake {stake}", nameof(stake));

        return paytable;
    }
}
