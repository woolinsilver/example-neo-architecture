using Framework;
using Mermaid.Tickets;
using Microsoft.Extensions.DependencyInjection;

namespace Mermaid.Profile;

/// <summary>
/// This is the concrete class which the Game Engine constructs reflectively.
/// </summary>
public sealed class MermaidEntryPoint : EntryPoint<MermaidTier, MermaidTicket>
{
    public override void ConfigureServices(IServiceCollection services)
    {
        new MermaidTicketsModule().ConfigureServices(services);
        new MermaidProfileModule().ConfigureServices(services);
    }
}
