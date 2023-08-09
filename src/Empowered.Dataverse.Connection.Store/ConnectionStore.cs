using System.Security;
using Empowered.Dataverse.Connection.Store.Contract;
using Empowered.Dataverse.Connection.Store.ErrorHandling;
using Empowered.Dataverse.Connection.Store.Model;
using Empowered.Dataverse.Connection.Store.Services;
using Microsoft.Extensions.Logging;

namespace Empowered.Dataverse.Connection.Store;

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
        var wallet = _walletFileService.ReadWallet();
        _logger.LogTrace("Listing {Count} connections with current connection {ConnectionName} and timestamp {Timestamp}",
            wallet.Connections.Count(), wallet.Current?.Name, wallet.TimeStamp);
        return wallet;
    }

    public IConnection Get(string name)
    {
        _logger.LogTrace("Getting connection with name {ConnectionName}", name);

        if (TryGet(name, out var connection) && connection != null)
        {
            _logger.LogTrace("Returning connection with name {ConnectionName}", name);
            return connection;
        }

        _logger.LogWarning("Connection with name {ConnectionName} not found", name);
        throw new ArgumentException(ErrorMessages.ConnectionNotFound(name), nameof(name));
    }

    public bool TryGet(string name, out IConnection? connection)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            _logger.LogWarning("Argument with name {ArgumentName} is null", nameof(name));
            throw new ArgumentNullException(nameof(name));
        }

        var wallet = _walletFileService.ReadWallet();

        var isExisting = TryGetConnectionFromWallet(wallet, name, out var existingConnection);
        connection = existingConnection;

        _logger.LogWarning("Try getting connection with name {ConnectionName} --> exists: {ConnectionExists}", name, isExisting);
        return isExisting;
    }

    public void Upsert(IConnection connection, SecureString secret, bool useConnection = false)
    {
        var wallet = _walletFileService.ReadWallet();

        var newConnection = new Model.Connection(connection, secret);

        if (newConnection.ConnectionType == ConnectionType.Unknown)
        {
            _logger.LogWarning("Connection {ConnectionName} has invalid connection type", connection.Name);
            throw new ArgumentException(ErrorMessages.InvalidConnection(connection), nameof(connection));
        }

        if (TryGet(connection.Name, out _))
        {
            _logger.LogTrace("Connection with name {ConnectionName} already exists -> delete existing connection and recreate it", connection.Name);
            var isCurrentConnection = wallet.Current?.Name == connection.Name || useConnection;
            Delete(connection.Name);
            Upsert(connection, secret, isCurrentConnection);
            return;
        }

        _logger.LogTrace("Create connection with name {ConnectionName} and connection type {ConnectionType}", newConnection.Name, newConnection.ConnectionType);
        wallet.ExistingConnections.Add(newConnection);

        if (useConnection)
        {
            _logger.LogTrace("Set created connection with name {ConnectionName} as current connection", newConnection.Name);
            wallet.CurrentConnection = newConnection;
        }

        _walletFileService.WriteWallet(wallet);
    }

    public void Delete(string name)
    {
        if (!TryDelete(name))
        {
            _logger.LogWarning("Connection with name {ConnectionName} doesn't exist", name);
            throw new ArgumentException(ErrorMessages.ConnectionNotFound(name), nameof(name));
        }
    }

    public bool TryDelete(string name)
    {
        var wallet = _walletFileService.ReadWallet();
        if (!TryGetConnectionFromWallet(wallet, name, out _))
        {
            _logger.LogTrace("Connection with name {ConnectionName} doesn't exist", name);
            return false;
        }

        wallet.ExistingConnections = wallet.ExistingConnections
            .Where(x => x.Name != name)
            .ToHashSet();

        if (wallet.CurrentConnection?.Name == name)
        {
            _logger.LogTrace("Removing to be deleted connection {ConnectionName} as current connection", name);
            wallet.CurrentConnection = null;
        }

        _walletFileService.WriteWallet(wallet);
        _logger.LogTrace("Deleted connection with name {ConnectionName}", name);
        
        return true;
    }

    public void Purge()
    {
        var wallet = _walletFileService.ReadWallet();
        wallet.ExistingConnections = new HashSet<Model.Connection>();
        wallet.CurrentConnection = null;
        _walletFileService.WriteWallet(wallet);
        _logger.LogTrace("Purged all connections from wallet on {Timestamp}", wallet.TimeStamp);
    }

    public void Use(string name)
    {
        if (!TryUse(name))
        {
            _logger.LogWarning("Couldn't find connection with name {ConnectionName} to be used", name);
            throw new ArgumentException(ErrorMessages.ConnectionNotFound(name), nameof(name));
        }
    }

    public bool TryUse(string name)
    {
        var wallet = _walletFileService.ReadWallet();

        if (!TryGetConnectionFromWallet(wallet, name, out var connection))
        {
            _logger.LogTrace("Couldn't find connection with name {ConnectionName} to be used", name);
            return false;
        }

        wallet.CurrentConnection = connection;

        _walletFileService.WriteWallet(wallet);
        
        _logger.LogTrace("Set connection with name {ConnectionName} as current connection", name);
        return true;
    }

    private static bool TryGetConnectionFromWallet(ConnectionWallet wallet, string name, out Model.Connection? connection)
    {
        connection = wallet.ExistingConnections.FirstOrDefault(x => x.Name == name);

        return connection != null;
    }
}