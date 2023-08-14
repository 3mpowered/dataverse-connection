using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Store.Model;

public class DeviceCodeConnection : BaseConnection, IDeviceCodeConnection
{
    [JsonConstructor]
    public DeviceCodeConnection()
    {
        Type = ConnectionType.DeviceCode;
    }

    [SetsRequiredMembers]
    public DeviceCodeConnection(IDeviceCodeConnection connection) : base(connection)
    {
    }

    [SetsRequiredMembers]
    public DeviceCodeConnection(string name, Uri environmentUrl) : base(name, environmentUrl)
    {
        Type = ConnectionType.DeviceCode;
    }

    public override IBaseConnection Clone() => new DeviceCodeConnection(this);
}