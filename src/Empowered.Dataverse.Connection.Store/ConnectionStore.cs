using Empowered.Dataverse.Connection.Store.Contracts;
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

    public IDataverseConnection Get(string name)
    {
        _logger.LogTrace("Getting connection with name {ConnectionName}", name);

        if (TryGet(name, out IDataverseConnection? connection) && connection != null)
        {
            _logger.LogTrace("Returning connection with name {ConnectionName}", name);
            return connection;
        }

        _logger.LogWarning("Connection with name {ConnectionName} not found", name);
        throw new ArgumentException(ErrorMessages.ConnectionNotFound(name), nameof(name));
    }

    public bool TryGet(string name, out IDataverseConnection? connection)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            _logger.LogWarning("Argument with name {ArgumentName} is null", nameof(name));
            throw new ArgumentNullException(nameof(name));
        }

        var wallet = _walletFileService.ReadWallet();

        var isExisting = TryGetConnectionFromWallet(wallet, name, out IDataverseConnection? existingConnection);
        connection = existingConnection ?? default;

        _logger.LogWarning("Try getting connection with name {ConnectionName} --> exists: {ConnectionExists}", name, isExisting);
        return isExisting;
    }

    public IDataverseConnection GetActive()
    {
        _logger.LogTrace("Getting active connection");
        if (TryGetActive(out var connection) && connection != null)
        {
            return connection;
        }

        _logger.LogWarning("Active connection not found");
        throw new InvalidOperationException(ErrorMessages.NoActiveConnection);
    }

    public bool TryGetActive(out IDataverseConnection? connection)
    {
        var wallet = _walletFileService.ReadWallet();
        connection = wallet.Current;

        var hasActiveConnection = wallet.Current != null;
        _logger.LogWarning("Try getting active connection --> exists: {ConnectionExists}", hasActiveConnection);
        return hasActiveConnection;
    }


    public void Upsert(IDataverseConnection connection, bool useConnection = false)
    {
        var wallet = _walletFileService.ReadWallet();

        if (connection.Type == ConnectionType.Unknown)
        {
            _logger.LogWarning("Connection {ConnectionName} has invalid connection type", connection.Name);
            throw new ArgumentException(ErrorMessages.InvalidConnection(connection.Name), nameof(connection));
        }

        var newConnection = new DataverseConnection(connection);

        if (TryGet(connection.Name, out _))
        {
            _logger.LogTrace("Connection with name {ConnectionName} already exists -> delete existing connection and recreate it", connection.Name);
            var isCurrentConnection = wallet.Current?.Name == connection.Name || useConnection;
            Delete(connection.Name);
            Upsert(connection, isCurrentConnection);
            return;
        }

        _logger.LogTrace("Create connection with name {ConnectionName} and connection type {ConnectionType}", connection.Name,
            connection.Type);
        wallet.ExistingConnections.Add(newConnection);

        if (useConnection)
        {
            _logger.LogTrace("Set created connection with name {ConnectionName} as current connection", connection.Name);
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
        wallet.ExistingConnections = new HashSet<DataverseConnection>();
        wallet.CurrentConnection = null;
        _walletFileService.WriteWallet(wallet);
        _logger.LogTrace("Purged all connections from wallet on {Timestamp}", wallet.TimeStamp);
    }

    public void Use(string name)
    {
        if (TryUse(name))
        {
            return;
        }

        _logger.LogWarning("Couldn't find connection with name {ConnectionName} to be used", name);
        throw new ArgumentException(ErrorMessages.ConnectionNotFound(name), nameof(name));
    }

    public bool TryUse(string name)
    {
        var wallet = _walletFileService.ReadWallet();

        if (!TryGetConnectionFromWallet(wallet, name, out var connection))
        {
            _logger.LogTrace("Couldn't find connection with name {ConnectionName} to be used", name);
            return false;
        }

        if (connection != null)
        {
            wallet.CurrentConnection = new DataverseConnection(connection);
        }

        _walletFileService.WriteWallet(wallet);

        _logger.LogTrace("Set connection with name {ConnectionName} as current connection", name);
        return true;
    }

    private static bool TryGetConnectionFromWallet(ConnectionWallet wallet, string name, out IDataverseConnection? connection)
    {
        connection = wallet.ExistingConnections.FirstOrDefault(x => x.Name == name);

        return connection != null;
    }
}