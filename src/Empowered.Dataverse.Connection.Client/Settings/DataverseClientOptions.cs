using System.Diagnostics.CodeAnalysis;
using Empowered.Dataverse.Connection.Store.Contracts;
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Empowered.Dataverse.Connection.Client.Settings;

public class DataverseClientOptions
{
    public const string Section = "3mpowered__Dataverse";

    public DataverseClientOptions()
    {
    }

    [SetsRequiredMembers]
    public DataverseClientOptions(IBaseConnection connection)
    {
        Name = connection.Name;
        EnvironmentUrl = connection.EnvironmentUrl;
        ConnectionType = connection.Type;

        if (ConnectionType == ConnectionType.ClientCertificate)
        {
            var certificateConnection = connection.ToImplementation<IClientCertificateConnection>();
            ApplicationId = certificateConnection.ApplicationId;
            TenantId = certificateConnection.TenantId;
            CertificateFilePath = certificateConnection.FilePath;
            CertificatePassword = certificateConnection.Password;
        }

        if (ConnectionType == ConnectionType.ClientSecret)
        {
            var clientSecretConnection = connection.ToImplementation<IClientSecretConnection>();
            ApplicationId = clientSecretConnection.ApplicationId;
            TenantId = clientSecretConnection.TenantId;
            ClientSecret = clientSecretConnection.ClientSecret;
        }

        if (ConnectionType == ConnectionType.UserPassword)
        {
            var userPasswordConnection = connection.ToImplementation<IUserPasswordConnection>();
            UserName = userPasswordConnection.UserName;
            Password = userPasswordConnection.Password;
            TenantId = userPasswordConnection.TenantId;
        }

        if (ConnectionType == ConnectionType.ManagedIdentity)
        {
            var managedIdentityConnection = connection.ToImplementation<IManagedIdentityConnection>();
            ApplicationId = managedIdentityConnection.ClientId;
        }
    }

    public required string Name { get; init; }
    public required ConnectionType ConnectionType { get; init; }
    public required Uri EnvironmentUrl { get; init; }
    public string? ApplicationId { get; init; }
    public string? TenantId { get; init; }
    public string? ClientSecret { get; init; }
    public string? CertificateFilePath { get; init; }
    public string? CertificatePassword { get; init; }
    public string? UserName { get; init; }
    public string? Password { get; init; }
}