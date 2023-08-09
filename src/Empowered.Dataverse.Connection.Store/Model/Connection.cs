using System.Diagnostics.CodeAnalysis;
using System.Security;
using Empowered.Dataverse.Connection.Store.Contract;

namespace Empowered.Dataverse.Connection.Store.Model;

internal class Connection : IConnection
{
    public Connection()
    {
    }

    [SetsRequiredMembers]
    public Connection(IConnection connection, SecureString secret)
    {
        Name = connection.Name;
        EnvironmentUrl = connection.EnvironmentUrl;
        TenantId = connection.TenantId;
        ApplicationId = connection.ApplicationId;
        CertificateFilePath = connection.CertificateFilePath;
        UserName = connection.UserName;

        switch (ConnectionType)
        {
            case ConnectionType.UserPassword:
                Password = secret;
                break;
            case ConnectionType.Certificate:
                CertificatePassword = secret;
                break;
            case ConnectionType.ClientSecret:
                ClientSecret = secret;
                break;
            case ConnectionType.Unknown:
            default:
                break;
        }
    }

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

    public required string Name { get; init; }
    public required Uri EnvironmentUrl { get; init; }
    public string? TenantId { get; init; }
    public string? ApplicationId { get; init; }
    public SecureString? ClientSecret { get; init; }
    public string? CertificateFilePath { get; init; }
    public SecureString? CertificatePassword { get; init; }
    public string? UserName { get; init; }
    public SecureString? Password { get; init; }
}