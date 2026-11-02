using Framework.Module;
using Microsoft.Extensions.DependencyInjection;

namespace Mermaid.Tickets;

public class MermaidTicketsModule : IModule
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTicketFactory<MermaidTicketFactory>();
    }
}
