using Empowered.Dataverse.Connection.Store.Contract;
using Empowered.Dataverse.Connection.Store.ErrorHandling;
using Empowered.Dataverse.Connection.Store.Extensions;
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
    public void ShouldThrowOnDeletionOfNonExistingConnection()
    {
        const string connectionName = "to-be-deleted";

        var existingConnection = new Model.Connection
        {
            Name = "another-connection",
            EnvironmentUrl = new Uri("https://tbd.crm4.dynamics.com"),
            UserName = "me@example.com"
        };
        var wallet = new ConnectionWallet
        {
            CurrentConnection = existingConnection,
            ExistingConnections = new HashSet<Model.Connection>
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

        var existingConnection = new Model.Connection
        {
            Name = "another-connection",
            EnvironmentUrl = new Uri("https://tbd.crm4.dynamics.com"),
            UserName = "me@example.com"
        };
        var wallet = new ConnectionWallet
        {
            CurrentConnection = existingConnection,
            ExistingConnections = new HashSet<Model.Connection>
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
        var existingConnection = new Model.Connection
        {
            Name = "another-connection",
            EnvironmentUrl = new Uri("https://tbd.crm4.dynamics.com"),
            UserName = "me@example.com"
        };
        var wallet = new ConnectionWallet
        {
            CurrentConnection = existingConnection,
            ExistingConnections = new HashSet<Model.Connection>
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

        var existingConnection = new Model.Connection
        {
            Name = connectionName,
            EnvironmentUrl = new Uri("https://tbd.crm4.dynamics.com"),
            UserName = "me@example.com"
        };
        var wallet = new ConnectionWallet
        {
            CurrentConnection = existingConnection,
            ExistingConnections = new HashSet<Model.Connection>
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

        var existingConnection = new Model.Connection
        {
            Name = connectionName,
            EnvironmentUrl = new Uri("https://tbd.crm4.dynamics.com"),
            UserName = "me@example.com"
        };
        var wallet = new ConnectionWallet
        {
            CurrentConnection = existingConnection,
            ExistingConnections = new HashSet<Model.Connection>
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
        var newConnection = new Model.Connection
        {
            Name = "new-connection",
            EnvironmentUrl = new Uri("https://new.crm4.dynamics.com"),
            UserName = "me@example.com"
        };
        var wallet = new ConnectionWallet();

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.Upsert(newConnection, "my-secret".ToSecureString(), false);

        wallet.CurrentConnection.Should().BeNull();
    }

    [Fact]
    public void ShouldUseCreatedConnection()
    {
        var newConnection = new Model.Connection
        {
            Name = "new-connection",
            EnvironmentUrl = new Uri("https://new.crm4.dynamics.com"),
            UserName = "me@example.com"
        };
        var wallet = new ConnectionWallet();

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.Upsert(newConnection, "my-secret".ToSecureString(), true);

        wallet.CurrentConnection.Should().NotBeNull();
        wallet.CurrentConnection.Should().Match<IConnection>(connection =>
            connection.Name == newConnection.Name &&
            connection.ConnectionType == ConnectionType.UserPassword &&
            connection.EnvironmentUrl == newConnection.EnvironmentUrl &&
            connection.UserName == newConnection.UserName
        );
    }

    [Fact]
    public void ShouldCreateNewUserPasswordConnection()
    {
        var newConnection = new Model.Connection
        {
            Name = "new-connection",
            EnvironmentUrl = new Uri("https://new.crm4.dynamics.com"),
            UserName = "me@example.com",
            ApplicationId = Guid.NewGuid().ToString("D")
        };
        var wallet = new ConnectionWallet();

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        var action = () => _connectionStore.Upsert(newConnection, "my-secret".ToSecureString());

        action.Should()
            .ThrowExactly<ArgumentException>()
            .WithParameterName("connection")
            .Where(exception =>
                exception.Message.StartsWith(ErrorMessages.InvalidConnection(newConnection))
            );
    }

    [Fact]
    public void ShouldNotCreateUnknownConnection()
    {
        var newConnection = new Model.Connection
        {
            Name = "new-connection",
            EnvironmentUrl = new Uri("https://new.crm4.dynamics.com"),
            UserName = "me@example.com"
        };
        var wallet = new ConnectionWallet();

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.Upsert(newConnection, "my-secret".ToSecureString());

        wallet.Connections.Should().ContainSingle(connection =>
            connection.Name == newConnection.Name &&
            connection.ConnectionType == ConnectionType.UserPassword &&
            connection.EnvironmentUrl == newConnection.EnvironmentUrl &&
            connection.UserName == newConnection.UserName
        );
    }

    [Fact]
    public void ShouldCreateNewClientSecretConnection()
    {
        var newConnection = new Model.Connection
        {
            Name = "new-connection",
            EnvironmentUrl = new Uri("https://new.crm4.dynamics.com"),
            TenantId = Guid.NewGuid().ToString("D"),
            ApplicationId = Guid.NewGuid().ToString("D"),
        };
        var wallet = new ConnectionWallet();

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.Upsert(newConnection, "my-secret".ToSecureString());

        wallet.Connections.Should().ContainSingle(connection =>
            connection.Name == newConnection.Name &&
            connection.ConnectionType == ConnectionType.ClientSecret &&
            connection.EnvironmentUrl == newConnection.EnvironmentUrl &&
            connection.TenantId == newConnection.TenantId &&
            connection.ApplicationId == newConnection.ApplicationId
        );
    }

    [Fact]
    public void ShouldCreateNewCertificateConnection()
    {
        var newConnection = new Model.Connection
        {
            Name = "new-connection",
            EnvironmentUrl = new Uri("https://new.crm4.dynamics.com"),
            ApplicationId = Guid.NewGuid().ToString("D"),
            TenantId = Guid.NewGuid().ToString("D"),
            CertificateFilePath = "C:\\Temp"
        };
        var wallet = new ConnectionWallet();

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);
        A.CallTo(() => _walletFileService.WriteWallet(A<ConnectionWallet>._))
            .Invokes(call =>
                wallet = call.Arguments.First().As<ConnectionWallet>()
            );

        _connectionStore.Upsert(newConnection, "my-secret".ToSecureString());

        wallet.Connections.Should().ContainSingle(connection =>
            connection.Name == newConnection.Name &&
            connection.ConnectionType == ConnectionType.Certificate &&
            connection.EnvironmentUrl == newConnection.EnvironmentUrl &&
            connection.ApplicationId == newConnection.ApplicationId &&
            connection.TenantId == newConnection.TenantId &&
            connection.CertificateFilePath == newConnection.CertificateFilePath
        );
    }

    [Fact]
    public void ShouldUpsertExistingConnection()
    {
        const string connectionName = "existing-connection";
        var existingConnection = new Model.Connection
        {
            Name = connectionName,
            EnvironmentUrl = new Uri("https://new.crm4.dynamics.com"),
            UserName = "me@example.com"
        };

        var upsertConnection = new Model.Connection
        {
            Name = connectionName,
            EnvironmentUrl = new Uri("https://new-url.crm4.dynamics.com"),
            ApplicationId = Guid.NewGuid().ToString("D"),
            TenantId = Guid.NewGuid().ToString("D")
        };

        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<Model.Connection>
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

        _connectionStore.Upsert(upsertConnection, "my-secret".ToSecureString());

        wallet.Current.Should().NotBeNull();
        wallet.Current.Name.Should().Be(upsertConnection.Name);
        wallet.Current.EnvironmentUrl.Should().Be(upsertConnection.EnvironmentUrl);
        wallet.Current.ConnectionType.Should().Be(ConnectionType.ClientSecret);
        wallet.Current.ApplicationId.Should().Be(upsertConnection.ApplicationId);
        wallet.Current.TenantId.Should().Be(upsertConnection.TenantId);
        wallet.Current.UserName.Should().BeNull();
        wallet.Current.CertificateFilePath.Should().BeNull();

        wallet.Connections.Should().Satisfy(connection =>
            connection.Name == upsertConnection.Name &&
            connection.EnvironmentUrl == upsertConnection.EnvironmentUrl &&
            connection.ConnectionType == ConnectionType.ClientSecret &&
            connection.ApplicationId == upsertConnection.ApplicationId &&
            connection.TenantId == upsertConnection.TenantId &&
            connection.UserName == null &&
            connection.CertificateFilePath == null
        );
    }

    [Fact]
    public void ShouldListExistingConnections()
    {
        var currentConnection = new Model.Connection
        {
            Name = "current-connection",
            EnvironmentUrl = new Uri("https://my.crm4.dynamics.com")
        };
        var wallet = new ConnectionWallet
        {
            CurrentConnection = currentConnection,
            ExistingConnections = new HashSet<Model.Connection>
            {
                currentConnection,
                new()
                {
                    Name = "a",
                    EnvironmentUrl = new Uri("https://a.crm4.dynamics.com")
                },
                new()
                {
                    Name = "b",
                    EnvironmentUrl = new Uri("https://b.crm4.dynamics.com")
                }
            }
        };
        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);

        var retrievedWallet = _connectionStore.List();

        retrievedWallet.Should().NotBeNull();
        retrievedWallet.Current.Should().Be(currentConnection);

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
        var existingConnection = new Model.Connection
        {
            Name = connectionName,
            EnvironmentUrl = new Uri("https://my-env.crm4.dynamics.com"),
            UserName = "myuser@example.com",
        };
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<Model.Connection>
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
        connection.UserName.Should().Be(existingConnection.UserName);
    }

    [Fact]
    public void ShouldThrowOnNonExistingConnection()
    {
        const string connectionName = "my-connection";
        var anotherConnection = new Model.Connection
        {
            Name = "another-connection",
            EnvironmentUrl = new Uri("https://my-env.crm4.dynamics.com"),
            UserName = "myuser@example.com",
        };
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<Model.Connection>
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

        IConnection? connection = null;
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
        var existingConnection = new Model.Connection
        {
            Name = connectionName,
            EnvironmentUrl = new Uri("https://my-env.crm4.dynamics.com"),
            UserName = "myuser@example.com",
        };
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<Model.Connection>
            {
                existingConnection
            }
        };
        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);

        var isExistingConnection = _connectionStore.TryGet(connectionName, out var connection);

        isExistingConnection.Should().BeTrue();
        connection.Should().NotBeNull();
        connection.Name.Should().Be(connectionName);
        connection.EnvironmentUrl.Should().Be(existingConnection.EnvironmentUrl);
        connection.UserName.Should().Be(existingConnection.UserName);
    }

    [Fact]
    public void ShouldNotGetExistingConnectionWithTry()
    {
        const string connectionName = "my-connection";
        var existingConnection = new Model.Connection
        {
            Name = "another-connection",
            EnvironmentUrl = new Uri("https://my-env.crm4.dynamics.com"),
            UserName = "myuser@example.com",
        };
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<Model.Connection>
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
        var existingConnection = new Model.Connection
        {
            Name = connectionName,
            EnvironmentUrl = new Uri("https://my.crm4.dynamics.com"),
            UserName = "me@example.com"
        };
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<Model.Connection>
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
        wallet.Current.Name.Should().Be(existingConnection.Name);
        wallet.Current.EnvironmentUrl.Should().Be(existingConnection.EnvironmentUrl);
        wallet.Current.UserName.Should().Be(existingConnection.UserName);
        wallet.Current.ApplicationId.Should().Be(existingConnection.ApplicationId);
        wallet.Current.TenantId.Should().Be(existingConnection.TenantId);
        wallet.Current.CertificateFilePath.Should().Be(existingConnection.CertificateFilePath);
    }

    [Fact]
    public void ShouldThrowOnUsingNonExistingConnection()
    {
        const string connectionName = "my-connection";
        var existingConnection = new Model.Connection
        {
            Name = "another-connection",
            EnvironmentUrl = new Uri("https://my.crm4.dynamics.com"),
            UserName = "me@example.com"
        };
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<Model.Connection>
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