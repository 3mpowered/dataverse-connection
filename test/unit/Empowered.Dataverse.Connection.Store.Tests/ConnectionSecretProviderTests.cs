using Empowered.Dataverse.Connection.Store.ErrorHandling;
using Empowered.Dataverse.Connection.Store.Extensions;
using Empowered.Dataverse.Connection.Store.Model;
using Empowered.Dataverse.Connection.Store.Services;


namespace Empowered.Dataverse.Connection.Store;

public class ConnectionSecretProviderTests
{
    private readonly IWalletFileService _walletFileService;
    private readonly ConnectionSecretProvider _secretProvider;

    public ConnectionSecretProviderTests()
    {
        _walletFileService = A.Fake<IWalletFileService>();
        _secretProvider = new ConnectionSecretProvider(_walletFileService);
    }

    [Fact]
    public void ShouldReturnClientSecret()
    {
        const string connectionName = "my-connection";
        var secretConnection = new SecretConnection
        {
            Name = connectionName,
            EnvironmentUrl = new Uri("https://con.crm16.dynamics.com"),
            ApplicationId = Guid.NewGuid().ToString("D"),
            TenantId = Guid.NewGuid().ToString("D"),
            ClientSecret = "my-password"
        };
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<SecretConnection>
            {
                secretConnection
            }
        };

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);

        var secret = _secretProvider.GetConnectionSecret(connectionName);

        secret.Should().Be(secretConnection.ClientSecret);
    }

    [Fact]
    public void ShouldReturnUserPassword()
    {
        const string connectionName = "my-connection";
        var secretConnection = new SecretConnection
        {
            Name = connectionName,
            EnvironmentUrl = new Uri("https://con.crm16.dynamics.com"),
            UserName = "my@test.com",
            Password = "my-password"
        };
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<SecretConnection>
            {
                secretConnection
            }
        };

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);

        var secret = _secretProvider.GetConnectionSecret(connectionName);

        secret.Should().Be(secretConnection.Password);
    }

    [Fact]
    public void ShouldReturnCertificatePassword()
    {
        const string connectionName = "my-connection";
        var secretConnection = new SecretConnection
        {
            Name = connectionName,
            EnvironmentUrl = new Uri("https://con.crm16.dynamics.com"),
            ApplicationId = Guid.NewGuid().ToString("D"),
            TenantId = Guid.NewGuid().ToString("D"),
            CertificateFilePath = "C:\\temp",
            CertificatePassword = "my-password"
        };
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<SecretConnection>
            {
                secretConnection
            }
        };

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);

        var secret = _secretProvider.GetConnectionSecret(connectionName);

        secret.Should().Be(secretConnection.CertificatePassword);
    }

    [Fact]
    public void ShouldThrowOnMissingConnectionName()
    {
        const string connectionName = "my-connection";
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<SecretConnection>()
        };

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);

        Action action = () => _secretProvider.GetConnectionSecret(connectionName);

        action.Should().ThrowExactly<ArgumentException>()
            .WithParameterName("name")
            .Where(exception => exception.Message.StartsWith(ErrorMessages.ConnectionNotFound(connectionName)));
    }
    
    [Fact]
    public void ShouldThrowOnUnknownConnectionType()
    {
        const string connectionName = "my-connection";
        var secretConnection = new SecretConnection
        {
            Name = connectionName,
            EnvironmentUrl = new Uri("https://con.crm16.dynamics.com"),
            ApplicationId = Guid.NewGuid().ToString("D"),
            CertificateFilePath = "C:\\temp",
            CertificatePassword = "my-password"
        };
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<SecretConnection>
            {
                secretConnection
            }
        };

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);

        Action action = () => _secretProvider.GetConnectionSecret(connectionName);

        action.Should().ThrowExactly<ArgumentOutOfRangeException>()
            .WithParameterName(nameof(SecretConnection.Name))
            .Where(exception => exception.Message.StartsWith(ErrorMessages.InvalidConnection(connectionName)));
    }
    
    [Fact]
    public void ShouldThrowOnMissingSecret()
    {
        const string connectionName = "my-connection";
        var secretConnection = new SecretConnection
        {
            Name = connectionName,
            EnvironmentUrl = new Uri("https://con.crm16.dynamics.com"),
            ApplicationId = Guid.NewGuid().ToString("D"),
            TenantId = Guid.NewGuid().ToString("D"),
            CertificateFilePath = "C:\\temp",
        };
        var wallet = new ConnectionWallet
        {
            ExistingConnections = new HashSet<SecretConnection>
            {
                secretConnection
            }
        };

        A.CallTo(() => _walletFileService.ReadWallet())
            .Returns(wallet);

        Action action = () => _secretProvider.GetConnectionSecret(connectionName);

        action.Should().ThrowExactly<InvalidOperationException>()
            .Where(exception => exception.Message.StartsWith(ErrorMessages.MissingSecret(connectionName)));
    }
}