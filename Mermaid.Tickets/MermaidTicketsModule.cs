using Framework.Module;
using Microsoft.Extensions.DependencyInjection;

namespace Mermaid.Tickets;

// Here we can wire anything into the Dependency Injection container.
// At a minimum we need the ITicketFactory.
// We could add other singletons for the ITicketFactory to collaborate with.

public class MermaidTicketsModule : IModule
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTicketFactory<MermaidTicketFactory>();
    }
}
