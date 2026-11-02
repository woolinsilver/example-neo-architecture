using Framework.Contracts;
using Framework.Domain;
using Framework.EarlyPay;
using Framework.Module;
using Framework.Service;
using Framework.Tickets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SGStudios.LocalPrize.GameLogic;
using SGStudios.Progressive.GameLogic;
using SGStudios.Random.GameLogic;

namespace Framework;

public abstract class EntryPoint<TTier, TTicket> : EarlyPayGameLogic<Command, Result<TTicket>>, IModule
    where TTier : Tier
    where TTicket : Ticket
{
    // dependency injection container
    private ServiceProvider? _services;

    // allow subclass to wire things into the dependency injection container
    public virtual void ConfigureServices(IServiceCollection services) { }

    public sealed override Task InitializeAsync(
        IConfiguration configuration,
        IPluginLoader pluginLoader, // not needed
        ILoggerFactory loggerFactory)
    {
        var services = new ServiceCollection();

        // framework services
        services.AddSingleton(typeof(GameLogicService<TTier, TTicket>));

        // provided by host
        services.AddSingleton(configuration);
        services.AddSingleton(loggerFactory);
        services.AddLogging();

        // provided by subclass
        ConfigureServices(services);

        // build the container
        _services = services.BuildServiceProvider();

        // force creation of services now
        _ = _services.GetRequiredService<GameLogicService<TTier, TTicket>>();

        return Task.CompletedTask;
    }

    protected override async Task<Response<Result<TTicket>>> ExecuteAsync(Request<Command> request, IGameLogicRandom random, IGameLogicProgressive progressive, CancellationToken ct)
    {
        var service = (GameLogicService<TTier, TTicket>)_services!.GetRequiredService(typeof(GameLogicService<TTier, TTicket>));
        return await service.ExecuteAsync(request, random, progressive, ct);
    }
}
