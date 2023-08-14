using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Client.Settings;

public class DataverseClientOptions
{
    public const string Section = "3mpowered__Dataverse";

    public DataverseClientOptions()
    {
    }

    public DataverseClientOptions(IBaseConnection connection)
    {
        Name = connection.Name;
        EnvironmentUrl = connection.EnvironmentUrl;
        ConnectionType = connection.Type;

        if (ConnectionType == ConnectionType.ClientCertificate)
        {
            var certificateConnection = connection.ToConnection<IClientCertificateConnection>();
            ApplicationId = certificateConnection.ApplicationId;
            TenantId = certificateConnection.TenantId;
            CertificateFilePath = certificateConnection.FilePath;
            CertificatePassword = certificateConnection.Password;
        }

        if (ConnectionType == ConnectionType.ClientSecret)
        {
            var clientSecretConnection = connection.ToConnection<IClientSecretConnection>();
            ApplicationId = clientSecretConnection.ApplicationId;
            TenantId = clientSecretConnection.TenantId;
            ClientSecret = clientSecretConnection.ClientSecret;
        }

        if (ConnectionType == ConnectionType.Interactive)
        {
            var interactiveUserConnection = connection.ToConnection<IInteractiveConnection>();
        }

        if (ConnectionType == ConnectionType.UserPassword)
        {
            var userPasswordConnection = connection.ToConnection<IUserPasswordConnection>();
            UserName = userPasswordConnection.UserName;
            Password = userPasswordConnection.Password;
            TenantId = userPasswordConnection.TenantId;
        }
    }

    public string Name { get; set; }
    public ConnectionType ConnectionType { get; set; }
    public Uri EnvironmentUrl { get; set; }
    public string? ApplicationId { get; set; }
    public string? TenantId { get; set; }
    public string? ClientSecret { get; set; }
    public string? CertificateFilePath { get; set; }
    public string? CertificatePassword { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
}