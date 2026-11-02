using Framework.Contracts;
using Framework.Domain;
using Framework.EarlyPay;
using SGStudios.Money;

namespace Framework.Profiles;

public interface IProfile<TTier> : IPaytableSelector<TTier> where TTier : Tier
{
    void Validate(Request<Command> request);

    /// <summary>
    /// The real-world currency this profile was designed for.
    /// </summary>
    Currency Currency { get; }

    int MinTickets { get; }

    int MaxTickets { get; }
}
