namespace Empowered.Dataverse.Connection.Store.Contracts;

public enum ConnectionType
{
    UserPassword,
    ClientCertificate,
    ClientSecret,
    Unknown,
    Interactive,
    DeviceCode,
    ManagedIdentity,
    AzureDefault,
    AzureCli,
    AzureDeveloperCli,
    AzurePowershell,
    VisualStudio,
    VisualStudioCode
}