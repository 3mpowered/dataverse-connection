namespace Empowered.Dataverse.Connection.Store.Factories;

public class ConnectionStoreFactoryTests
{
    [Fact]
    public void ShouldGetConnectionStoreFromFactory()
    {
        ConnectionStoreFactory.Get().Should().NotBeNull().And.BeOfType<ConnectionStore>();
    }
}