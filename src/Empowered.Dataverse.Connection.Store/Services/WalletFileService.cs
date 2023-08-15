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

    public WalletFileService(
        IDataProtectionProvider provider,
        IEnvironmentService environmentService,
        ILogger<WalletFileService> logger
    )
    {
        _environmentService = environmentService;
        _logger = logger;
        _dataProtector = provider.CreateProtector(Application.ConnectionFile);

        var connectionFilePath = environmentService.GetConnectionFilePath();
        _logger.LogTrace("Getting connection file path {Path}", connectionFilePath);

        if (connectionFilePath.Exists)
        {
            _logger.LogTrace("Connection file path {Path} already exists -> try reading wallet file {FileName}", connectionFilePath,
                connectionFilePath.Name);
            try
            {
                _ = ReadWallet();
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, "Reading of existing wallet failed with message: {Message}", exception.Message);
                throw;
            }
        }
        else
        {
            _logger.LogTrace("Initialising new wallet file {FileName} in path {Path}", connectionFilePath.Name, connectionFilePath.FullName);
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
        _logger.LogTrace("Write wallet to path {Path}", connectionFilePath.FullName);
        try
        {
            var serializedWallet = JsonSerializer.Serialize(wallet);
            var unprotectedWallet = Encoding.UTF8.GetBytes(serializedWallet);
            var protectedWallet = _dataProtector.Protect(unprotectedWallet);
            using var fileStream = connectionFilePath.Create();
            fileStream.Write(protectedWallet);
            fileStream.Flush();
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
        _logger.LogTrace("Read wallet from path {Path}", connectionFilePath.FullName);
        try
        {
            var protectedWallet = File.ReadAllBytes(connectionFilePath.FullName);
            var unprotectedWallet = _dataProtector.Unprotect(protectedWallet);
            var wallet = (unprotectedWallet.Length != 0
                ? JsonSerializer.Deserialize<ConnectionWallet>(unprotectedWallet)
                : null) ?? new ConnectionWallet();

            _logger.LogTrace("Read wallet with {Count} connections and current connection {ConnectionName} and timestamp {Timestamp}",
                wallet.Connections.Count(), wallet.Current?.Name, wallet.TimeStamp);

            return wallet;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Reading wallet in {FilePath} failed with error: {Message}", connectionFilePath.FullName, exception.Message);
            throw;
        }
    }
}