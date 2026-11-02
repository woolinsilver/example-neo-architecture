namespace Framework.Contracts;

public class Command
{
    public required decimal StakePerTicket { get; set; }
    public required int NumberOfTickets { get; set; }
}
