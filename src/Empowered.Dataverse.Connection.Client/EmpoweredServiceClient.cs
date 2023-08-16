using Empowered.Dataverse.Connection.Client.Contracts;
using Empowered.Dataverse.Connection.Client.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.PowerPlatform.Dataverse.Client.Utils;

namespace Empowered.Dataverse.Connection.Client;

public class EmpoweredServiceClient : ServiceClient
{
    public EmpoweredServiceClient(ITokenProvider tokenProvider, Uri environmentUrl, ILogger<EmpoweredServiceClient> logger) 
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
    
    public EmpoweredServiceClient(ITokenProvider tokenProvider, IOptions<DataverseClientOptions> options, ILogger<EmpoweredServiceClient> logger)
        : this(tokenProvider, options.Value.EnvironmentUrl, logger: logger)
    {
    
    }
}