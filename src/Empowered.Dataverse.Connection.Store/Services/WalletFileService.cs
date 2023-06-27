using System.Text;
using System.Text.Json;
using Empowered.Dataverse.Connection.Store.Constants;
using Empowered.Dataverse.Connection.Store.Model;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;

namespace Empowered.Dataverse.Connection.Store.Services;

internal class WalletFileService : IWalletFileService
{
    private readonly IEnvironmentService _environmentService;
    private readonly ILogger<WalletFileService> _logger;
    private readonly IDataProtector _dataProtector;

    public WalletFileService(IDataProtectionProvider provider, IEnvironmentService environmentService, ILogger<WalletFileService> logger)
    {
        _environmentService = environmentService;
        _logger = logger;
        _dataProtector = provider.CreateProtector(Application.ConnectionFile);


        var connectionFilePath = environmentService.GetConnectionFilePath();

        if (connectionFilePath.Exists)
        {
            try
            {
                ReadWallet();
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "Reading of wallet failed -> instantiate new wallet");
                InitialiseNewWallet();
            }
        }
        else
        {
            InitialiseNewWallet();
        }
    }

    private void InitialiseNewWallet()
    {
        WriteWallet(new ConnectionWallet());
    }

    public void WriteWallet(ConnectionWallet wallet)
    {
        var connectionFilePath = _environmentService.GetConnectionFilePath();
        try
        {
            var unprotectedWallet = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(wallet));
            var protectedWallet = _dataProtector.Protect(unprotectedWallet);
            using var fileStream = connectionFilePath.Create();
            fileStream.Write(protectedWallet);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Writing wallet file to {FilePath} failed with error: {Message}", connectionFilePath.FullName,
                exception.Message);
            throw;
        }
    }

    public ConnectionWallet ReadWallet()
    {
        var connectionFilePath = _environmentService.GetConnectionFilePath();
        try
        {
            var protectedWallet = File.ReadAllBytes(connectionFilePath.FullName);
            var unprotectedWallet = _dataProtector.Unprotect(protectedWallet);
            var wallet = JsonSerializer.Deserialize<ConnectionWallet>(unprotectedWallet);
            return wallet ?? new ConnectionWallet();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Reading wallet in {FilePath} failed with error: {Message}", connectionFilePath.FullName, exception.Message);
            throw;
        }
    }
}