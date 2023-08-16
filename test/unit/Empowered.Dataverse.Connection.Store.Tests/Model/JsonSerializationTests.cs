// using System.Text.Json;
// using Empowered.Dataverse.Connection.Store.Contracts;
//
// namespace Empowered.Dataverse.Connection.Store.Model;
//
// public class JsonSerializationTests
// {
//     [Fact]
//     public void ShouldDeserializeWithActualType()
//     {
//         var environmentUrl = new Uri("https://crm4.dynamics.com");
//
//         var clientSecretConnection = new ClientSecretConnection(
//             "client secret connection",
//             environmentUrl,
//             "secret",
//             Guid.NewGuid().ToString(),
//             Guid.NewGuid().ToString()
//         );
//         var clientCertificateConnection = new ClientCertificateConnection(
//             "certificate connection",
//             environmentUrl,
//             Guid.NewGuid().ToString(),
//             Guid.NewGuid().ToString(),
//             "secret",
//             "C:\\Temp"
//         );
//         var userPasswordConnection = new UserPasswordConnection(
//             "user password connection",
//             environmentUrl,
//             "secret",
//             "a@b.de",
//             Guid.NewGuid().ToString("D")
//         );
//         var interactiveConnection = new InteractiveConnection("interactive connection", environmentUrl);
//         var deviceCodeConnection = new DeviceCodeConnection("device code connection", environmentUrl);
//         var managedIdentityConnection = new ManagedIdentityConnection("managed identity connection", environmentUrl, Guid.NewGuid().ToString("D"));
//         var azureCliConnection = new AzureCliConnection("azure cli connection", environmentUrl);
//         var azureDefaultConnection = new AzureDefaultConnection("azure default connection", environmentUrl);
//         var azurePowershellConnection = new AzurePowershellConnection("azure powershell connection", environmentUrl);
//         var visualStudioConnection = new VisualStudioConnection("visual studio connection", environmentUrl);
//         var visualStudioCodeConnection = new VisualStudioCodeConnection("visual studio code connection", environmentUrl);
//         var azureDeveloperCliConnection = new AzureDeveloperCliConnection("azure developer cli connection", environmentUrl);
//
//         var cons = new HashSet<BaseConnection>
//         {
//             interactiveConnection,
//             clientSecretConnection,
//             clientCertificateConnection,
//             userPasswordConnection,
//             deviceCodeConnection,
//             managedIdentityConnection,
//             azureCliConnection,
//             azureDefaultConnection,
//             azurePowershellConnection,
//             visualStudioConnection,
//             visualStudioCodeConnection,
//             azureDeveloperCliConnection
//         };
//
//         var jsonSerializerOptions = new JsonSerializerOptions
//         {
//             WriteIndented = true
//         };
//
//         var json = JsonSerializer.Serialize(cons, jsonSerializerOptions);
//         var connections = JsonSerializer.Deserialize<ICollection<BaseConnection>>(json, jsonSerializerOptions);
//
//         connections.Should().NotBeNull();
//
// #pragma warning disable CS8604 // Possible null reference argument.
//         connections
// #pragma warning restore CS8604 // Possible null reference argument.
//             .OfType<InteractiveConnection>()
//             .Should()
//             .ContainSingle()
//             .And.Subject
//             .Single()
//             .Should()
//             .BeEquivalentTo(interactiveConnection);
//
//         connections
//             .OfType<DeviceCodeConnection>()
//             .Should()
//             .ContainSingle()
//             .And.Subject
//             .Single()
//             .Should()
//             .BeEquivalentTo(deviceCodeConnection);
//
//         connections
//             .OfType<ClientCertificateConnection>()
//             .Should()
//             .ContainSingle()
//             .And.Subject
//             .Single()
//             .Should()
//             .BeEquivalentTo(clientCertificateConnection);
//
//         connections
//             .OfType<ClientSecretConnection>()
//             .Should()
//             .ContainSingle()
//             .And.Subject
//             .Single()
//             .Should()
//             .BeEquivalentTo(clientSecretConnection);
//
//         connections
//             .OfType<UserPasswordConnection>()
//             .Should()
//             .ContainSingle()
//             .And.Subject
//             .Single()
//             .Should()
//             .BeEquivalentTo(userPasswordConnection);
//         
//         connections
//             .OfType<VisualStudioConnection>()
//             .Should()
//             .ContainSingle()
//             .And.Subject
//             .Single()
//             .Should()
//             .BeEquivalentTo(visualStudioConnection);
//         
//         connections
//             .OfType<VisualStudioCodeConnection>()
//             .Should()
//             .ContainSingle()
//             .And.Subject
//             .Single()
//             .Should()
//             .BeEquivalentTo(visualStudioCodeConnection);
//         
//         connections
//             .OfType<AzureDefaultConnection>()
//             .Should()
//             .ContainSingle()
//             .And.Subject
//             .Single()
//             .Should()
//             .BeEquivalentTo(azureDefaultConnection);
//         
//         connections
//             .OfType<IAzureCliConnection>()
//             .Should()
//             .ContainSingle()
//             .And.Subject
//             .Single()
//             .Should()
//             .BeEquivalentTo(azureCliConnection);
//         
//         connections
//             .OfType<IAzurePowershellConnection>()
//             .Should()
//             .ContainSingle()
//             .And.Subject
//             .Single()
//             .Should()
//             .BeEquivalentTo(azurePowershellConnection);
//         
//         connections
//             .OfType<IAzureDeveloperCliConnection>()
//             .Should()
//             .ContainSingle()
//             .And.Subject
//             .Single()
//             .Should()
//             .BeEquivalentTo(azureDeveloperCliConnection);
//         
//         connections
//             .OfType<IManagedIdentityConnection>()
//             .Should()
//             .ContainSingle()
//             .And.Subject
//             .Single()
//             .Should()
//             .BeEquivalentTo(managedIdentityConnection);
//     }
// }