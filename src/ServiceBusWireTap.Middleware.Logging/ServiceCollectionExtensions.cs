using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ServiceBusWireTap.Middleware.Logging;

/// <summary>
/// Extension methods for registering ServiceBus wire-tap middleware.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the ServiceBus wire-tap middleware to the service collection with default options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddServiceBusWireTap(this IServiceCollection services)
    {
        return services.AddServiceBusWireTap(new ServiceBusWireTapOptions());
    }

    /// <summary>
    /// Adds the ServiceBus wire-tap middleware to the service collection with custom options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="options">The wire-tap options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddServiceBusWireTap(this IServiceCollection services, ServiceBusWireTapOptions options)
    {
        services.AddSingleton(Options.Create(options));
        services.AddSingleton<ServiceBusWireTapMiddleware>();
        return services;
    }

    /// <summary>
    /// Adds the ServiceBus wire-tap middleware to the service collection with a configuration action.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Action to configure the wire-tap options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddServiceBusWireTap(this IServiceCollection services, Action<ServiceBusWireTapOptions> configureOptions)
    {
        services.Configure(configureOptions);
        return services;
    }
} 