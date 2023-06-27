using System.Security;
using Empowered.Dataverse.Connection.Store.Contract;
using Empowered.Dataverse.Connection.Store.Services;
using Microsoft.Extensions.Logging;

namespace Empowered.Dataverse.Connection.Store.Store;

internal class ConnectionStore : IConnectionStore
{
    private readonly IWalletFileService _walletFileService;
    private readonly ILogger<IConnectionStore> _logger;

    internal ConnectionStore(IWalletFileService walletFileService, ILogger<ConnectionStore> logger)
    {
        _walletFileService = walletFileService;
        _logger = logger;
    }

    public IConnectionWallet List()
    {
        throw new NotImplementedException();
    }

    public IConnection Get(string name)
    {
        throw new NotImplementedException();
    }

    public bool TryGet(string name, out IConnection? connection)
    {
        throw new NotImplementedException();
    }

    public void Upsert(IConnection connection, SecureString secret)
    {
        throw new NotImplementedException();
    }

    public void Delete(string name)
    {
        throw new NotImplementedException();
    }

    public bool TryDelete(string name)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public void Use(string name)
    {
        throw new NotImplementedException();
    }

    public bool TryUse(string name)
    {
        throw new NotImplementedException();
    }
}