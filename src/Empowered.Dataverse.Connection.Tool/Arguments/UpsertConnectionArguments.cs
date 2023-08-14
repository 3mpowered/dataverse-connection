using System.ComponentModel.DataAnnotations;
using CommandDotNet;
using Empowered.Dataverse.Connection.Store.Contracts;
using Empowered.Dataverse.Connection.Store.Model;

namespace Empowered.Dataverse.Connection.Tool.Arguments;

public class UpsertConnectionArguments : IArgumentModel, IValidatableObject
{
    public required ConnectionNameArguments ConnectionNameArguments { get; set; }
    public required ConnectionArguments ConnectionArguments { get; set; }

    [Option(Description = "Test the Dataverse connection after upsertion")]
    public bool TestConnection { get; set; } = true;

    private ConnectionType ConnectionType
    {
        get
        {
            if (IsClientSecret())
            {
                return ConnectionType.ClientSecret;
            }

            if (IsCertificate())
            {
                return ConnectionType.ClientCertificate;
            }

            if (IsUserPassword())
            {
                return ConnectionType.UserPassword;
            }

            if (IsInteractive())
            {
                return ConnectionType.Interactive;
            }

            if (IsDeviceCode())
            {
                return ConnectionType.DeviceCode;
            }

            return ConnectionType.Unknown;
        }
    }


    public IBaseConnection ToConnection()
    {
        var name = ConnectionNameArguments.Name;
        var url = ConnectionArguments.Url;

        return ConnectionType switch
        {
            ConnectionType.UserPassword => new UserPasswordConnection(
                name,
                url,
                ConnectionArguments.Username!,
                ConnectionArguments.Password!.GetPassword(),
                ConnectionArguments.TenantId!
            ),
            ConnectionType.ClientCertificate => new ClientCertificateConnection(
                name,
                url,
                ConnectionArguments.ApplicationId!,
                ConnectionArguments.TenantId!,
                ConnectionArguments.CertificateFilePath!.FullName,
                ConnectionArguments.CertificatePassword!.GetPassword()
            ),
            ConnectionType.ClientSecret => new ClientSecretConnection(
                name,
                url,
                ConnectionArguments.ApplicationId!,
                ConnectionArguments.TenantId!,
                ConnectionArguments.ClientSecret!.GetPassword()
            ),
            ConnectionType.Interactive => new InteractiveConnection(
                name,
                url
            ),
            ConnectionType.DeviceCode => new DeviceCodeConnection(name, url),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ConnectionArguments is { DeviceCode: true, Interactive: true })
        {
            yield return new ValidationResult("--device-code and --interactive option are mutually exclusive");
        }
        if (ConnectionType == ConnectionType.Unknown)
        {
            yield return new ValidationResult("Invalid combination of connection arguments");
        }
    }

    private bool IsDeviceCode()
    {
        return ConnectionArguments.DeviceCode &&
               !IsClientSecret() &&
               !IsCertificate() &&
               !IsInteractive() &&
               !IsUserPassword();
    }

    private bool IsInteractive()
    {
        return ConnectionArguments.Interactive &&
               !IsDeviceCode() &&
               !IsUserPassword() &&
               !IsCertificate() &&
               !IsClientSecret();
    }

    private bool IsUserPassword()
    {
        return !string.IsNullOrWhiteSpace(ConnectionArguments.Username) &&
               !string.IsNullOrWhiteSpace(ConnectionArguments.TenantId) &&
               ConnectionArguments.Password != null &&
               !IsInteractive() &&
               !IsDeviceCode() &&
               !IsCertificate() &&
               !IsClientSecret();
    }

    private bool IsCertificate()
    {
        return ConnectionArguments.CertificatePassword != null &&
               ConnectionArguments.CertificateFilePath != null &&
               !string.IsNullOrWhiteSpace(ConnectionArguments.TenantId) &&
               !string.IsNullOrWhiteSpace(ConnectionArguments.ApplicationId) &&
               !IsClientSecret() &&
               !IsInteractive() &&
               !IsDeviceCode() &&
               !IsUserPassword();
    }

    private bool IsClientSecret()
    {
        return ConnectionArguments.ClientSecret != null &&
               !string.IsNullOrWhiteSpace(ConnectionArguments.ApplicationId) &&
               !string.IsNullOrWhiteSpace(ConnectionArguments.TenantId) &&
               !IsCertificate() &&
               !IsUserPassword() &&
               !IsInteractive();
    }
}