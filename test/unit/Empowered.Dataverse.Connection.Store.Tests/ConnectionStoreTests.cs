using Empowered.Dataverse.Connection.Store.Contracts;
using Empowered.Dataverse.Connection.Store.ErrorHandling;
using Empowered.Dataverse.Connection.Store.Model;
using Empowered.Dataverse.Connection.Store.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace Empowered.Dataverse.Connection.Store;

public class ConnectionStoreTests
{
    private readonly IWalletFileService _walletFileService;
    private readonly ConnectionStore _connectionStore;

    public ConnectionStoreTests()
    {
        var nullLogger = NullLogger<ConnectionStore>.Instance;
        _walletFileService = A.Fake<IWalletFileService>();
        _connectionStore = new ConnectionStore(_walletFileService, nullLogger);
    }

    [Fact]
    public void ShouldDelegateDefaultGetMethodToGenericImplementation()
    {
        IConnectionStore connectionStore = _connectionStore;
        const string connectionName = "connection";
        var connection = DataverseConnection.InteractiveConnection(
            connectionName,
            new Uri("https://tbd.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<DataverseConnection>
            {
                connection
            }
        };

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        var baseConnection = connectionStore.Get(connectionName);

        baseConnection.Should().BeEquivalentTo(connection);
    }

    [Fact]
    public void ShouldDelegateDefaultTryGetMethodToGenericImplementation()
    {
        IConnectionStore connectionStore = _connectionStore;
        const string connectionName = "connection";
        var connection = DataverseConnection.InteractiveConnection(
            connectionName,
            new Uri("https://tbd.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<DataverseConnection>
            {
                connection
            }
        };

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        connectionStore.TryGet(connectionName, out var baseConnection).Should().BeTrue();

        baseConnection.Should().BeEquivalentTo(connection);
    }

    [Fact]
    public void ShouldDelegateDefaultTryGetActiveMethodToGenericImplementation()
    {
        IConnectionStore connectionStore = _connectionStore;
        const string connectionName = "connection";
        var connection = DataverseConnection.InteractiveConnection(
            connectionName,
            new Uri("https://tbd.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            CurrentConnection = connection,
            ExistingConnections = new HashSet<DataverseConnection>
            {
                connection
            }
        };

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        connectionStore.TryGetActive(out var baseConnection).Should().BeTrue();

        baseConnection.Should().BeEquivalentTo(connection);
    }

    [Fact]
    public void ShouldDelegateDefaultGetActiveMethodToGenericImplementation()
    {
        IConnectionStore connectionStore = _connectionStore;
        const string connectionName = "connection";
        var connection = DataverseConnection.InteractiveConnection(
            connectionName,
            new Uri("https://tbd.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            CurrentConnection = connection,
            ExistingConnections = new HashSet<DataverseConnection>
            {
                connection
            }
        };

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        var baseConnection = connectionStore.GetActive();

        baseConnection.Should().BeEquivalentTo(connection);
    }

    [Fact]
    public void ShouldGetActive()
    {
        var activeConnection = DataverseConnection.InteractiveConnection(
            "active-connection",
            new Uri("https://tbd.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            CurrentConnection = activeConnection,
            ExistingConnections = new HashSet<DataverseConnection>
            {
                activeConnection
            }
        };

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        var connection = _connectionStore.GetActive();

        connection.Should().NotBeNull();
        connection.Name.Should().Be(activeConnection.Name);
        connection.EnvironmentUrl.Should().Be(activeConnection.EnvironmentUrl);
    }

    [Fact]
    public void ShouldThrowOnNonExistingActiveConnection()
    {
        var wallet = new ConnectionWallet();

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);

        var action = () => _connectionStore.GetActive();

        action.Should().ThrowExactly<InvalidOperationException>()
            .Where(exception =>
                !string.IsNullOrWhiteSpace(exception.Message) &&
                exception.Message.StartsWith(ErrorMessages.NoActiveConnection)
            );
    }

    [Fact]
    public void ShouldThrowOnDeletionOfNonExistingConnection()
    {
        const string connectionName = "to-be-deleted";

        var existingConnection = DataverseConnection.InteractiveConnection("another-connection",
            new Uri("https://tbd.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            CurrentConnection = existingConnection,
            ExistingConnections = new HashSet<DataverseConnection>
            {
                existingConnection
            }
        };

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);

        var action = () => _connectionStore.Delete(connectionName);

        action.Should().ThrowExactly<ArgumentException>()
            .Where(exception =>
                !string.IsNullOrWhiteSpace(exception.Message) &&
                exception.Message.StartsWith(ErrorMessages.ConnectionNotFound(connectionName))
            )
            .WithParameterName("name");
    }

    [Fact]
    public void ShouldNotDeleteExistingConnectionWithTry()
    {
        const string connectionName = "to-be-deleted";

        var existingConnection = DataverseConnection.InteractiveConnection(
            "another-connection",
            new Uri("https://tbd.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            CurrentConnection = existingConnection,
            ExistingConnections = new HashSet<DataverseConnection>
            {
                existingConnection
            }
        };

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.TryDelete(connectionName).Should().BeFalse();

        wallet.Current.Should().NotBeNull();
        wallet.Connections.Should().NotContain(x => x.Name == connectionName);
    }

    [Fact]
    public void ShouldPurgeConnections()
    {
        var existingConnection = DataverseConnection.InteractiveConnection(
            "another-connection",
            new Uri("https://tbd.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            CurrentConnection = existingConnection,
            ExistingConnections = new HashSet<DataverseConnection>
            {
                existingConnection
            }
        };

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.Purge();

        wallet.Current.Should().BeNull();
        wallet.Connections.Should().BeEmpty();
    }

    [Fact]
    public void ShouldDeleteExistingConnectionWithTry()
    {
        const string connectionName = "to-be-deleted";

        var existingConnection = DataverseConnection.InteractiveConnection(
            connectionName,
            new Uri("https://tbd.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            CurrentConnection = existingConnection,
            ExistingConnections = new HashSet<DataverseConnection>
            {
                existingConnection
            }
        };

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.TryDelete(connectionName).Should().BeTrue();

        wallet.Current.Should().BeNull();
        wallet.Connections.Should().NotContain(x => x.Name == connectionName);
    }


    [Fact]
    public void ShouldDeleteExistingConnection()
    {
        const string connectionName = "to-be-deleted";

        var existingConnection = DataverseConnection.InteractiveConnection(
            connectionName,
            new Uri("https://tbd.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            CurrentConnection = existingConnection,
            ExistingConnections = new HashSet<DataverseConnection>
            {
                existingConnection
            }
        };

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.Delete(connectionName);

        wallet.Current.Should().BeNull();
        wallet.Connections.Should().NotContain(x => x.Name == connectionName);
    }

    [Fact]
    public void ShouldNotUseCreatedConnection()
    {
        var newConnection = DataverseConnection.InteractiveConnection(
            "new-connection",
            new Uri("https://new.crm4.dynamics.com")
        );

        var wallet = new ConnectionWallet();

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        // ReSharper disable once RedundantArgumentDefaultValue
        _connectionStore.Upsert(newConnection, false);

        wallet.CurrentConnection.Should().BeNull();
    }

    [Fact]
    public void ShouldUseCreatedConnection()
    {
        var newConnection = DataverseConnection.InteractiveConnection(
            "new-connection",
            new Uri("https://new.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet();

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.Upsert(newConnection, true);

        wallet.CurrentConnection.Should().NotBeNull();
        wallet.CurrentConnection.Should().Match<DataverseConnection>(connection =>
            connection.Name == newConnection.Name &&
            connection.EnvironmentUrl == newConnection.EnvironmentUrl
        );
    }

    [Fact]
    public void ShouldNotCreateUnknownConnection()
    {
        var newConnection = new DataverseConnection(
            "new-connection",
            new Uri("https://new.crm4.dynamics.com"),
            ConnectionType.Unknown
        );
        var wallet = new ConnectionWallet();

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        var action = () => _connectionStore.Upsert(newConnection);

        action.Should()
            .ThrowExactly<ArgumentException>()
            .WithParameterName("connection")
            .Where(exception =>
                exception.Message.StartsWith(ErrorMessages.InvalidConnection(newConnection.Name))
            );
    }

    [Fact]
    public void ShouldCreateNewDeviceCodeConnection()
    {
        var newConnection = DataverseConnection.DeviceCodeConnection(
            "new-connection",
            new Uri("https://new.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet();

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.Upsert(newConnection);

        wallet.Connections
            .Where(x => x.Type == ConnectionType.DeviceCode)
            .Should()
            .ContainSingle(connection =>
                connection.Name == newConnection.Name &&
                connection.Type == ConnectionType.DeviceCode &&
                connection.EnvironmentUrl == newConnection.EnvironmentUrl
            );
    }

    [Fact]
    public void ShouldCreateNewInteractiveConnection()
    {
        var newConnection = DataverseConnection.InteractiveConnection(
            "new-connection",
            new Uri("https://new.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet();

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.Upsert(newConnection);

        wallet.Connections
            .Where(x => x.Type == ConnectionType.Interactive)
            .Should()
            .ContainSingle(connection =>
                connection.Name == newConnection.Name &&
                connection.Type == newConnection.Type &&
                connection.EnvironmentUrl == newConnection.EnvironmentUrl
            );
    }

    [Fact]
    public void ShouldCreateNewAzureCliConnection()
    {
        var newConnection = DataverseConnection.AzureCliConnection(
            "new-connection",
            new Uri("https://new.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet();

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.Upsert(newConnection);

        wallet.Connections
            .Where(x => x.Type == ConnectionType.AzureCli)
            .Should()
            .ContainSingle(connection =>
                connection.Name == newConnection.Name &&
                connection.Type == newConnection.Type &&
                connection.EnvironmentUrl == newConnection.EnvironmentUrl
            );
    }

    [Fact]
    public void ShouldCreateNewAzureDefaultConnection()
    {
        var newConnection = DataverseConnection.AzureDefaultConnection(
            "new-connection",
            new Uri("https://new.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet();

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.Upsert(newConnection);

        wallet.Connections
            .Where(x => x.Type == ConnectionType.AzureDefault)
            .Should()
            .ContainSingle(connection =>
                connection.Name == newConnection.Name &&
                connection.Type == newConnection.Type &&
                connection.EnvironmentUrl == newConnection.EnvironmentUrl
            );
    }


    [Fact]
    public void ShouldCreateNewAzureDeveloperCliConnection()
    {
        var newConnection = DataverseConnection.AzureDeveloperCliConnection(
            "new-connection",
            new Uri("https://new.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet();

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.Upsert(newConnection);

        wallet.Connections
            .Where(x => x.Type == ConnectionType.AzureDeveloperCli)
            .Should()
            .ContainSingle(connection =>
                connection.Name == newConnection.Name &&
                connection.Type == newConnection.Type &&
                connection.EnvironmentUrl == newConnection.EnvironmentUrl
            );
    }


    [Fact]
    public void ShouldCreateNewAzurePowershellConnection()
    {
        var newConnection = DataverseConnection.AzurePowershellConnection(
            "new-connection",
            new Uri("https://new.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet();

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.Upsert(newConnection);

        wallet.Connections
            .Where(x => x.Type == ConnectionType.AzurePowershell)
            .Should()
            .ContainSingle(connection =>
                connection.Name == newConnection.Name &&
                connection.Type == newConnection.Type &&
                connection.EnvironmentUrl == newConnection.EnvironmentUrl
            );
    }


    [Fact]
    public void ShouldCreateNewVisualStudioCodeConnection()
    {
        var newConnection = DataverseConnection.VisualStudioCodeConnection(
            "new-connection",
            new Uri("https://new.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet();

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.Upsert(newConnection);

        wallet.Connections
            .Where(x => x.Type == ConnectionType.VisualStudioCode)
            .Should()
            .ContainSingle(connection =>
                connection.Name == newConnection.Name &&
                connection.Type == newConnection.Type &&
                connection.EnvironmentUrl == newConnection.EnvironmentUrl
            );
    }


    [Fact]
    public void ShouldCreateNewVisualStudioConnection()
    {
        var newConnection = DataverseConnection.VisualStudioConnection(
            "new-connection",
            new Uri("https://new.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet();

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.Upsert(newConnection);

        wallet.Connections
            .Where(x => x.Type == ConnectionType.VisualStudio)
            .Should()
            .ContainSingle(connection =>
                connection.Name == newConnection.Name &&
                connection.Type == newConnection.Type &&
                connection.EnvironmentUrl == newConnection.EnvironmentUrl
            );
    }


    [Fact]
    public void ShouldCreateNewManagedIdentityConnection()
    {
        var newConnection = DataverseConnection.ManagedIdentityConnection(
            "new-connection",
            new Uri("https://new.crm4.dynamics.com"),
            Guid.NewGuid().ToString("D")
        );
        var wallet = new ConnectionWallet();

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.Upsert(newConnection);

        wallet.Connections
            .Where(x => x.Type == ConnectionType.ManagedIdentity)
            .Should()
            .ContainSingle(connection =>
                connection.Name == newConnection.Name &&
                connection.Type == newConnection.Type &&
                connection.EnvironmentUrl == newConnection.EnvironmentUrl &&
                connection.ApplicationId == newConnection.ApplicationId
            );
    }

    [Fact]
    public void ShouldCreateNewUserPasswordConnection()
    {
        var newConnection = DataverseConnection.UserPasswordConnection(
            "new-connection",
            new Uri("https://new.crm4.dynamics.com"),
            "me@example.com",
            "secret",
            Guid.NewGuid().ToString("D")
        );
        var wallet = new ConnectionWallet();

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.Upsert(newConnection);

        wallet.Connections
            .Where(x => x.Type == ConnectionType.UserPassword)
            .Should()
            .ContainSingle(connection =>
                connection.Name == newConnection.Name &&
                connection.Type == ConnectionType.UserPassword &&
                connection.EnvironmentUrl == newConnection.EnvironmentUrl &&
                connection.UserName == newConnection.UserName &&
                connection.Password == newConnection.Password &&
                connection.TenantId == newConnection.TenantId
            );
    }

    [Fact]
    public void ShouldCreateNewClientSecretConnection()
    {
        var newConnection = DataverseConnection.ClientSecretConnection(
            "new-connection",
            new Uri("https://new.crm4.dynamics.com"),
            Guid.NewGuid().ToString("D"),
            Guid.NewGuid().ToString("D"),
            "secret"
        );
        var wallet = new ConnectionWallet();

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.Upsert(newConnection);

        wallet.Connections
            .Where(x => x.Type == ConnectionType.ClientSecret)
            .Should()
            .ContainSingle(connection =>
                connection.Name == newConnection.Name &&
                connection.Type == ConnectionType.ClientSecret &&
                connection.EnvironmentUrl == newConnection.EnvironmentUrl &&
                connection.TenantId == newConnection.TenantId &&
                connection.ApplicationId == newConnection.ApplicationId &&
                connection.ClientSecret == newConnection.ClientSecret
            );
    }

    [Fact]
    public void ShouldCreateNewCertificateConnection()
    {
        var newConnection = DataverseConnection.ClientCertificateConnection(
            "new-connection",
            new Uri("https://new.crm4.dynamics.com"),
            Guid.NewGuid().ToString("D"),
            Guid.NewGuid().ToString("D"),
            "C:\\Temp",
            "secret"
        );
        var wallet = new ConnectionWallet();

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.Upsert(newConnection);

        wallet.Connections
            .Where(x => x.Type == ConnectionType.ClientCertificate)
            .Should()
            .ContainSingle(connection =>
                connection.Name == newConnection.Name &&
                connection.Type == ConnectionType.ClientCertificate &&
                connection.EnvironmentUrl == newConnection.EnvironmentUrl &&
                connection.ApplicationId == newConnection.ApplicationId &&
                connection.TenantId == newConnection.TenantId &&
                connection.CertificateFilePath == newConnection.CertificateFilePath &&
                connection.CertificatePassword == newConnection.CertificatePassword
            );
    }

    [Fact]
    public void ShouldDelegateDefaultUpsertMethodToGenericImplementation()
    {
        var newConnection = DataverseConnection.ClientCertificateConnection(
            "new-connection",
            new Uri("https://new.crm4.dynamics.com"),
            Guid.NewGuid().ToString("D"),
            Guid.NewGuid().ToString("D"),
            "C:\\Temp",
            "secret"
        );
        var wallet = new ConnectionWallet();

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.Upsert(newConnection);

        wallet.Connections
            .Where(x => x.Type == ConnectionType.ClientCertificate)
            .Should()
            .ContainSingle(connection =>
                connection.Name == newConnection.Name &&
                connection.Type == ConnectionType.ClientCertificate &&
                connection.EnvironmentUrl == newConnection.EnvironmentUrl &&
                connection.ApplicationId == newConnection.ApplicationId &&
                connection.TenantId == newConnection.TenantId &&
                connection.CertificateFilePath == newConnection.CertificateFilePath &&
                connection.CertificatePassword == newConnection.CertificatePassword
            );
    }

    [Fact]
    public void ShouldUpsertExistingConnection()
    {
        const string connectionName = "existing-connection";
        var existingConnection = DataverseConnection.InteractiveConnection(
            connectionName,
            new Uri("https://new.crm4.dynamics.com")
        );

        var upsertConnection = DataverseConnection.ClientSecretConnection(
            connectionName,
            new Uri("https://new-url.crm4.dynamics.com"),
            Guid.NewGuid().ToString("D"),
            Guid.NewGuid().ToString("D"),
            "secret"
        );

        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<DataverseConnection>
            {
                existingConnection
            },
            CurrentConnection = existingConnection
        };

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.Upsert(upsertConnection);

        wallet.Current.Should().NotBeNull();
        wallet.Current!.Name.Should().Be(upsertConnection.Name);
        wallet.Current.EnvironmentUrl.Should().Be(upsertConnection.EnvironmentUrl);
        wallet.Current.ApplicationId.Should().Be(upsertConnection.ApplicationId);
        wallet.Current.TenantId.Should().Be(upsertConnection.TenantId);
        wallet.Current.ClientSecret.Should().Be(upsertConnection.ClientSecret);

        wallet.Connections
            .Where(x => x.Type == ConnectionType.ClientSecret)
            .Should()
            .Satisfy(con =>
                con.Name == upsertConnection.Name &&
                con.EnvironmentUrl == upsertConnection.EnvironmentUrl &&
                con.ApplicationId == upsertConnection.ApplicationId &&
                con.TenantId == upsertConnection.TenantId &&
                con.ClientSecret == upsertConnection.ClientSecret
            );
    }

    [Fact]
    public void ShouldListExistingConnections()
    {
        var currentConnection = DataverseConnection.InteractiveConnection(
            "current-connection",
            new Uri("https://my.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            CurrentConnection = currentConnection,
            ExistingConnections = new HashSet<DataverseConnection>
            {
                currentConnection,
                DataverseConnection.InteractiveConnection("a", new Uri("https://a.crm4.dynamics.com")),
                DataverseConnection.InteractiveConnection("b", new Uri("https://b.crm4.dynamics.com"))
            }
        };
        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);

        var retrievedWallet = _connectionStore.List();

        retrievedWallet.Should().NotBeNull();
        retrievedWallet.Current.Should().NotBeNull();
        retrievedWallet.Current!.EnvironmentUrl.Should().Be(currentConnection.EnvironmentUrl);
        retrievedWallet.Current.Name.Should().Be(currentConnection.Name);

        retrievedWallet.Connections
            .Should()
            .HaveCount(wallet.ExistingConnections.Count)
            .And.OnlyContain(connection =>
                wallet.Connections.Select(x => x.Name).Contains(connection.Name)
            );
    }

    [Fact]
    public void ShouldGetExistingConnection()
    {
        const string connectionName = "my-connection";
        var existingConnection = DataverseConnection.InteractiveConnection(
            connectionName,
            new Uri("https://my-env.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<DataverseConnection>
            {
                existingConnection
            }
        };
        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);

        var connection = _connectionStore.Get(connectionName);

        connection.Should().NotBeNull();
        connection.Name.Should().Be(connectionName);
        connection.EnvironmentUrl.Should().Be(existingConnection.EnvironmentUrl);
    }

    [Fact]
    public void ShouldThrowOnNonExistingConnection()
    {
        const string connectionName = "my-connection";
        var anotherConnection = DataverseConnection.InteractiveConnection(
            "another-connection",
            new Uri("https://my-env.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<DataverseConnection>
            {
                anotherConnection
            }
        };
        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);

        var action = () => _connectionStore.Get(connectionName);

        action.Should()
            .ThrowExactly<ArgumentException>()
            .WithParameterName("name")
            .Where(exception =>
                exception.Message.StartsWith(ErrorMessages.ConnectionNotFound(connectionName))
            );
    }

    [Fact]
    public void ShouldThrowOnMissingConnectionName()
    {
        const string connectionName = "   ";

        IDataverseConnection? connection = null;
        var action = () => _connectionStore.TryGet(connectionName, out connection);

        action.Should()
            .ThrowExactly<ArgumentNullException>()
            .WithParameterName("name");

        connection.Should().BeNull();
    }

    [Fact]
    public void ShouldGetExistingConnectionWithTry()
    {
        const string connectionName = "my-connection";
        var existingConnection = DataverseConnection.InteractiveConnection(
            connectionName,
            new Uri("https://my-env.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<DataverseConnection>
            {
                existingConnection
            }
        };
        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);

        var isExistingConnection = _connectionStore.TryGet(connectionName, out var connection);

        isExistingConnection.Should().BeTrue();
        connection.Should().NotBeNull();
        connection!.Name.Should().Be(connectionName);
        connection.EnvironmentUrl.Should().Be(existingConnection.EnvironmentUrl);
    }

    [Fact]
    public void ShouldNotGetExistingConnectionWithTry()
    {
        const string connectionName = "my-connection";
        var existingConnection = DataverseConnection.InteractiveConnection(
            "another-connection",
            new Uri("https://my-env.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<DataverseConnection>
            {
                existingConnection
            }
        };
        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);

        var isExistingConnection = _connectionStore.TryGet(connectionName, out var connection);

        isExistingConnection.Should().BeFalse();
        connection.Should().BeNull();
    }

    [Fact]
    public void ShouldUseExistingConnection()
    {
        const string connectionName = "my-connection";
        var existingConnection = DataverseConnection.InteractiveConnection(
            connectionName,
            new Uri("https://my.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<DataverseConnection>
            {
                existingConnection
            },
            CurrentConnection = null
        };

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.Use(connectionName);

        wallet.Current.Should().NotBeNull();
        var baseConnection = wallet.Current.As<DataverseConnection>();
        baseConnection.Name.Should().Be(existingConnection.Name);
        baseConnection.EnvironmentUrl.Should().Be(existingConnection.EnvironmentUrl);
    }

    [Fact]
    public void ShouldThrowOnUsingNonExistingConnection()
    {
        const string connectionName = "my-connection";
        var existingConnection = DataverseConnection.InteractiveConnection(
            "another-connection",
            new Uri("https://my.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<DataverseConnection>
            {
                existingConnection
            },
            CurrentConnection = null
        };

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        var action = () => _connectionStore.Use(connectionName);

        action.Should()
            .ThrowExactly<ArgumentException>()
            .WithParameterName("name")
            .Where(exception =>
                exception.Message.StartsWith(ErrorMessages.ConnectionNotFound(connectionName))
            );
    }
}