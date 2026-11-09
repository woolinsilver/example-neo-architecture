using Framework.Profiles;
using Mermaid.Tickets;
using SGStudios.Money;

namespace Mermaid.Profile;

// Here is our example profile; in reality this would be for a specific customer.
// This example has only a single paytable, therefore only a single permitted stake (per ticket).

public class MermaidProfile() : SimpleProfile<MermaidTier>([MermaidPaytable.Instance]), IMermaidProfile
{
    public override Currency Currency => Currency.GBP;

    public override int MaxTickets => 5;

    public bool Fish => true;
}
