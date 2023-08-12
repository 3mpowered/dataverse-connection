using System.Diagnostics.CodeAnalysis;
using System.Security;
using Empowered.Dataverse.Connection.Store.Contracts;
using Empowered.Dataverse.Connection.Store.ErrorHandling;

namespace Empowered.Dataverse.Connection.Store.Model;

internal class SecretConnection : AbstractConnection
{
    public SecretConnection()
    {
    }

    [SetsRequiredMembers]
    public SecretConnection(IConnection connection, string secret)
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

    public sealed override required string Name { get; init; }
    public sealed override required Uri EnvironmentUrl { get; init; }
    public sealed override string? TenantId { get; init; }
    public sealed override string? ApplicationId { get; init; }
    public string? ClientSecret { get; init; }
    public sealed override string? CertificateFilePath { get; init; }
    public string? CertificatePassword { get; init; }
    public sealed override string? UserName { get; init; }
    public string? Password { get; init; }

    public PublicConnection ToPublicConnection()
    {
        return new PublicConnection(Name, EnvironmentUrl)
        {
            ApplicationId = ApplicationId,
            UserName = UserName,
            TenantId = TenantId,
            CertificateFilePath = CertificateFilePath
        };
    }

    public string GetSecret()
    {
        var secret = ConnectionType switch {
            ConnectionType.UserPassword => Password,
            ConnectionType.Certificate => CertificatePassword,
            ConnectionType.ClientSecret => ClientSecret,
            _ => throw new ArgumentOutOfRangeException(nameof(Name), ErrorMessages.InvalidConnection(Name))
        };
        return secret ?? throw new InvalidOperationException(ErrorMessages.MissingSecret(Name));
    }
}