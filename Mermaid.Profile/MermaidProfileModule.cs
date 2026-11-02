using Framework.Module;
using Mermaid.Tickets;
using Microsoft.Extensions.DependencyInjection;

namespace Mermaid.Profile;

public class MermaidProfileModule : IModule
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddProfile<MermaidProfile>();
        services.AddSingleton<IMermaidFeatures>(sp => sp.GetRequiredService<MermaidProfile>());
    }
}
