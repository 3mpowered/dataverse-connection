using System.Security.Cryptography.X509Certificates;
using Azure.Core;
using Azure.Identity;
using Empowered.Dataverse.Connection.Client.Contracts;
using Empowered.Dataverse.Connection.Client.Settings;
using Empowered.Dataverse.Connection.Store.Contracts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MemoryCache = System.Runtime.Caching.MemoryCache;

namespace Empowered.Dataverse.Connection.Client.Authentication;

public class TokenProvider : ITokenProvider
{
    private readonly IMemoryCache _cache;
    private readonly DataverseClientOptions _clientOptions;

    public static readonly TimeSpan CacheTreshold = TimeSpan.FromMinutes(5);
    
    public TokenProvider(IOptions<DataverseClientOptions> connectionSettings, IMemoryCache cache)
    {
        _cache = cache;
        _clientOptions = connectionSettings.Value;
    }

    public TokenProvider(DataverseClientOptions options, IMemoryCache cache)
    {
        _cache = cache;
        _clientOptions = options;
    }
    
    public async Task<string> GetToken(string environmentUri)
    {
        TokenCredential credential = _clientOptions.ConnectionType switch
        {
            ConnectionType.UserPassword => GetUserCredential(),
            ConnectionType.Certificate => GetClientCertificateCredential(),
            ConnectionType.ClientSecret => GetClientSecretCredential(),
            ConnectionType.Unknown => GetInteractiveCredential(),
            _ => throw new ArgumentOutOfRangeException()
        };

        var instanceUrl = new Uri(environmentUri);
        var scope = $"{instanceUrl.Scheme}://{instanceUrl.Authority}/.default";

        if (_cache.TryGetValue<AccessToken>(scope, out var token))
        {
            return token.Token;
        }

        token = await credential.GetTokenAsync(new TokenRequestContext(new[] { scope }), new CancellationToken());
        _cache.Set(scope, token, token.ExpiresOn - CacheTreshold);

        return token.Token;
    }

    private static InteractiveBrowserCredential GetInteractiveCredential()
    {
        return new InteractiveBrowserCredential();
    }

    private TokenCredential GetUserCredential()
    {
        return new UsernamePasswordCredential(
            _clientOptions.UserName,
            _clientOptions.Password,
            null,
            "51f81489-12ee-4a9e-aaae-a2591f45987d"
        );
    }

    private ClientSecretCredential GetClientSecretCredential()
    {
        return new ClientSecretCredential(
            _clientOptions.TenantId,
            _clientOptions.ApplicationId,
            _clientOptions.ClientSecret
        );
    }

    private ClientCertificateCredential GetClientCertificateCredential()
    {
        var certificate = new X509Certificate2(_clientOptions.CertificateFilePath, _clientOptions.CertificatePassword);
        return new ClientCertificateCredential(
            _clientOptions.TenantId,
            _clientOptions.ApplicationId,
            certificate
        );
    }
}