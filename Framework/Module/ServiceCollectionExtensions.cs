using Framework.Profiles;
using Framework.Tickets;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Module;

public static class ServiceCollectionExtensions
{
    /// <typeparam name="TProfile">Must extend <see cref="IProfile{TTier}"/></typeparam>
    public static void AddProfile<TProfile>(this IServiceCollection services)
        where TProfile : class
    {
        // register the concrete implementation, for example MermaidProfile
        services.AddSingleton<TProfile>();

        // allow it to also be retrieved via IMermaidProfile, IProfile<MermaidTier>, etc.
        Type iProfile = GetIProfileInterface<TProfile>();
        foreach (var @interface in typeof(TProfile).GetInterfaces()
                     .Where(iProfile.IsAssignableFrom))
        {
            services.AddSingleton(@interface, sp => sp.GetRequiredService<TProfile>());
        }
    }

    /// <summary>
    /// Gets the <see cref="IProfile{TTier}"/> interface implemented by <typeparamref name="TProfile"/>.
    /// </summary>
    /// <typeparam name="TProfile">The type to inspect.</typeparam>
    /// <returns>The <see cref="IProfile{TTier}"/> interface type.</returns>
    /// <exception cref="ArgumentException">
    /// <typeparamref name="TProfile"/> does not implement <see cref="IProfile{TTier}"/>.
    /// </exception>
    private static Type GetIProfileInterface<TProfile>() where TProfile : class
    {
        var @interface = typeof(TProfile).GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IProfile<>));

        if (@interface == null)
            throw new ArgumentException($"Type {typeof(TProfile).Name} must implement IProfile<TTier>.");

        return @interface;
    }

    /// <typeparam name="TFactory">Must extend <see cref="ITicketFactory{TTier, TTicket}"/></typeparam>
    public static void AddTicketFactory<TFactory>(this IServiceCollection services)
        where TFactory : class
    {
        // register the concrete implementation, for example MermaidTicketFactory
        services.AddSingleton<TFactory>();

        // allow it to also be retrieved via ITicketFactory<MermaidTier, MermaidTicket>, etc.
        Type iTicketFactory = GetITicketFactoryInterface<TFactory>();
        foreach (var @interface in typeof(TFactory).GetInterfaces()
                     .Where(iTicketFactory.IsAssignableFrom))
        {
            services.AddSingleton(@interface, sp => sp.GetRequiredService<TFactory>());
        }
    }

    /// <summary>
    /// Gets the <see cref="ITicketFactory{TTier, TTicket}"/> interface implemented by
    /// <typeparamref name="TTicketFactory"/>.
    /// </summary>
    /// <typeparam name="TTicketFactory">The type to inspect.</typeparam>
    /// <returns>The <see cref="ITicketFactory{TTier, TTicket}"/> interface type.</returns>
    /// <exception cref="ArgumentException">
    /// <typeparamref name="TTicketFactory"/> does not implement
    /// <see cref="ITicketFactory{TTier, TTicket}"/>.
    /// </exception>
    private static Type GetITicketFactoryInterface<TTicketFactory>() where TTicketFactory : class
    {
        var @interface = typeof(TTicketFactory).GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ITicketFactory<,>));

        if (@interface == null)
            throw new ArgumentException($"Type {typeof(TTicketFactory).Name} must implement ITicketFactory<TTier, TTicket>.");

        return @interface;
    }
}
