namespace Empowered.Dataverse.Connection.Store.Contracts;

public interface IBaseConnection : IEquatable<IBaseConnection>
{
    string Name { get; }
    Uri EnvironmentUrl { get; }
    ConnectionType Type { get; }
    TConnection ToConnection<TConnection>() where TConnection : IBaseConnection => (TConnection)this;

    IBaseConnection Clone();

    bool IEquatable<IBaseConnection>.Equals(IBaseConnection? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name;
    }
}