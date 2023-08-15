using Empowered.Dataverse.Connection.Store.Contracts;
using Empowered.Dataverse.Connection.Store.ErrorHandling;
using Empowered.Dataverse.Connection.Store.Model;

namespace Empowered.Dataverse.Connection.Store.Services;

public class ConnectionMapperTests
{
    private readonly ConnectionMapper _connectionMapper = new();

    [Fact]
    public void ShouldMapExplicitConnectionToInterface()
    {
        var connection = new InteractiveConnection("hello", new Uri("https://xxx.de"));

        var externalConnection = _connectionMapper.ToExternal(connection);

        externalConnection
            .Should().NotBeNull()
            .And.NotBe(connection);
        externalConnection.Should().BeEquivalentTo(connection);
    }

    [Fact]
    public void ShouldMapIClientCertificateConnectionToImplementation()
    {
        var connection = A.Fake<IClientCertificateConnection>();

        var internalConnection = _connectionMapper.ToInternal(connection);

        internalConnection.Should().NotBeNull()
            .And.BeOfType<ClientCertificateConnection>()
            .And.NotBe(connection)
            .And.BeEquivalentTo(connection);
    }

    [Fact]
    public void ShouldMapIClientSecretConnectionToImplementation()
    {
        var connection = A.Fake<IClientSecretConnection>();

        var internalConnection = _connectionMapper.ToInternal(connection);

        internalConnection.Should().NotBeNull()
            .And.BeOfType<ClientSecretConnection>()
            .And.NotBe(connection)
            .And.BeEquivalentTo(connection);
    }

    [Fact]
    public void ShouldMapUserIPasswordConnectionToImplementation()
    {
        var connection = A.Fake<IUserPasswordConnection>();

        var internalConnection = _connectionMapper.ToInternal(connection);

        internalConnection.Should().NotBeNull()
            .And.BeOfType<UserPasswordConnection>()
            .And.NotBe(connection)
            .And.BeEquivalentTo(connection);
    }

    [Fact]
    public void ShouldMapIInteractiveConnectionToImplementation()
    {
        var connection = A.Fake<IInteractiveConnection>();

        var internalConnection = _connectionMapper.ToInternal(connection);

        internalConnection.Should().NotBeNull()
            .And.BeOfType<InteractiveConnection>()
            .And.NotBe(connection)
            .And.BeEquivalentTo(connection);
    }

    [Fact]
    public void ShouldMapIDeviceCodeConnectionToImplementation()
    {
        var connection = A.Fake<IDeviceCodeConnection>();

        var internalConnection = _connectionMapper.ToInternal(connection);

        internalConnection.Should().NotBeNull()
            .And.BeOfType<DeviceCodeConnection>()
            .And.NotBe(connection)
            .And.BeEquivalentTo(connection);
    }

    [Fact]
    public void ShouldMapIManagedIdentityConnectionToImplementation()
    {
        var connection = A.Fake<IManagedIdentityConnection>();

        var internalConnection = _connectionMapper.ToInternal(connection);

        internalConnection.Should().NotBeNull()
            .And.BeOfType<ManagedIdentityConnection>()
            .And.NotBe(connection)
            .And.BeEquivalentTo(connection);
    }

    [Fact]
    public void ShouldMapIAzureDefaultConnectionToImplementation()
    {
        var connection = A.Fake<IAzureDefaultConnection>();

        var internalConnection = _connectionMapper.ToInternal(connection);

        internalConnection.Should().NotBeNull()
            .And.BeOfType<AzureDefaultConnection>()
            .And.NotBe(connection)
            .And.BeEquivalentTo(connection);
    }

    [Fact]
    public void ShouldMapIAzureCliConnectionToImplementation()
    {
        var connection = A.Fake<IAzureCliConnection>();

        var internalConnection = _connectionMapper.ToInternal(connection);

        internalConnection.Should().NotBeNull()
            .And.BeOfType<AzureCliConnection>()
            .And.NotBe(connection)
            .And.BeEquivalentTo(connection);
    }

    [Fact]
    public void ShouldMapIAzureDeveloperCliConnectionToImplementation()
    {
        var connection = A.Fake<IAzureDeveloperCliConnection>();

        var internalConnection = _connectionMapper.ToInternal(connection);

        internalConnection.Should().NotBeNull()
            .And.BeOfType<AzureDeveloperCliConnection>()
            .And.NotBe(connection)
            .And.BeEquivalentTo(connection);
    }

    [Fact]
    public void ShouldMapIAzurePowershellConnectionToImplementation()
    {
        var connection = A.Fake<IAzurePowershellConnection>();

        var internalConnection = _connectionMapper.ToInternal(connection);

        internalConnection.Should().NotBeNull()
            .And.BeOfType<AzurePowershellConnection>()
            .And.NotBe(connection)
            .And.BeEquivalentTo(connection);
    }

    [Fact]
    public void ShouldMapIVisualStudioConnectionToImplementation()
    {
        var connection = A.Fake<IVisualStudioConnection>();

        var internalConnection = _connectionMapper.ToInternal(connection);

        internalConnection.Should().NotBeNull()
            .And.BeOfType<VisualStudioConnection>()
            .And.NotBe(connection)
            .And.BeEquivalentTo(connection);
    }

    [Fact]
    public void ShouldMapIVisualStudioCodeConnectionToImplementation()
    {
        var connection = A.Fake<IVisualStudioCodeConnection>();

        var internalConnection = _connectionMapper.ToInternal(connection);

        internalConnection.Should().NotBeNull()
            .And.BeOfType<VisualStudioCodeConnection>()
            .And.NotBe(connection)
            .And.BeEquivalentTo(connection);
    }

    [Fact]
    public void ShouldThrowOnOutOfRangeConnectionType()
    {
        var connection = A.Fake<ITenantConnection>();

        var action = () => _connectionMapper.ToInternal(connection);

        action.Should().ThrowExactly<ArgumentOutOfRangeException>()
            .WithParameterName("connection")
            .Where(exception => exception.Message.StartsWith(ErrorMessages.ConnectionOutOfRange(connection.GetType())));
    }
}