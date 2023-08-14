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
public class BaseConnection : IBaseConnection
{
    [JsonConstructor]
    internal BaseConnection()
    {
    }
    
    [SetsRequiredMembers]
    internal BaseConnection(string name, Uri environmentUrl)
    {
        Name = name;
        EnvironmentUrl = environmentUrl;
    }

    [SetsRequiredMembers]
    internal BaseConnection(IBaseConnection connection)
    {
        Name = connection.Name;
        EnvironmentUrl = connection.EnvironmentUrl;
        Type = connection.Type;
    }

    public required string Name { get; init; }
    public required Uri EnvironmentUrl { get; init; }
    public required ConnectionType Type { get; init; } = ConnectionType.Unknown;
    public virtual IBaseConnection Clone() => new BaseConnection(this);
}