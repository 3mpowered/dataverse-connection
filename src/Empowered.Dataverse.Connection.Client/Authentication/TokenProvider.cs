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
    private readonly ICredentialProvider _credentialProvider;
    private readonly ILogger<TokenProvider> _logger;
    private readonly DataverseClientOptions _clientOptions;

    public static readonly TimeSpan CacheTreshold = TimeSpan.FromMinutes(5);

    public TokenProvider(IOptions<DataverseClientOptions> options, IMemoryCache cache, ICredentialProvider credentialProvider,
        ILogger<TokenProvider> logger) : this(options.Value, cache, credentialProvider, logger)
    {
    }

    public TokenProvider(DataverseClientOptions options, IMemoryCache cache, ICredentialProvider credentialProvider, ILogger<TokenProvider> logger)
    {
        _cache = cache;
        _credentialProvider = credentialProvider;
        _logger = logger;
        _clientOptions = options;
    }

    public async Task<string> GetToken(string environmentUri)
    {
        var instanceUrl = new Uri(environmentUri);
        var scope = $"{instanceUrl.Scheme}://{instanceUrl.Authority}/.default";
        _logger.LogTrace("Retrieving token for environment {InstanceUrl} with scope {Scope}", environmentUri, scope);

        if (_cache.TryGetValue<AccessToken>($"{_clientOptions.Type}-{scope}", out var token))
        {
            _logger.LogTrace("Getting cached token expiring {ExpirationDate}", token.ExpiresOn);
            return token.Token;
        }

        var credential = _credentialProvider.GetCredential();
        _logger.LogTrace("Authenticating with credential {CredentialType} for connection type {ConnectionType}", credential.GetType(),
            _clientOptions.Type);

        token = await credential.GetTokenAsync(new TokenRequestContext(new[] { scope }), new CancellationToken());
        var cacheExpirationDate = token.ExpiresOn - CacheTreshold;
        _cache.Set(scope, token, cacheExpirationDate);
        _logger.LogTrace("Caching token with expiration date {ExpirationDate} until {CacheExpiration}", token.ExpiresOn, cacheExpirationDate);

        return token.Token;
    }
}