using Microsoft.Extensions.DependencyInjection;

namespace Framework.Module;

/// <summary>
/// Can add services to the dependency injection container.
/// </summary>
public interface IModule
{
    void ConfigureServices(IServiceCollection services);
}
