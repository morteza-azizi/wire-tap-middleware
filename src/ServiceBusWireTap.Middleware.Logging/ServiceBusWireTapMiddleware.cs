using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace ServiceBusWireTap.Middleware.Logging;

public class ServiceBusWireTapMiddleware(ILogger<ServiceBusWireTapMiddleware> logger, ServiceBusWireTapOptions options) : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var functionName = context.FunctionDefinition.Name;
        try
        {
            if (!IsServiceBusTriggered(context))
            {
                logger.LogInformation("Function {FunctionName} does not use a Service Bus trigger. Skipping WireTap middleware.", functionName);
            }
            else
            {
                logger.LogInformation("WireTap middleware invoked for Service Bus function: {FunctionName}", functionName);

                var serviceBusMessage = await ExtractServiceBusMessage(context);
                if (serviceBusMessage != null)
                {
                    await LogServiceBusMessage(context, serviceBusMessage);
                }
                else
                {
                    logger.LogWarning("Warning: Could not retrieve a Service Bus message for function '{FunctionName}'. Skipping logging.", functionName);
                }
            }

            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception thrown in Service Bus WireTap middleware during execution of function '{FunctionName}'.", functionName);
            throw;
        }
    }

    private static bool IsServiceBusTriggered(FunctionContext context)
    {
        return context.FunctionDefinition.InputBindings.Values
            .Any(binding => binding.Type.Equals("serviceBusTrigger", StringComparison.OrdinalIgnoreCase));
    }

    private static async Task<ServiceBusReceivedMessage?> ExtractServiceBusMessage(FunctionContext context)
    {
        var functionName = context.FunctionDefinition.Name;
        try
        {
            var inputBinding = context.FunctionDefinition.InputBindings
                .FirstOrDefault(b => b.Value.Type.Equals("serviceBusTrigger", StringComparison.OrdinalIgnoreCase)).Value;

            if (inputBinding == null)
            {
                context.GetLogger<ServiceBusWireTapMiddleware>().LogWarning("No ServiceBusTrigger input binding found for function {FunctionName}", functionName);
                return null;
            }

            var bindingResult = await context.BindInputAsync<ServiceBusReceivedMessage>(inputBinding);

            if (bindingResult?.Value == null)
            {
                context.GetLogger<ServiceBusWireTapMiddleware>().LogWarning("Failed to bind input to ServiceBusReceivedMessage for function {FunctionName}", functionName);
                return null;
            }

            return bindingResult.Value;
        }
        catch (Exception ex)
        {
            context.GetLogger<ServiceBusWireTapMiddleware>().LogError(ex, "Exception extracting ServiceBusReceivedMessage in function {FunctionName}", functionName);
            return null;
        }
    }

    private static async Task LogServiceBusMessage(FunctionContext context, ServiceBusReceivedMessage message)
    {
        var functionName = context.FunctionDefinition.Name;
        var logger = context.GetLogger<ServiceBusWireTapMiddleware>();
        var options = context.InstanceServices.GetService(typeof(ServiceBusWireTapOptions)) as ServiceBusWireTapOptions;

        var logEntry = new ServiceBusMessageLogEntry
        {
            Timestamp = DateTimeOffset.UtcNow,
            FunctionName = functionName,
            InvocationId = context.InvocationId,
            MessageId = message.MessageId,
            CorrelationId = message.CorrelationId,
            Subject = message.Subject,
            To = message.To,
            ReplyTo = message.ReplyTo,
            ReplyToSessionId = message.ReplyToSessionId,
            SessionId = message.SessionId,
            ContentType = message.ContentType,
            DeliveryCount = message.DeliveryCount,
            EnqueuedTime = message.EnqueuedTime,
            ScheduledEnqueueTime = message.ScheduledEnqueueTime,
            ExpiresAt = message.ExpiresAt,
            LockToken = message.LockToken,
            SequenceNumber = message.SequenceNumber,
            PartitionKey = message.PartitionKey,
            DeadLetterSource = message.DeadLetterSource,
            EnqueuedSequenceNumber = message.EnqueuedSequenceNumber,
            LockedUntil = message.LockedUntil,
            State = message.State.ToString(),
            ApplicationProperties = message.ApplicationProperties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString() ?? "null"),
            UserProperties = message.ApplicationProperties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString() ?? "null")
        };

        if (options != null && options.IncludeMessageBody)
        {
            logEntry.MessageBody = message.Body.ToString();
        }

        var jsonLog = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        logger.LogInformation("Successfully intercepted and logged Service Bus message: {JsonLog}", jsonLog);

        if (options != null && options.CustomLogAction != null)
        {
            try
            {
                await options.CustomLogAction(logEntry);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while executing the custom log action for intercepted Service Bus message with ID '{MessageId}'.", message.MessageId);
            }
        }
    }
}