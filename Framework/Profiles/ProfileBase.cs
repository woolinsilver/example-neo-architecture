using Framework.Contracts;
using Framework.Domain;
using Framework.EarlyPay;
using SGStudios.LocalPrize.GameLogic;
using SGStudios.Money;

namespace Framework.Profiles;

/// <summary>
/// An <see cref="IProfile{TTier}"/> which can validate a <see cref="Request{Command}"/>
/// </summary>
public abstract class ProfileBase<TTier> : IProfile<TTier> where TTier : Tier
{
    public void Validate(Request<Command> request)
    {
        ValidateCurrency(request);
        ValidateStakePerTicket(request);
        ValidateNumberOfTickets(request);
        ValidateTotalStake(request);
    }

    protected abstract IEnumerable<decimal> PermittedStakes { get; }

    /// <inheritdoc />
    public abstract Paytable<TTier> SelectPaytable(decimal stake, DateTimeOffset time);

    /// <inheritdoc />
    public abstract Currency Currency { get; }

    /// <inheritdoc />
    public virtual int MinTickets => 1;

    /// <inheritdoc />
    public virtual int MaxTickets => 1;

    private void ValidateCurrency(Request<Command> request)
    {
        var currency = request.TotalStake.Currency;
        Currency[] supported = [Currency, Currency.FUN];
        if (!supported.Contains(currency))
            throw CommandRejectedException.UnsupportedCurrency(currency);
    }

    private void ValidateStakePerTicket(Request<Command> request)
    {
        if (!PermittedStakes.Contains(request.Command.StakePerTicket))
            throw CommandRejectedException.InvalidCommand($"Unsupported stake per ticket {request.Command.StakePerTicket}");
    }

    private void ValidateNumberOfTickets(Request<Command> request)
    {
        if (request.Command.NumberOfTickets < MinTickets || request.Command.NumberOfTickets > MaxTickets)
            throw CommandRejectedException.InvalidCommand($"Number of tickets must be between {MinTickets} and {MaxTickets}");
    }

    private static void ValidateTotalStake(Request<Command> request)
    {
        var currency = request.TotalStake.Currency;
        var expected = currency.Amount(request.Command.StakePerTicket * request.Command.NumberOfTickets);
        if (!request.TotalStake.Equals(expected))
            throw CommandRejectedException.StakeMismatch($"Command requires stake of {expected}");
    }
}
