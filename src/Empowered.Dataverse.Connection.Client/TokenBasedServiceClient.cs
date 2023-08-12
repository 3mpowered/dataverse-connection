using Empowered.Dataverse.Connection.Client.Contracts;
using Empowered.Dataverse.Connection.Client.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.PowerPlatform.Dataverse.Client.Utils;

namespace Empowered.Dataverse.Connection.Client;

public class TokenBasedServiceClient : ServiceClient
{
    public TokenBasedServiceClient(ITokenProvider tokenProvider, Uri environmentUrl, ILogger<TokenBasedServiceClient> logger) 
        : base(environmentUrl, tokenProvider.GetToken, logger: logger)
    {
        if (LastException != null)
        {
            throw new DataverseConnectionException($"Initialising Dataverse connection failed: {LastException.Message}", LastException);
        }

        if (!string.IsNullOrWhiteSpace(LastError))
        {
            throw new DataverseConnectionException($"Initialising Dataverse connection failed: {LastError}");
        }
    }
    
    public TokenBasedServiceClient(ITokenProvider tokenProvider, IOptions<DataverseClientOptions> options, ILogger<TokenBasedServiceClient> logger)
        : this(tokenProvider, options.Value.EnvironmentUrl, logger: logger)
    {
    
    }
}