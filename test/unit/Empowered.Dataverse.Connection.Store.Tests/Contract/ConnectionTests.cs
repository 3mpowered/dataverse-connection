using Empowered.Dataverse.Connection.Store.Contracts;
using Empowered.Dataverse.Connection.Store.Model;

namespace Empowered.Dataverse.Connection.Store.Contract;

public class ConnectionTests
{
    [Fact]
    public void ShouldReturnTrueForConnectionsWithSameNameOnEqualityComparison()
    {
        const string name = "Test";
        var connection1 = new DataverseConnection(name, new Uri("https://test.crm4.dynamics.com"), ConnectionType.Interactive)
            .As<IDataverseConnection>();

        var connection2 = new DataverseConnection(name, new Uri("https://test.crm12.dynamics.com"), ConnectionType.Interactive)
            .As<IDataverseConnection>();

        connection1.Equals(connection2).Should().BeTrue();
    }

    [Fact]
    public void ShouldReturnFalseForEqualityComparisonOnNull()
    {
        const string name = "Test";
        var connection1 = new DataverseConnection(name, new Uri("https://test.crm4.dynamics.com"), ConnectionType.Interactive)
            .As<IDataverseConnection>();

        IDataverseConnection? connection2 = null;

        connection1.Equals(connection2).Should().BeFalse();
    }

    [Fact]
    public void ShouldReturnTrueForEqualityComparisonOnSameReference()
    {
        const string name = "Test";
        var connection1 = new DataverseConnection(name, new Uri("https://test.crm4.dynamics.com"), ConnectionType.Interactive)
            .As<IDataverseConnection>();

        connection1.Equals(connection1).Should().BeTrue();
    }
}