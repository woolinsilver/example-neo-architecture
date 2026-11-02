using Framework.Tickets;

namespace Mermaid.Tickets;

public class MermaidTicket : Ticket
{
    public required MermaidSymbol[] Symbols { get; init; }
    public required bool Fish { get; init; }
}
