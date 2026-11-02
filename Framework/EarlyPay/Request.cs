using SGStudios.Money;

namespace Framework.EarlyPay;

/// <summary>
/// Simplified StartGameRequest from the Game Engine which does not involve database state.
/// </summary>
public record Request<TCommand>
{
    public required CurrencyAmount TotalStake { get; init; }
    public required TCommand Command { get; init; }
    public required DateTimeOffset Time { get; init; }
}
