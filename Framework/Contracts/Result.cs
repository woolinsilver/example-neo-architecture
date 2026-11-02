using Framework.Tickets;

namespace Framework.Contracts;

public class Result<TTicket> where TTicket : Ticket
{
    public required IEnumerable<TTicket> Tickets { get; init; }
}
