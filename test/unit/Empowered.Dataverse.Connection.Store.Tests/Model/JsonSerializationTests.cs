using System.Text.Json;

namespace Empowered.Dataverse.Connection.Store.Model;

public class JsonSerializationTests
{
    [Fact]
    public void ShouldDeserializeWithActualType()
    {
        var interactiveConnection = new InteractiveConnection(
            "interactive connection",
            new Uri("https://a.b.de")
        );
        var clientSecretConnection = new ClientSecretConnection(
            "client secret connection",
            new Uri("https://crm4.dynamics.com"),
            "secret",
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString()
        );
        var clientCertificateConnection = new ClientCertificateConnection(
            "certificate connection",
            new Uri("https://crm4.dynamics.com"),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            "secret",
            "C:\\Temp"
        );
        var userPasswordConnection = new UserPasswordConnection(
            "user password connection",
            new Uri("https://crm4.dynamics.com"),
            "secret",
            "a@b.de",
            Guid.NewGuid().ToString("D")
        );
        var deviceCodeConnection = new DeviceCodeConnection(
            "device code connection",
            new Uri("https://a.b.de")
        );

        var cons = new HashSet<BaseConnection>
        {
            interactiveConnection,
            clientSecretConnection,
            clientCertificateConnection,
            userPasswordConnection,
            deviceCodeConnection
        };

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(cons, jsonSerializerOptions);
        var connections = JsonSerializer.Deserialize<IEnumerable<BaseConnection>>(json, jsonSerializerOptions).ToList();

        connections.Should().NotBeNull();
        
        connections
            .OfType<InteractiveConnection>()
            .Should()
            .ContainSingle()
            .And.Subject
            .Single()
            .Should()
            .BeEquivalentTo(interactiveConnection);
        
        connections
            .OfType<DeviceCodeConnection>()
            .Should()
            .ContainSingle()
            .And.Subject
            .Single()
            .Should()
            .BeEquivalentTo(deviceCodeConnection);
        
        connections
            .OfType<ClientCertificateConnection>()
            .Should()
            .ContainSingle()
            .And.Subject
            .Single()
            .Should()
            .BeEquivalentTo(clientCertificateConnection);
        
        connections
            .OfType<ClientSecretConnection>()
            .Should()
            .ContainSingle()
            .And.Subject
            .Single()
            .Should()
            .BeEquivalentTo(clientSecretConnection);
        
        connections
            .OfType<UserPasswordConnection>()
            .Should()
            .ContainSingle()
            .And.Subject
            .Single()
            .Should()
            .BeEquivalentTo(userPasswordConnection);
    }
}