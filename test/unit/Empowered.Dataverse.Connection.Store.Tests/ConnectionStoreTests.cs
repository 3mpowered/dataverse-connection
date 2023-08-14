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
        _connectionStore = new ConnectionStore(_walletFileService, new ConnectionMapper(), nullLogger);
    }

    [Fact]
    public void ShouldGetActive()
    {
        var activeConnection = new InteractiveConnection(
            "active-connection",
            new Uri("https://tbd.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            CurrentConnection = activeConnection,
            ExistingConnections = new HashSet<BaseConnection>
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

        var connection = _connectionStore.GetActive<IInteractiveConnection>();

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

        var action = () => _connectionStore.GetActive<BaseConnection>();

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

        var existingConnection = new InteractiveConnection("another-connection",
            new Uri("https://tbd.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            CurrentConnection = existingConnection,
            ExistingConnections = new HashSet<BaseConnection>
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

        var existingConnection = new InteractiveConnection(
            "another-connection",
            new Uri("https://tbd.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            CurrentConnection = existingConnection,
            ExistingConnections = new HashSet<BaseConnection>
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
        var existingConnection = new InteractiveConnection(
            "another-connection",
            new Uri("https://tbd.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            CurrentConnection = existingConnection,
            ExistingConnections = new HashSet<BaseConnection>
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

        var existingConnection = new InteractiveConnection(
            connectionName,
            new Uri("https://tbd.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            CurrentConnection = existingConnection,
            ExistingConnections = new HashSet<BaseConnection>
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

        var existingConnection = new InteractiveConnection(
            connectionName,
            new Uri("https://tbd.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            CurrentConnection = existingConnection,
            ExistingConnections = new HashSet<BaseConnection>
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
        var newConnection = new InteractiveConnection(
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

        _connectionStore.Upsert(newConnection, false);

        wallet.CurrentConnection.Should().BeNull();
    }

    [Fact]
    public void ShouldUseCreatedConnection()
    {
        var newConnection = new InteractiveConnection(
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
        wallet.CurrentConnection.Should().Match<InteractiveConnection>(connection =>
            connection.Name == newConnection.Name &&
            connection.EnvironmentUrl == newConnection.EnvironmentUrl
        );
    }

    [Fact]
    public void ShouldNotCreateUnknownConnection()
    {
        var newConnection = new BaseConnection(
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
        var newConnection = new DeviceCodeConnection(
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
            .OfType<IDeviceCodeConnection>()
            .Should()
            .ContainSingle(connection =>
                connection.Name == newConnection.Name &&
                connection.Type == ConnectionType.DeviceCode &&
                connection.EnvironmentUrl == newConnection.EnvironmentUrl
            );
    }
    
    [Fact]
    public void ShouldCreateNewUserPasswordConnection()
    {
        var newConnection = new UserPasswordConnection(
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
            .OfType<IUserPasswordConnection>()
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
        var newConnection = new ClientSecretConnection(
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
            .OfType<IClientSecretConnection>()
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
        var newConnection = new ClientCertificateConnection(
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
            .OfType<IClientCertificateConnection>()
            .Should()
            .ContainSingle(connection =>
                connection.Name == newConnection.Name &&
                connection.Type == ConnectionType.ClientCertificate &&
                connection.EnvironmentUrl == newConnection.EnvironmentUrl &&
                connection.ApplicationId == newConnection.ApplicationId &&
                connection.TenantId == newConnection.TenantId &&
                connection.FilePath == newConnection.FilePath &&
                connection.Password == newConnection.Password
            );
    }

    [Fact]
    public void ShouldUpsertExistingConnection()
    {
        const string connectionName = "existing-connection";
        var existingConnection = new InteractiveConnection(
            connectionName,
            new Uri("https://new.crm4.dynamics.com")
        );

        var upsertConnection = new ClientSecretConnection(
            connectionName,
            new Uri("https://new-url.crm4.dynamics.com"),
            Guid.NewGuid().ToString("D"),
            Guid.NewGuid().ToString("D"),
            "secret"
        );

        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<BaseConnection>
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
        var connection = wallet.Current.As<IClientSecretConnection>();
        connection.Name.Should().Be(upsertConnection.Name);
        connection.EnvironmentUrl.Should().Be(upsertConnection.EnvironmentUrl);
        connection.ApplicationId.Should().Be(upsertConnection.ApplicationId);
        connection.TenantId.Should().Be(upsertConnection.TenantId);
        connection.ClientSecret.Should().Be(upsertConnection.ClientSecret);

        wallet.Connections
            .OfType<IClientSecretConnection>()
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
        var currentConnection = new BaseConnection(
            "current-connection",
            new Uri("https://my.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            CurrentConnection = currentConnection,
            ExistingConnections = new HashSet<BaseConnection>
            {
                currentConnection,
                new("a", new Uri("https://a.crm4.dynamics.com")),
                new("b", new Uri("https://b.crm4.dynamics.com"))
            }
        };
        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);

        var retrievedWallet = _connectionStore.List();

        retrievedWallet.Should().NotBeNull();
        retrievedWallet.Current.Should().NotBeNull();
        retrievedWallet.Current.EnvironmentUrl.Should().Be(currentConnection.EnvironmentUrl);
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
        var existingConnection = new InteractiveConnection(
            connectionName,
            new Uri("https://my-env.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<BaseConnection>
            {
                existingConnection
            }
        };
        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);

        var connection = _connectionStore.Get<IInteractiveConnection>(connectionName);

        connection.Should().NotBeNull();
        connection.Name.Should().Be(connectionName);
        connection.EnvironmentUrl.Should().Be(existingConnection.EnvironmentUrl);
    }

    [Fact]
    public void ShouldThrowOnNonExistingConnection()
    {
        const string connectionName = "my-connection";
        var anotherConnection = new InteractiveConnection(
            "another-connection",
            new Uri("https://my-env.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<BaseConnection>
            {
                anotherConnection
            }
        };
        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);

        var action = () => _connectionStore.Get<IInteractiveConnection>(connectionName);

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

        IInteractiveConnection? connection = null;
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
        var existingConnection = new InteractiveConnection(
            connectionName,
            new Uri("https://my-env.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<BaseConnection>
            {
                existingConnection
            }
        };
        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);

        var isExistingConnection = _connectionStore.TryGet(connectionName, out IInteractiveConnection? connection);

        isExistingConnection.Should().BeTrue();
        connection.Should().NotBeNull();
        connection.Name.Should().Be(connectionName);
        connection.EnvironmentUrl.Should().Be(existingConnection.EnvironmentUrl);
    }

    [Fact]
    public void ShouldNotGetExistingConnectionWithTry()
    {
        const string connectionName = "my-connection";
        var existingConnection = new InteractiveConnection(
            "another-connection",
            new Uri("https://my-env.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<BaseConnection>
            {
                existingConnection
            }
        };
        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);

        var isExistingConnection = _connectionStore.TryGet(connectionName, out IInteractiveConnection? connection);

        isExistingConnection.Should().BeFalse();
        connection.Should().BeNull();
    }

    [Fact]
    public void ShouldUseExistingConnection()
    {
        const string connectionName = "my-connection";
        var existingConnection = new InteractiveConnection(
            connectionName,
            new Uri("https://my.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<BaseConnection>
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
        var baseConnection = wallet.Current.As<IInteractiveConnection>();
        baseConnection.Name.Should().Be(existingConnection.Name);
        baseConnection.EnvironmentUrl.Should().Be(existingConnection.EnvironmentUrl);
    }

    [Fact]
    public void ShouldThrowOnUsingNonExistingConnection()
    {
        const string connectionName = "my-connection";
        var existingConnection = new InteractiveConnection(
            "another-connection",
            new Uri("https://my.crm4.dynamics.com")
        );
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<BaseConnection>
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