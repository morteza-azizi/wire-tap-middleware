using Microsoft.Azure.Functions.Worker;
using System.Threading.Tasks;
using System;
using Azure.Messaging.ServiceBus;

namespace ExampleApp;

public class OrderProcessingFunction
{
    [Function("OrderProcessingFunction")]
    public async Task Run(
        [ServiceBusTrigger("%QueueName%", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message)
    {
        // The message will be intercepted and logged by the ServiceBusWireTap middleware
        Console.WriteLine($"OrderProcessingFunction triggered! MessageId: {message.MessageId}");
    }
}