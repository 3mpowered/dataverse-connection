using System.Security.Cryptography.X509Certificates;
using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Identity;
using Empowered.Dataverse.Connection.Client.Contracts;
using Empowered.Dataverse.Connection.Client.Settings;
using Empowered.Dataverse.Connection.Store.Contracts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Empowered.Dataverse.Connection.Client.Authentication;

public class TokenProvider : ITokenProvider
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<TokenProvider> _logger;
    private readonly DataverseClientOptions _clientOptions;

    public static readonly TimeSpan CacheTreshold = TimeSpan.FromMinutes(5);
    
    public TokenProvider(IOptions<DataverseClientOptions> connectionSettings, IMemoryCache cache, ILogger<TokenProvider> logger)
    {
        _cache = cache;
        _logger = logger;
        _clientOptions = connectionSettings.Value;
    }

    public TokenProvider(DataverseClientOptions options, IMemoryCache cache, ILogger<TokenProvider> logger)
    {
        _cache = cache;
        _logger = logger;
        _clientOptions = options;
    }
    
    public async Task<string> GetToken(string environmentUri)
    {
        var instanceUrl = new Uri(environmentUri);
        var scope = $"{instanceUrl.Scheme}://{instanceUrl.Authority}/.default";

        if (_cache.TryGetValue<AccessToken>($"{_clientOptions.ConnectionType}-{scope}", out var token))
        {
            return token.Token;
        }

        var credential = GetTokenCredential();

        token = await credential.GetTokenAsync(new TokenRequestContext(new[] { scope }), new CancellationToken());
        _cache.Set(scope, token, token.ExpiresOn - CacheTreshold);

        return token.Token;
    }

    private TokenCredential GetTokenCredential()
    {
        return _clientOptions.ConnectionType switch
        {
            ConnectionType.UserPassword => new UsernamePasswordCredential(
                _clientOptions.UserName,
                _clientOptions.Password,
                _clientOptions.TenantId,
                "51f81489-12ee-4a9e-aaae-a2591f45987d" // TODO: Add Constant
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
                    Name = $"{_clientOptions.Name}_{_clientOptions.ConnectionType}"
                },
                DisableAutomaticAuthentication = false,
                DisableInstanceDiscovery = false,
                RetryPolicy = new RetryPolicy(1),
            }),
            _ => throw new ArgumentOutOfRangeException($"Unknown connection type {_clientOptions.ConnectionType}")
        };
    }
}