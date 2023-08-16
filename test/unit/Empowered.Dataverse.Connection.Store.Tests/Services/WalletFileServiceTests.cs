using System.Reflection;
using System.Security.Cryptography;
using Empowered.Dataverse.Connection.Store.Constants;
using Empowered.Dataverse.Connection.Store.Contracts;
using Empowered.Dataverse.Connection.Store.Model;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Empowered.Dataverse.Connection.Store.Services;

public class WalletFileServiceTests : IDisposable
{
    private readonly IEnvironmentService _environmentService;
    private readonly IDataProtectionProvider _dataProtectionProvider;
    private readonly FileInfo _connectionFilePath;
    private readonly DirectoryInfo _directoryInfo;

    public WalletFileServiceTests()
    {
        var location = new FileInfo(Assembly.GetExecutingAssembly().Location);
        _directoryInfo = new DirectoryInfo(Path.Combine(location.Directory!.FullName, Application.Name));
        if (!_directoryInfo.Exists)
        {
            _directoryInfo.Create();
        }

        _connectionFilePath = new FileInfo(Path.Combine(_directoryInfo.FullName, Application.ConnectionFile));
        _dataProtectionProvider = DataProtectionProvider.Create(Application.Name);
        _environmentService = A.Fake<IEnvironmentService>();
        A.CallTo(() => _environmentService.GetConnectionFilePath())
            .Returns(_connectionFilePath);
    }

    [Fact]
    public void ShouldInitialiseWalletSuccessfully()
    {
        var nullLogger = NullLogger<WalletFileService>.Instance;

        var _ = new WalletFileService(_dataProtectionProvider, _environmentService, nullLogger);

        _connectionFilePath.Exists.Should().BeTrue();
    }

    [Fact]
    public void ShouldInitialiseExistingWalletSuccessfully()
    {
        var nullLogger = NullLogger<WalletFileService>.Instance;


        _ = new WalletFileService(_dataProtectionProvider, _environmentService, nullLogger);
        _ = new WalletFileService(_dataProtectionProvider, _environmentService, nullLogger);

        _connectionFilePath.Exists.Should().BeTrue();
    }
    
    [Fact]
    public void ShouldThrowOnInitializationOfExistingWallet()
    {
        var nullLogger = NullLogger<WalletFileService>.Instance;
        _ = new WalletFileService(_dataProtectionProvider, _environmentService, nullLogger);
        
        var dataProtectionProvider = A.Fake<IDataProtectionProvider>();
        var dataProtector = A.Fake<IDataProtector>();
        A.CallTo(() => dataProtectionProvider.CreateProtector(A<string>._))
            .Returns(dataProtector);
        A.CallTo(() => dataProtector.Unprotect(A<byte[]>._))
            .Throws<CryptographicException>();
        
        var action = () => new WalletFileService(dataProtectionProvider, _environmentService, nullLogger);

        action.Should()
            .ThrowExactly<CryptographicException>();
    }
    
    [Fact]
    public void ShouldWriteAndReadWallet()
    {
        var connection = new DataverseConnection("Test", new Uri("https://test.crm4.dynamics.com"), ConnectionType.UserPassword) 
        {
            UserName = "test@test.com",
            Password = "12345",
            TenantId = Guid.NewGuid().ToString("D")
        };
        var wallet = new ConnectionWallet
        {
            CurrentConnection = connection,
            ExistingConnections = { connection }
        };

        var walletFileService = new WalletFileService(_dataProtectionProvider, _environmentService, NullLogger<WalletFileService>.Instance);
        walletFileService.WriteWallet(wallet);

        var readWallet = walletFileService.ReadWallet();

        readWallet.TimeStamp.Should().Be(wallet.TimeStamp);
        readWallet.Current.Should().BeEquivalentTo(wallet.Current);
        readWallet.Connections.Should().Contain(connection);
    }

    [Fact]
    public void ShouldThrowExceptionWhenWriteWalletFails()
    {
        var dataProtectionProvider = A.Fake<IDataProtectionProvider>();
        var dataProtector = A.Fake<IDataProtector>();
        A.CallTo(() => dataProtectionProvider.CreateProtector(A<string>._))
            .Returns(dataProtector);
        var walletFileService = new WalletFileService(dataProtectionProvider, _environmentService, NullLogger<WalletFileService>.Instance);
        A.CallTo(() => dataProtector.Protect(A<byte[]>._))
            .Throws<CryptographicException>();
        var actor = () => walletFileService.WriteWallet(new ConnectionWallet());

        actor.Should().ThrowExactly<CryptographicException>();
    }

    [Fact]
    public void ShouldThrowExceptionWhenReadWalletFails()
    {
        var dataProtectionProvider = A.Fake<IDataProtectionProvider>();
        var dataProtector = A.Fake<IDataProtector>();
        A.CallTo(() => dataProtectionProvider.CreateProtector(A<string>._))
            .Returns(dataProtector);
        var walletFileService = new WalletFileService(dataProtectionProvider, _environmentService, NullLogger<WalletFileService>.Instance);
        A.CallTo(() => dataProtector.Unprotect(A<byte[]>._))
            .Throws<CryptographicException>();
        var actor = () => walletFileService.ReadWallet();

        actor.Should().ThrowExactly<CryptographicException>();
    }

    public void Dispose()
    {
        if (_connectionFilePath.Exists)
        {
            _connectionFilePath.Delete();
        }

        if (_directoryInfo.Exists)
        {
            _directoryInfo.Delete();
        }
    }
}