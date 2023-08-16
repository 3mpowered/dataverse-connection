using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Empowered.Dataverse.Connection.Store.Contracts;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Empowered.Dataverse.Connection.Store.Model;

public class DataverseConnection : IDataverseConnection
{
    [JsonConstructor]
    public DataverseConnection()
    {
    }

    [SetsRequiredMembers]
    public DataverseConnection(string name, Uri environmentUrl, ConnectionType type)
    {
        Name = name;
        EnvironmentUrl = environmentUrl;
        Type = type;
    }

    [SetsRequiredMembers]
    public DataverseConnection(IDataverseConnection connection)
    {
        Name = connection.Name;
        EnvironmentUrl = connection.EnvironmentUrl;
        Type = connection.Type;
        ApplicationId = connection.ApplicationId;
        TenantId = connection.TenantId;
        ClientSecret = connection.ClientSecret;
        CertificateFilePath = connection.CertificateFilePath;
        CertificatePassword = connection.CertificatePassword;
        UserName = connection.UserName;
        Password = connection.Password;
    }

    public required string Name { get; init; }
    public required Uri EnvironmentUrl { get; init; }
    public ConnectionType Type { get; init; }
    public string? ApplicationId { get; init; }
    public string? TenantId { get; init; }
    public string? ClientSecret { get; init; }
    public string? CertificateFilePath { get; init; }
    public string? CertificatePassword { get; init; }
    public string? UserName { get; init; }
    public string? Password { get; init; }
    public IDataverseConnection Clone() => new DataverseConnection(this);

    public static DataverseConnection InteractiveConnection(string name, Uri environmentUrl) =>
        new(name, environmentUrl, ConnectionType.Interactive);

    public static DataverseConnection VisualStudioConnection(string name, Uri environmentUrl) =>
        new(name, environmentUrl, ConnectionType.VisualStudio);

    public static DataverseConnection VisualStudioCodeConnection(string name, Uri environmentUrl) =>
        new(name, environmentUrl, ConnectionType.VisualStudioCode);

    public static DataverseConnection AzurePowershellConnection(string name, Uri environmentUrl) =>
        new(name, environmentUrl, ConnectionType.AzurePowershell);

    public static DataverseConnection ClientSecretConnection(string name, Uri environmentUrl, string tenantId, string applicationId,
        string clientSecret) => new(name, environmentUrl, ConnectionType.ClientSecret)
    {
        TenantId = tenantId,
        ApplicationId = applicationId,
        ClientSecret = clientSecret
    };

    public static DataverseConnection ClientCertificateConnection(string name, Uri environmentUrl, string tenantId, string applicationId,
        string certificateFilePath, string certificatePassword) =>
        new(name, environmentUrl, ConnectionType.ClientCertificate)
        {
            TenantId = tenantId,
            ApplicationId = applicationId,
            CertificatePassword = certificatePassword,
            CertificateFilePath = certificateFilePath
        };

    public static DataverseConnection UserPasswordConnection(string name, Uri environmentUrl, string tenantId, string userName, string password) =>
        new(name, environmentUrl, ConnectionType.UserPassword)
        {
            TenantId = tenantId,
            UserName = userName,
            Password = password
        };

    public static DataverseConnection ManagedIdentityConnection(string name, Uri environmentUrl, string? applicationId = null) =>
        new(name, environmentUrl, ConnectionType.ManagedIdentity)
        {
            ApplicationId = applicationId
        };

    public static DataverseConnection AzureDeveloperCliConnection(string name, Uri environmentUrl) =>
        new(name, environmentUrl, ConnectionType.AzureDeveloperCli);

    public static DataverseConnection AzureDefaultConnection(string name, Uri environmentUrl) =>
        new(name, environmentUrl, ConnectionType.AzureDefault);

    public static DataverseConnection AzureCliConnection(string name, Uri environmentUrl) =>
        new(name, environmentUrl, ConnectionType.AzureCli);

    public static DataverseConnection DeviceCodeConnection(string name, Uri environmentUrl) =>
        new(name, environmentUrl, ConnectionType.DeviceCode);
}