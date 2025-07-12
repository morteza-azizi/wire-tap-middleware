namespace ServiceBusWireTap.Middleware.Logging;

/// <summary>
/// Represents a structured log entry for a ServiceBus message.
/// </summary>
public class ServiceBusMessageLogEntry
{
    /// <summary>
    /// Timestamp when the message was intercepted by the middleware.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Name of the Azure Function being executed.
    /// </summary>
    public string FunctionName { get; set; } = string.Empty;

    /// <summary>
    /// Unique invocation ID for the function execution.
    /// </summary>
    public string InvocationId { get; set; } = string.Empty;

    /// <summary>
    /// ServiceBus message ID.
    /// </summary>
    public string? MessageId { get; set; }

    /// <summary>
    /// ServiceBus message correlation ID.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Subject of the message.
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// The "to" address of the message.
    /// </summary>
    public string? To { get; set; }

    /// <summary>
    /// The reply-to address for the message.
    /// </summary>
    public string? ReplyTo { get; set; }

    /// <summary>
    /// The reply-to session ID for the message.
    /// </summary>
    public string? ReplyToSessionId { get; set; }

    /// <summary>
    /// Session ID of the message.
    /// </summary>
    public string? SessionId { get; set; }

    /// <summary>
    /// Content type of the message.
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// Number of times this message has been delivered.
    /// </summary>
    public int DeliveryCount { get; set; }

    /// <summary>
    /// The time when the message was enqueued.
    /// </summary>
    public DateTimeOffset EnqueuedTime { get; set; }

    /// <summary>
    /// The time when the message is scheduled to be enqueued.
    /// </summary>
    public DateTimeOffset? ScheduledEnqueueTime { get; set; }

    /// <summary>
    /// The time when the message expires.
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>
    /// The lock token for the message.
    /// </summary>
    public string? LockToken { get; set; }

    /// <summary>
    /// The sequence number of the message.
    /// </summary>
    public long SequenceNumber { get; set; }

    /// <summary>
    /// The partition key for the message.
    /// </summary>
    public string? PartitionKey { get; set; }

    /// <summary>
    /// The dead letter source.
    /// </summary>
    public string? DeadLetterSource { get; set; }

    /// <summary>
    /// The enqueued sequence number.
    /// </summary>
    public long? EnqueuedSequenceNumber { get; set; }

    /// <summary>
    /// The time until which the message is locked.
    /// </summary>
    public DateTimeOffset? LockedUntil { get; set; }

    /// <summary>
    /// The state of the message.
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Application properties of the message.
    /// </summary>
    public Dictionary<string, string> ApplicationProperties { get; set; } = new();

    /// <summary>
    /// User properties of the message (alias for ApplicationProperties).
    /// </summary>
    public Dictionary<string, string> UserProperties { get; set; } = new();

    /// <summary>
    /// The message body as a string.
    /// </summary>
    public string? MessageBody { get; set; }
} 