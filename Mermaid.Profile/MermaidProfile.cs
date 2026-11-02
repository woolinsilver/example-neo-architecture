using Framework.Profiles;
using Mermaid.Tickets;
using SGStudios.Money;

namespace Mermaid.Profile;

public class MermaidProfile() : SimpleProfile<MermaidTier>([MermaidPaytable.Instance]), IMermaidProfile
{
    public override Currency Currency => Currency.GBP;

    public override int MaxTickets => 5;

    public bool Fish => true;
}
