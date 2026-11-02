using Framework.Domain;
using SGStudios.Random.Generator;

namespace Framework.Tickets;

public interface ITicketFactory<in TTier, out TTicket>
    where TTier : Tier
    where TTicket : Ticket
{
    TTicket CreateTicket(TTier tier, IDeterministicGenerator generator);
}
