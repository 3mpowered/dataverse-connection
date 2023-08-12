namespace Empowered.Dataverse.Connection.Store.Factories;

public class ConnectionSecretProviderFactoryTests
{
    [Fact]
    public void ShouldGetConnectionSecretProviderFromFactory()
    {
        ConnectionSecretProviderFactory.Get().Should().NotBeNull().And.BeOfType<ConnectionSecretProvider>();
    }
}