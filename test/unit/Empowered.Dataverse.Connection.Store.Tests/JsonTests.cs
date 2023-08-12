using System.Text.Json;
using System.Text.Json.Serialization;
using Empowered.Dataverse.Connection.Store.Contracts;

namespace Empowered.Dataverse.Connection.Store;

[JsonDerivedType(typeof(UserCon))]
[JsonDerivedType(typeof(AppCon))]
public interface ICon
{
    public string Name { get; set; }
    public string Url { get; set; }

    public ConnectionType ConnectionType { get; }
}

[JsonDerivedType(typeof(UserCon), 0)]
[JsonDerivedType(typeof(AppCon), 1)]
internal  class BaseCon : ICon
{
    // public BaseCon()
    // {
    // }
    //
    // [JsonConstructor]
    // public BaseCon(string name, string url)
    // {
    //     Name = name;
    //     Url = url;
    // }

    public string Name { get; set; }
    public string Url { get; set; }
    
    [JsonIgnore]
    public ConnectionType ConnectionType => throw new NotImplementedException();
}

internal class UserCon : BaseCon
{
    // public UserCon()
    // {
    // }
    //
    // [JsonConstructor]
    // public UserCon(string name, string url, string userName, string password) : base(name, url)
    // {
    //     UserName = userName;
    //     Password = password;
    // }

    public  ConnectionType ConnectionType => ConnectionType.UserPassword;
    public string UserName { get; set; }
    public string Password { get; set; }
}

internal class AppCon : BaseCon
{
    // public AppCon()
    // {
    // }
    //
    // [JsonConstructor]
    // public AppCon(string name, string url, string clientId) : base(name, url)
    // {
    //     ClientId = clientId;
    // }

    public  ConnectionType ConnectionType => ConnectionType.ClientSecret;
    public string ClientId { get; set; }
}

public class JsonTests
{
    [Fact]
    public void ShouldTestSomething()
    {
        var userCon = new UserCon
        {
            Name = "User Connection",
            Url = "https://test.de,",
            UserName = "a@b.de",
            Password = "s3cr3t"
        };

        var appCon = new AppCon
        {
            Name = "App Connection",
            Url = "https://test.de",
            ClientId = Guid.NewGuid().ToString()
        };

        var cons = new  HashSet<BaseCon>
        {
            userCon,
            appCon
        };

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(cons, jsonSerializerOptions);
        var deserializedCons = JsonSerializer.Deserialize<IEnumerable<BaseCon>>(json, jsonSerializerOptions);

        deserializedCons.Should().NotBeNull();
    }
}