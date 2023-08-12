
using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Client.Settings;

public class DataverseClientOptions
{
    public const string Section = "3mpowered__Dataverse";

    public DataverseClientOptions()
    {
        
    }

    public DataverseClientOptions(IConnection connection, string connectionSecret)
    {
        Name = connection.Name;
        ConnectionType = connection.ConnectionType;
        ApplicationId = connection.ApplicationId ?? string.Empty;
        TenantId = connection.TenantId ?? string.Empty;
        EnvironmentUrl = connection.EnvironmentUrl;
        UserName = connection.UserName ?? string.Empty;
        CertificateFilePath = connection.CertificateFilePath ?? string.Empty;
        ClientSecret = ConnectionType == ConnectionType.ClientSecret ? connectionSecret : string.Empty;
        CertificatePassword = ConnectionType == ConnectionType.Certificate ? connectionSecret : string.Empty;
        Password = ConnectionType == ConnectionType.UserPassword ? connectionSecret : string.Empty;
    }

    public string Name { get; set; }
    public ConnectionType ConnectionType { get; set; }
    public Uri EnvironmentUrl { get; set; }
    public string ApplicationId { get; set; }
    public string TenantId { get; set; }
    public string ClientSecret { get; set; }
    public string CertificateFilePath { get; set; }
    public string CertificatePassword { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}