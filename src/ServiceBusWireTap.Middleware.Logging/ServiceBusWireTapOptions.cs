using Microsoft.Extensions.Logging;

namespace ServiceBusWireTap.Middleware.Logging;

/// <summary>
/// Configuration options for the ServiceBus WireTap middleware.
/// </summary>
public class ServiceBusWireTapOptions
{
    /// <summary>
    /// The log level to use for wire-tap logging. Default is Information.
    /// </summary>
    public LogLevel LogLevel { get; set; } = LogLevel.Information;

    /// <summary>
    /// Gets or sets whether to include the message body in the log entry.
    /// Default is true.
    /// </summary>
    public bool IncludeMessageBody { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum size of the message body to log in bytes.
    /// If null, the entire body is logged. If set, larger bodies are truncated.
    /// </summary>
    public int? MaxBodySizeToLog { get; set; }

    /// <summary>
    /// Gets or sets a custom action to execute when a ServiceBus message is intercepted.
    /// This allows for custom logging, metrics collection, or other processing.
    /// </summary>
    public Func<ServiceBusMessageLogEntry, Task>? CustomLogAction { get; set; }

    /// <summary>
    /// Gets or sets whether to log application properties.
    /// Default is true.
    /// </summary>
    public bool IncludeApplicationProperties { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to log user properties.
    /// Default is true.
    /// </summary>
    public bool IncludeUserProperties { get; set; } = true;
} 