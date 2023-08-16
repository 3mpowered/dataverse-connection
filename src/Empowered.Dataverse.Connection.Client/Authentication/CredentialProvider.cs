using System.Security.Cryptography.X509Certificates;
using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Identity;
using Empowered.Dataverse.Connection.Client.Constants;
using Empowered.Dataverse.Connection.Client.Contracts;
using Empowered.Dataverse.Connection.Client.Settings;
using Empowered.Dataverse.Connection.Store.Contracts;
using Microsoft.Extensions.Options;

namespace Empowered.Dataverse.Connection.Client.Authentication;

public class CredentialProvider : ICredentialProvider
{
    private readonly DataverseClientOptions _clientOptions;

    public CredentialProvider(IOptions<DataverseClientOptions> options) : this(options.Value)
    {
    }

    public CredentialProvider(DataverseClientOptions options)
    {
        _clientOptions = options;
    }

    public TokenCredential GetCredential()
    {
        return _clientOptions.Type switch
        {
            ConnectionType.UserPassword => new UsernamePasswordCredential(
                _clientOptions.UserName,
                _clientOptions.Password,
                _clientOptions.TenantId,
                ConnectionDefaults.DefaultAppId
            ),
            ConnectionType.ClientCertificate => new ClientCertificateCredential(
                _clientOptions.TenantId,
                _clientOptions.ApplicationId,
                new X509Certificate2(_clientOptions.CertificateFilePath!, _clientOptions.CertificatePassword)
            ),
            ConnectionType.ClientSecret => new ClientSecretCredential(
                _clientOptions.TenantId,
                _clientOptions.ApplicationId,
                _clientOptions.ClientSecret
            ),
            ConnectionType.Interactive => new InteractiveBrowserCredential(),
            ConnectionType.Unknown => new InteractiveBrowserCredential(),
            ConnectionType.DeviceCode => new DeviceCodeCredential(new DeviceCodeCredentialOptions
            {
                TokenCachePersistenceOptions = new TokenCachePersistenceOptions
                {
                    Name = $"{_clientOptions.Name}_{_clientOptions.Type}"
                },
                DisableAutomaticAuthentication = false,
                DisableInstanceDiscovery = false,
                RetryPolicy = new RetryPolicy(1),
            }),
            ConnectionType.ManagedIdentity => GetManagedIdentityCredential(),
            ConnectionType.AzureDefault => !string.IsNullOrWhiteSpace(_clientOptions.ApplicationId)
                ? new DefaultAzureCredential(new DefaultAzureCredentialOptions
                    {
                        ManagedIdentityClientId = _clientOptions.ApplicationId
                    }
                )
                : new DefaultAzureCredential(),
            ConnectionType.AzureCli => new AzureCliCredential(),
            ConnectionType.AzureDeveloperCli => new AzureDeveloperCliCredential(),
            ConnectionType.AzurePowershell => new AzurePowerShellCredential(),
            ConnectionType.VisualStudio => new VisualStudioCredential(),
            ConnectionType.VisualStudioCode => new VisualStudioCodeCredential(),
            _ => throw new ArgumentOutOfRangeException($"Unknown connection type {_clientOptions.Type}")
        };
    }

    private ManagedIdentityCredential GetManagedIdentityCredential()
    {
        // TODO: Implement resource identifier
        return new ManagedIdentityCredential(_clientOptions.ApplicationId);
    }
}