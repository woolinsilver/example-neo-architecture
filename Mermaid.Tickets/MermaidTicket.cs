using Framework.Tickets;

namespace Mermaid.Tickets;

// This will be serialized as JSON for the frontend.

public class MermaidTicket : Ticket
{
    public required MermaidSymbol[] Symbols { get; init; }
    public required bool Fish { get; init; }
}
