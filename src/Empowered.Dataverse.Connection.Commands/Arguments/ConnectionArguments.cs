using CommandDotNet;
using Empowered.Dataverse.Connection.Commands.Constants;
using Empowered.Dataverse.Connection.Store.Contracts;
using Empowered.Dataverse.Connection.Store.Model;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Empowered.Dataverse.Connection.Commands.Arguments;

public class ConnectionArguments : IArgumentModel, IDataverseConnection
{
    [Option(Description = "Test the Dataverse connection after upsertion")]
    public bool SkipConnectionTest { get; init; }
    
    [Option(Description = "The name to identify the connection")]
    [EnvVar(ConfigurationKeys.ConnectionName)]
    public required string Name { get; init; }
    
    [Option(Description = "Specify the connection type to authenticate against the Dataverse environment")]
    public ConnectionType Type { get; init; }
    
    [Option(Description = "The URL of the Dataverse environment")]
    [EnvVar(ConfigurationKeys.EnvironmentUrl)]
    public required Uri EnvironmentUrl { get; init; }

    [Option(Description = "The username to authenticate against the Dataverse environment")]
    [EnvVar(ConfigurationKeys.Username)]
    public string? UserName { get; init; }

    [Option(Description = "The passwort to authenticate against the Dataverse environment")]
    [EnvVar(ConfigurationKeys.Password)]
    public string? Password { get; init; }

    [Option(Description = "The application id to authenticate against the Dataverse environment")]
    [EnvVar(ConfigurationKeys.ApplicationId)]
    public string? ApplicationId { get; init; }

    [Option(Description = "The client secret to authenticate against the Dataverse environment")]
    [EnvVar(ConfigurationKeys.ClientSecret)]
    public string? ClientSecret { get; init; }

    [Option(Description = "The tenant id to authenticate against the Dataverse environment")]
    [EnvVar(ConfigurationKeys.TenantId)]
    public string? TenantId { get; init; }

    [Option(Description = "The absolute path to the certificate to authenticate against the Dataverse environment")]
    [EnvVar(ConfigurationKeys.CertificateFilePath)]
    public string? CertificateFilePath { get; init; }

    [Option(Description = "The certificate password to authenticate against the Dataverse environment")]
    [EnvVar(ConfigurationKeys.CertificatePassword)]
    public string? CertificatePassword { get; init; }

    public IDataverseConnection Clone() => new DataverseConnection(this);
    
}