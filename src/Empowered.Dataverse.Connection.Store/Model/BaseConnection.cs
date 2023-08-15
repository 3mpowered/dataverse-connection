using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Store.Model;

[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
[JsonDerivedType(typeof(InteractiveConnection), "interactive")]
[JsonDerivedType(typeof(DeviceCodeConnection), "deviceCode")]
[JsonDerivedType(typeof(UserPasswordConnection), "userPassword")]
[JsonDerivedType(typeof(ClientSecretConnection), "clientSecret")]
[JsonDerivedType(typeof(ClientCertificateConnection), "clientCertificate")]
[JsonDerivedType(typeof(ManagedIdentityConnection), "managedIdentity")]
[JsonDerivedType(typeof(AzureDefaultConnection), "defaultAzure")]
[JsonDerivedType(typeof(AzureCliConnection), "azureCli")]
[JsonDerivedType(typeof(AzureDeveloperCliConnection), "azureDeveloperCli")]
[JsonDerivedType(typeof(AzurePowershellConnection), "azurePowershell")]
[JsonDerivedType(typeof(VisualStudioConnection), "visualStudio")]
[JsonDerivedType(typeof(VisualStudioCodeConnection), "visualStudioCode")]
public class BaseConnection : IBaseConnection
{
    [JsonConstructor]
    internal BaseConnection()
    {
        Type = ConnectionType.Unknown;
    }

    [SetsRequiredMembers]
    internal BaseConnection(string name, Uri environmentUrl)
    {
        Name = name;
        EnvironmentUrl = environmentUrl;
        Type = ConnectionType.Unknown;
    }

    [SetsRequiredMembers]
    internal BaseConnection(IBaseConnection connection) : this(connection.Name, connection.EnvironmentUrl)
    {
        Type = connection.Type;
    }

    public required string Name { get; init; }
    public required Uri EnvironmentUrl { get; init; }
    public required ConnectionType Type { get; init; }
    public virtual IBaseConnection Clone() => new BaseConnection(this);
}