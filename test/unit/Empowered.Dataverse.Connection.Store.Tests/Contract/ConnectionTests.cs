namespace Empowered.Dataverse.Connection.Store.Contract;

public class ConnectionTests
{
    [Fact]
    public void ShouldReturnTrueForConnectionsWithSameNameOnEqualityComparison()
    {
        const string name = "Test";
        var connection1 = new Model.Connection
        {
            Name = name,
            EnvironmentUrl = new Uri("https://test.crm4.dynamics.com")
        }.As<IConnection>();

        var connection2 = new Model.Connection
        {
            Name = name,
            EnvironmentUrl = new Uri("https://test.crm12.dynamics.com")
        }.As<IConnection>();

        connection1.Equals(connection2).Should().BeTrue();
    }

    [Fact]
    public void ShouldReturnFalseForEqualityComparisonOnNull()
    {
        const string name = "Test";
        var connection1 = new Model.Connection
        {
            Name = name,
            EnvironmentUrl = new Uri("https://test.crm4.dynamics.com")
        }.As<IConnection>();

        IConnection? connection2 = null;

        connection1.Equals(connection2).Should().BeFalse();
    }
    
    [Fact]
    public void ShouldReturnTrueForEqualityComparisonOnSameReference()
    {
        const string name = "Test";
        var connection1 = new Model.Connection
        {
            Name = name,
            EnvironmentUrl = new Uri("https://test.crm4.dynamics.com")
        }.As<IConnection>();

        connection1.Equals(connection1).Should().BeTrue();
    }
}