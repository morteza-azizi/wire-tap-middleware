using System.Collections.Immutable;
using Microsoft.Azure.Functions.Worker.Definition;
using Microsoft.Azure.Functions.Worker;
using Moq;

public class MockFunctionDefinition : FunctionDefinition
{
    private readonly bool _hasServiceBusTrigger;
    public MockFunctionDefinition(bool hasServiceBusTrigger)
    {
        _hasServiceBusTrigger = hasServiceBusTrigger;
    }
    public override string PathToAssembly => "TestAssembly";
    public override string EntryPoint => "TestEntryPoint";
    public override string Id => "TestId";
    public override string Name => "TestFunction";
    public override IImmutableDictionary<string, BindingMetadata> InputBindings =>
        _hasServiceBusTrigger
            ? ImmutableDictionary<string, BindingMetadata>.Empty.Add("sb", CreateBindingMetadata("serviceBusTrigger"))
            : ImmutableDictionary<string, BindingMetadata>.Empty.Add("http", CreateBindingMetadata("httpTrigger"));
    public override IImmutableDictionary<string, BindingMetadata> OutputBindings => ImmutableDictionary<string, BindingMetadata>.Empty;
    public override ImmutableArray<FunctionParameter> Parameters => ImmutableArray<FunctionParameter>.Empty;

    private static BindingMetadata CreateBindingMetadata(string type)
    {
        var mock = new Mock<BindingMetadata>();
        mock.Setup(m => m.Type).Returns(type);
        return mock.Object;
    }
} 