using Framework.Module;
using Mermaid.Tickets;
using Microsoft.Extensions.DependencyInjection;

namespace Mermaid.Profile;

// Here we can wire anything into the Dependency Injection container.
// At a minimum we need the IProfile.
// The developer of Mermaid.Tickets has also asked us to provide IMermaidFeatures.

public class MermaidProfileModule : IModule
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddProfile<MermaidProfile>();
        services.AddSingleton<IMermaidFeatures>(sp => sp.GetRequiredService<MermaidProfile>());
    }
}
