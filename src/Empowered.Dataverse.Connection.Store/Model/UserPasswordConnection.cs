using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Store.Model;

public class UserPasswordConnection : TenantConnection, IUserPasswordConnection
{
    [JsonConstructor]
    public UserPasswordConnection()
    {
        Type = ConnectionType.UserPassword;
    }

    [SetsRequiredMembers]
    public UserPasswordConnection(IUserPasswordConnection connection) : base(connection)
    {
        UserName = connection.UserName;
        Password = connection.Password;
        TenantId = connection.TenantId;
    }

    [SetsRequiredMembers]
    public UserPasswordConnection(string name, Uri environmentUrl, string userName, string password, string tenantId) : base(name, environmentUrl, tenantId)
    {
        Type = ConnectionType.UserPassword;
        Password = password;
        TenantId = tenantId;
        UserName = userName;
    }

    public required string UserName { get; init; }
    public required string Password { get; init; }

    public override IBaseConnection Clone() => new UserPasswordConnection(this);
}