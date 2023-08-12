using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Store.Model;

public abstract class AbstractConnection : IConnection
{
    public abstract string Name { get; init; }
    public abstract Uri EnvironmentUrl { get; init; }
    public abstract string? TenantId { get; init; }
    public abstract string? ApplicationId { get; init; }
    public abstract string? CertificateFilePath { get; init; }
    public abstract string? UserName { get; init; }
    
    public ConnectionType ConnectionType
    {
        get
        {
            if (IsUserPasswordConnection())
            {
                return ConnectionType.UserPassword;
            }

            if (IsCertificateConnection())
            {
                return ConnectionType.Certificate;
            }

            if (IsClientSecretConnection())
            {
                return ConnectionType.ClientSecret;
            }

            return ConnectionType.Unknown;
        }
    }
    
    private bool IsClientSecretConnection()
    {
        return !string.IsNullOrWhiteSpace(ApplicationId) &&
               !string.IsNullOrWhiteSpace(TenantId) &&
               string.IsNullOrWhiteSpace(CertificateFilePath) &&
               string.IsNullOrWhiteSpace(UserName);
    }

    private bool IsCertificateConnection()
    {
        return !string.IsNullOrWhiteSpace(CertificateFilePath) &&
               !string.IsNullOrWhiteSpace(ApplicationId) &&
               !string.IsNullOrWhiteSpace(TenantId) &&
               string.IsNullOrWhiteSpace(UserName);
    }

    private bool IsUserPasswordConnection()
    {
        return !string.IsNullOrWhiteSpace(UserName) &&
               string.IsNullOrWhiteSpace(ApplicationId) &&
               string.IsNullOrWhiteSpace(TenantId) &&
               string.IsNullOrWhiteSpace(CertificateFilePath);
    }
}