using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ServiceBusWireTap.Middleware.Logging;

/// <summary>
/// Extension methods for registering ServiceBus WireTap middleware with the Azure Functions host.
/// </summary>
public static class HostBuilderExtensions
{
    /// <summary>
    /// Adds ServiceBus WireTap middleware to the Azure Functions host.
    /// </summary>
    /// <param name="hostBuilder">The host builder.</param>
    /// <param name="configureOptions">Action to configure the middleware options.</param>
    /// <returns>The host builder for chaining.</returns>
    public static IHostBuilder AddServiceBusWireTap(
        this IHostBuilder hostBuilder,
        Action<ServiceBusWireTapOptions> configureOptions)
    {
        hostBuilder.ConfigureServices(services =>
        {
            services.AddServiceBusWireTap(configureOptions);
        });

        hostBuilder.ConfigureFunctionsWorkerDefaults(workerApplication =>
        {
            workerApplication.UseMiddleware<ServiceBusWireTapMiddleware>();
        });

        return hostBuilder;
    }

    /// <summary>
    /// Adds ServiceBus WireTap middleware to the Azure Functions host.
    /// </summary>
    /// <param name="hostBuilder">The host builder.</param>
    /// <param name="options">The middleware options.</param>
    /// <returns>The host builder for chaining.</returns>
    public static IHostBuilder AddServiceBusWireTap(
        this IHostBuilder hostBuilder,
        ServiceBusWireTapOptions options)
    {
        hostBuilder.ConfigureServices(services =>
        {
            services.AddServiceBusWireTap(options);
        });

        hostBuilder.ConfigureFunctionsWorkerDefaults(workerApplication =>
        {
            workerApplication.UseMiddleware<ServiceBusWireTapMiddleware>();
        });

        return hostBuilder;
    }
} 