using Empowered.Dataverse.Connection.Client.Settings;
using Empowered.Dataverse.Connection.Store.Contracts;
using Empowered.Dataverse.Connection.Store.Factories;
using Microsoft.Extensions.Configuration;

namespace Empowered.Dataverse.Connection.Client.Configuration;

public class ConnectionConfigurationProvider : ConfigurationProvider
{
    private readonly string? _connectionName;
    private readonly IConnectionStore _connectionStore;
    private readonly IConnectionSecretProvider _connectionSecretProvider;

    public ConnectionConfigurationProvider(string? connectionName)
    {
        _connectionName = connectionName;
        _connectionStore = ConnectionStoreFactory.Get();
        _connectionSecretProvider = ConnectionSecretProviderFactory.Get();
    }

    public override void Load()
    {
        var connection = string.IsNullOrWhiteSpace(_connectionName)
            ? _connectionStore.GetActive()
            : _connectionStore.Get(_connectionName);
        
        var secret = _connectionSecretProvider.GetConnectionSecret(connection.Name);
        var secretKey = connection.ConnectionType switch
        {
            ConnectionType.UserPassword => $"{DataverseClientOptions.Section}__{nameof(DataverseClientOptions.Password)}",
            ConnectionType.Certificate => $"{DataverseClientOptions.Section}__{nameof(DataverseClientOptions.CertificatePassword)}",
            ConnectionType.ClientSecret => $"{DataverseClientOptions.Section}__{nameof(DataverseClientOptions.ClientSecret)}",
            _ => throw new ArgumentOutOfRangeException()
        };

        Data.Add($"{DataverseClientOptions.Section}__{nameof(DataverseClientOptions.Name)}", connection.Name);
        Data.Add($"{DataverseClientOptions.Section}__{nameof(DataverseClientOptions.UserName)}", connection.UserName);
        Data.Add($"{DataverseClientOptions.Section}__{nameof(DataverseClientOptions.ApplicationId)}", connection.ApplicationId);
        Data.Add($"{DataverseClientOptions.Section}__{nameof(DataverseClientOptions.TenantId)}", connection.TenantId);
        Data.Add($"{DataverseClientOptions.Section}__{nameof(DataverseClientOptions.CertificateFilePath)}", connection.CertificateFilePath);
        Data.Add($"{DataverseClientOptions.Section}__{nameof(DataverseClientOptions.ConnectionType)}", connection.ConnectionType.ToString());
        Data.Add(secretKey, secret);
        
        base.Load();
    }
}