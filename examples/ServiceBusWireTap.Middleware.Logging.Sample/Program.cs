using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceBusWireTap.Middleware.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(worker =>
    {
        // Register the ServiceBusWireTap middleware for logging/interception
        worker.UseMiddleware<ServiceBusWireTapMiddleware>();
    })
    .ConfigureServices(services =>
    {        
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        services.AddSingleton<ServiceBusWireTapOptions>();
    })    
    .Build();

await host.RunAsync();
