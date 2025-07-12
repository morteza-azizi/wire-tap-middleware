using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using ServiceBusWireTap.Middleware.Logging;
using System.Collections.Immutable;
using Microsoft.Azure.Functions.Worker.Definition;
using System.Threading;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ExampleApp;
using Azure.Messaging.ServiceBus;

public class ServiceBusWireTapMiddlewareTests
{
    [Fact]
    public async Task Invoke_WhenNotServiceBusTrigger_LogsAndCallsNext()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ServiceBusWireTapMiddleware>>();
        var options = new ServiceBusWireTapOptions();
        var middleware = new ServiceBusWireTapMiddleware(loggerMock.Object, options);

        var contextMock = new Mock<FunctionContext>();
        contextMock.Setup(c => c.FunctionDefinition)
            .Returns(new MockFunctionDefinition(hasServiceBusTrigger: false));

        bool nextCalled = false;
        Task Next(FunctionContext ctx) { nextCalled = true; return Task.CompletedTask; }

        // Act
        await middleware.Invoke(contextMock.Object, Next);

        // Assert
        Assert.True(nextCalled);
        loggerMock.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("does not use a Service Bus trigger")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Invoke_WhenServiceBusTrigger_LogsAndCallsNext()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ServiceBusWireTapMiddleware>>();
        var options = new ServiceBusWireTapOptions();
        var middleware = new ServiceBusWireTapMiddleware(loggerMock.Object, options);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(ServiceBusWireTapOptions)))
            .Returns(options);
        serviceProviderMock.Setup(sp => sp.GetService(typeof(ILogger<ServiceBusWireTapMiddleware>)))
            .Returns(loggerMock.Object);

        var contextMock = new Mock<FunctionContext>();
        contextMock.Setup(c => c.FunctionDefinition)
            .Returns(new MockFunctionDefinition(hasServiceBusTrigger: true));
        contextMock.Setup(c => c.InstanceServices).Returns(serviceProviderMock.Object);

        bool nextCalled = false;
        Task Next(FunctionContext ctx) { nextCalled = true; return Task.CompletedTask; }

        // Act
        await middleware.Invoke(contextMock.Object, Next);

        // Assert
        Assert.True(nextCalled);
        loggerMock.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("WireTap middleware invoked for Service Bus function")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Invoke_WhenServiceBusTriggerButNoMessage_LogsWarningAndCallsNext()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ServiceBusWireTapMiddleware>>();
        var options = new ServiceBusWireTapOptions();
        var middleware = new ServiceBusWireTapMiddleware(loggerMock.Object, options);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(ServiceBusWireTapOptions)))
            .Returns(options);
        serviceProviderMock.Setup(sp => sp.GetService(typeof(ILogger<ServiceBusWireTapMiddleware>)))
            .Returns(loggerMock.Object);

        var contextMock = new Mock<FunctionContext>();
        contextMock.Setup(c => c.FunctionDefinition)
            .Returns(new MockFunctionDefinition(hasServiceBusTrigger: true));
        contextMock.Setup(c => c.InstanceServices).Returns(serviceProviderMock.Object);

        bool nextCalled = false;
        Task Next(FunctionContext ctx) { nextCalled = true; return Task.CompletedTask; }

        // Act
        await middleware.Invoke(contextMock.Object, Next);

        // Assert
        Assert.True(nextCalled);
        // The middleware will log a warning when it can't extract the message
        loggerMock.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Could not retrieve a Service Bus message")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task Invoke_WhenExceptionOccurs_LogsErrorAndRethrows()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<ServiceBusWireTapMiddleware>>();
        var options = new ServiceBusWireTapOptions();
        var middleware = new ServiceBusWireTapMiddleware(loggerMock.Object, options);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(ServiceBusWireTapOptions)))
            .Returns(options);
        serviceProviderMock.Setup(sp => sp.GetService(typeof(ILogger<ServiceBusWireTapMiddleware>)))
            .Returns(loggerMock.Object);

        var contextMock = new Mock<FunctionContext>();
        contextMock.Setup(c => c.FunctionDefinition)
            .Returns(new MockFunctionDefinition(hasServiceBusTrigger: true));
        contextMock.Setup(c => c.InstanceServices).Returns(serviceProviderMock.Object);

        // Make the Next delegate throw an exception
        Task Next(FunctionContext ctx) { throw new InvalidOperationException("Test exception"); }

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => middleware.Invoke(contextMock.Object, Next));

        Assert.Equal("Test exception", exception.Message);
        loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Exception thrown in Service Bus WireTap middleware")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}