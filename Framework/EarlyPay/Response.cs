using SGStudios.Money;

namespace Framework.EarlyPay;

/// <summary>
/// Simplified StartGameResponse to the Game Engine which does not involve database state.
/// </summary>
public record Response<TResult>
{
    public required CurrencyAmount TotalPrize { get; init; }
    public required TResult Result { get; init; }
}
