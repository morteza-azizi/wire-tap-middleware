# 🚀 WireTapMiddleware: Messaging Broker Interception

## Why This Project Exists (A Story)

It started with a simple need: **"How can I see every message that flows through my Azure Service Bus-triggered Functions, no matter how the function is written?"**

The Wire Tap pattern, as described in the seminal work "Enterprise Integration Patterns" by Gregor Hohpe and Bobby Woolf, provides an elegant solution for intercepting and duplicating messages as they flow through a system. I've written about this pattern in detail in my article [Understanding the Wire Tap Pattern in Messaging Systems](https://www.mortezaazizi.com/posts/understanding-wire-tap-pattern-in-messaging-systems/).

I wanted a plug-and-play solution, something that would let me intercept, log, and analyze every Service Bus message, regardless of whether the function parameter was a POCO, a string, or the raw message. I wanted it to be easy, robust, and production-ready.

But the journey wasn't easy. I faced SDK quirks, mocking nightmares, and the ever-changing landscape of Azure Functions. This middleware is the result of that journey, a reusable, extensible, and modern solution for anyone who wants deep visibility into their message broker flows.

**Now I'm expanding beyond Service Bus to support multiple messaging brokers!**

---

## 🏗️ Solution Structure

```
WireTapMiddleware/
├── src/
│   └── ServiceBusWireTap.Middleware.Logging/          # Service Bus implementation
│       ├── ServiceBusWireTap.Middleware.Logging.csproj
│       ├── ServiceBusWireTapMiddleware.cs
│       ├── ServiceBusWireTapOptions.cs
│       ├── ServiceBusMessageLogEntry.cs
│       ├── ServiceCollectionExtensions.cs
│       ├── HostBuilderExtensions.cs
│       └── README.md
├── examples/
│   └── ServiceBusWireTap.Middleware.Logging.Sample/   # Example Azure Functions app
│       ├── ServiceBusWireTap.Middleware.Logging.Sample.csproj
│       ├── Program.cs
│       ├── OrderProcessingFunction.cs
│       ├── host.json
│       └── local.settings.json
├── tests/
│   └── WireTap.ServiceBus.Middleware.Tests/           # Unit tests
└── WireTapMiddleware.sln                              # Solution file
```

---

## 🚀 Quick Start

### Building the Solution

```bash
# Build the entire solution
dotnet build

# Build just the Service Bus middleware library
dotnet build src/ServiceBusWireTap.Middleware.Logging/ServiceBusWireTap.Middleware.Logging.csproj

# Build just the sample app
dotnet build examples/ServiceBusWireTap.Middleware.Logging.Sample/ServiceBusWireTap.Middleware.Logging.Sample.csproj
```

### Using the Service Bus Middleware

1. **Reference the middleware library** in your Azure Functions project
2. **Register the middleware** in your `Program.cs`:

```csharp
using ServiceBusWireTap.Middleware.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(worker =>
    {
        worker.UseMiddleware<ServiceBusWireTapMiddleware>();
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<ServiceBusWireTapOptions>();
    })
    .Build();

host.Run();
```

3. **Your functions work unchanged**—the middleware automatically logs all Service Bus messages!

### Running the Sample App

1. **Configure Service Bus connection** in `examples/ServiceBusWireTap.Middleware.Logging.Sample/local.settings.json`
2. **Run the sample app**:

```bash
cd examples/ServiceBusWireTap.Middleware.Logging.Sample
dotnet run
```

---

## 🔮 Future Plans

This repository is designed to support multiple messaging brokers:

- ✅ **Azure Service Bus** - Currently implemented
- 🔄 **RabbitMQ** - Coming soon
- 🔄 **Apache Kafka** - Coming soon
- 🔄 **Amazon SQS** - Coming soon

Each implementation will follow the same patterns and provide consistent logging capabilities.

### 🚀 Upcoming Features

- **Large Message Chunking**: Break down large message bodies into smaller chunks to work within logging framework limitations (Application Insights, etc.)
- **Selective property logging** to reduce noise and improve performance

---

## 🧗 Key Implementation Decisions

- **Mocking Azure Functions internals:** The SDK makes it hard to unit test Service Bus binding logic. I focused on what could be tested and relied on integration tests for the rest.
- **SDK changes:** Types like `IFunctionContextBindingFeature` and `Features` are not always public or available, so I adapted the testing strategy accordingly.
- **Binary vs. text payloads:** Azure Service Bus treats all payloads as binary. I ensured the logging was robust for both text and binary messages.
- **Generic architecture:** I designed the structure to support multiple messaging brokers while maintaining consistency.

---

## 📚 Documentation

- **Service Bus Middleware:** See `src/ServiceBusWireTap.Middleware.Logging/README.md` for detailed usage and configuration
- **Sample App:** See `examples/ServiceBusWireTap.Middleware.Logging.Sample/OrderProcessingFunction.cs` for usage examples

---

## 🤝 Contributing

1. Follow the existing folder structure
2. Add examples for new features
3. Update documentation
4. Test with the sample app
5. When adding new broker support, follow the established patterns

---

## 📝 License

This project is licensed under the MIT License. 