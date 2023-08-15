using Empowered.Dataverse.Connection.Store.Contracts;
using Empowered.Dataverse.Connection.Store.ErrorHandling;
using Empowered.Dataverse.Connection.Store.Model;

namespace Empowered.Dataverse.Connection.Store.Services;

internal class ConnectionMapper : IConnectionMapper
{
    public IBaseConnection ToExternal(BaseConnection connection)
    {
        return connection.Clone();
    }

    public BaseConnection ToInternal(IBaseConnection connection)
    {
        // TODO: find generic solution
        return connection switch
        {
            IClientCertificateConnection clientCertificateConnection => new ClientCertificateConnection(clientCertificateConnection),
            IClientSecretConnection clientSecretConnection => new ClientSecretConnection(clientSecretConnection),
            IUserPasswordConnection userPasswordConnection => new UserPasswordConnection(userPasswordConnection),
            IInteractiveConnection interactiveUserConnection => new InteractiveConnection(interactiveUserConnection),
            IDeviceCodeConnection deviceCodeConnection => new DeviceCodeConnection(deviceCodeConnection),
            IManagedIdentityConnection managedIdentityConnection => new ManagedIdentityConnection(managedIdentityConnection),
            IAzureDefaultConnection azureDefaultConnection => new AzureDefaultConnection(azureDefaultConnection),
            IAzureCliConnection azureCliConnection => new AzureCliConnection(azureCliConnection),
            IAzureDeveloperCliConnection azureDeveloperCliConnection => new AzureDeveloperCliConnection(azureDeveloperCliConnection),
            IAzurePowershellConnection azurePowershellConnection => new AzurePowershellConnection(azurePowershellConnection),
            IVisualStudioConnection visualStudioConnection => new VisualStudioConnection(visualStudioConnection),
            IVisualStudioCodeConnection vsCodeConnection => new VisualStudioCodeConnection(vsCodeConnection),
            _ => throw new ArgumentOutOfRangeException(nameof(connection), ErrorMessages.ConnectionOutOfRange(connection.GetType()))
        };
    }
}