using System.Security;
using Empowered.Dataverse.Connection.Store.Contracts;
using Empowered.Dataverse.Connection.Store.ErrorHandling;
using Empowered.Dataverse.Connection.Store.Services;

namespace Empowered.Dataverse.Connection.Store;

internal class ConnectionSecretProvider : IConnectionSecretProvider
{
    private readonly IWalletFileService _walletFileService;

    public ConnectionSecretProvider(IWalletFileService walletFileService)
    {
        _walletFileService = walletFileService;
    }
    public string GetConnectionSecret(string name)
    {
        var wallet = _walletFileService.ReadWallet();

        var connection = wallet.ExistingConnections.FirstOrDefault(connection => connection.Name == name);

        if (connection == null)
        {
            throw new ArgumentException(ErrorMessages.ConnectionNotFound(name), nameof(name));
        }

        return connection.GetSecret();
    }
}