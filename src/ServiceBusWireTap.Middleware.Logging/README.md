# 📦 Service Bus WireTap Middleware for .NET Isolated Azure Functions

A reusable middleware for Azure Functions (.NET 8+ Isolated Worker), enabling wire‑tap-style logging of complete ServiceBusReceivedMessage—capturing body, metadata, and application properties—regardless of typed function signatures.

**Part of the [WireTapMiddleware](https://github.com/morteza-azizi/WireTapMiddleware) collection for  messaging broker interception.**

## 🔍 Key Features

- **Works seamlessly with typed bindings** (e.g., POCOs), yet taps the full ServiceBusReceivedMessage for deep visibility
- **Built on IFunctionsWorkerMiddleware**, intercepting messages in a central place, no per-function changes needed
- **Logs MessageId, CorrelationId, full body text, and all ApplicationProperties**
- **Integrates with built-in logging** (Application Insights/ILogger)
- **Configurable logging options** for security and performance
- **Custom log actions** for integration with external systems

## 🚀 Quick Start

### 1. Install the Package

```bash
dotnet add package ServiceBusWireTap.Middleware.Logging
```

### 2. Register the Middleware

In your `Program.cs`:

```csharp
using Microsoft.Extensions.Hosting;
using ServiceBusWireTap.Middleware.Logging;

var host = new HostBuilder()
    .UseServiceBusWireTap() // Add this line
    .ConfigureFunctionsWorkerDefaults()
    .Build();

host.Run();
```

### 3. Your Functions Work Unchanged

```csharp
[Function("ProcessOrder")]
public async Task ProcessOrder([ServiceBusTrigger("orders", Connection = "ServiceBus")] Order order)
{
    // Your business logic here - unchanged!
    // The middleware automatically logs the complete ServiceBusReceivedMessage
}
```

## ⚙️ Configuration Options

### Basic Configuration

```csharp
var host = new HostBuilder()
    .UseServiceBusWireTap(options =>
    {
        options.LogLevel = LogLevel.Information;
        options.IncludeMessageBody = true;
        options.MaxBodySizeToLog = 10 * 1024; // 10KB
    })
    .ConfigureFunctionsWorkerDefaults()
    .Build();
```

### Advanced Configuration with Custom Actions

```csharp
var host = new HostBuilder()
    .UseServiceBusWireTap(options =>
    {
        options.LogLevel = LogLevel.Debug;
        options.IncludeMessageBody = false; // Don't log body for security
        options.CustomLogAction = async (logEntry) =>
        {
            // Send to custom telemetry system
            await CustomTelemetryClient.SendAsync(logEntry);
        };
    })
    .ConfigureFunctionsWorkerDefaults()
    .Build();
```

## 🔧 Configuration Options Reference

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `LogLevel` | `LogLevel` | `Information` | The log level for wire-tap entries |
| `IncludeMessageBody` | `bool` | `true` | Whether to include message body in logs |
| `MaxBodySizeToLog` | `int?` | `10240` | Maximum body size to log (bytes) |
| `CustomLogAction` | `Func<ServiceBusMessageLogEntry, Task>?` | `null` | Custom action for log entries |

## 🛡️ Security Considerations

For sensitive data, consider:

```csharp
.UseServiceBusWireTap(options =>
{
    options.IncludeMessageBody = false; // Exclude body for sensitive data
    options.MaxBodySizeToLog = 1024;    // Limit body size
})
```

## 🔍 Example Function Types Supported

### POCO Binding
```csharp
[Function("ProcessOrder")]
public async Task ProcessOrder([ServiceBusTrigger("orders")] Order order)
{
    // Middleware logs the complete ServiceBusReceivedMessage
    // Your function gets the typed Order object
}
```

### String Binding
```csharp
[Function("ProcessMessage")]
public async Task ProcessMessage([ServiceBusTrigger("messages")] string message)
{
    // Middleware logs full message details
    // Your function gets the string body
}
```

### ServiceBusReceivedMessage Binding
```csharp
[Function("ProcessRawMessage")]
public async Task ProcessRawMessage([ServiceBusTrigger("raw")] ServiceBusReceivedMessage message)
{
    // Middleware logs the message
    // Your function gets the raw message object
}
```

## 🔄 Integration with Application Insights

The middleware integrates seamlessly with Application Insights:

```csharp
// In your Function App configuration
builder.Services.AddApplicationInsightsTelemetryWorkerService();

// Wire-tap logs will appear in Application Insights with custom properties
// Query example:
// traces | where message contains "ServiceBus Message Intercepted"
```

## 🎯 Use Cases

- **Message Auditing**: Track all ServiceBus messages for compliance
- **Debugging**: Deep visibility into message flow without code changes  
- **Monitoring**: Real-time insights into message processing
- **Analytics**: Custom telemetry integration for business intelligence

## 🔮 Future Brokers

This is part of the WireTapMiddleware collection. Future implementations will include:
- RabbitMQ WireTap Middleware
- Apache Kafka WireTap Middleware  
- Amazon SQS WireTap Middleware

## 📝 License

This project is licensed under the MIT License.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request. 