using Empowered.Dataverse.Connection.Store.Contracts;
using Empowered.Dataverse.Connection.Store.Model;

namespace Empowered.Dataverse.Connection.Store.Contract;

public class ConnectionTests
{
    [Fact]
    public void ShouldReturnTrueForConnectionsWithSameNameOnEqualityComparison()
    {
        const string name = "Test";
        var connection1 = new BaseConnection(name, new Uri("https://test.crm4.dynamics.com")).As<IBaseConnection>();

        var connection2 = new BaseConnection(name, new Uri("https://test.crm12.dynamics.com")).As<IBaseConnection>();

        connection1.Equals(connection2).Should().BeTrue();
    }

    [Fact]
    public void ShouldReturnFalseForEqualityComparisonOnNull()
    {
        const string name = "Test";
        var connection1 = new BaseConnection(name, new Uri("https://test.crm4.dynamics.com")).As<IBaseConnection>();

        IBaseConnection? connection2 = null;

        connection1.Equals(connection2).Should().BeFalse();
    }
    
    [Fact]
    public void ShouldReturnTrueForEqualityComparisonOnSameReference()
    {
        const string name = "Test";
        var connection1 = new BaseConnection(name, new Uri("https://test.crm4.dynamics.com")).As<IBaseConnection>();

        connection1.Equals(connection1).Should().BeTrue();
    }
}