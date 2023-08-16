namespace Empowered.Dataverse.Connection.Store.Contracts;

/// <summary>
/// The connection store manages a list of connection to authenticate against a Dataverse environment.
/// </summary>
public interface IConnectionStore
{
    /// <summary>
    /// This method returns the currently used connection and a list of the non sensitive connection details.
    /// </summary>
    /// <returns></returns>
    IConnectionWallet List();
    
    /// <summary>
    /// This method returns a connection by a given name.
    /// </summary>
    /// <param name="name">The name of the connection to retrieve</param>
    /// <exception cref="ArgumentException">If the connection with the given name is not found.</exception>
    /// <exception cref="ArgumentNullException">If the given name is null or whitespace.</exception>
    /// <returns></returns>
    IDataverseConnection Get(string name);
    
    /// <summary>
    /// This method returns the currently active connection.
    /// <exception cref="InvalidOperationException">If no active connection exists.</exception>
    /// </summary>
    /// <returns></returns>
    IDataverseConnection GetActive();
    
    /// <summary>
    /// This method tries to get the active connection. Returns true if active connection is found, otherwise returns false.
    /// </summary>
    /// <param name="connection">The active connection if result is true, otherwise null.</param>
    /// <returns></returns>
    bool TryGetActive(out IDataverseConnection? connection);
    
    /// <summary>
    /// This method returns true if a connection by the given name exists. If the connection doesn't exist false is returned.
    /// </summary>
    /// <param name="name">The name of the given connection</param>
    /// <param name="connection">if the connection exists contains the connection else null</param>
    /// <exception cref="ArgumentNullException">If the given name is null or whitespace</exception>
    /// <returns></returns>
    bool TryGet(string name, out IDataverseConnection? connection);
    
    /// <summary>
    /// This method inserts or updates a given connection in the store.
    /// The name of the connection is used to determine if the connection already exists. If true the found connection is updated with the given
    /// connection data.
    /// </summary>
    /// <param name="connection">The non-sensitive connection data including the connection name</param>
    /// <param name="useConnection">Set to true if the upserted connection should be used as current connection. Defaults to false</param>
    /// <exception cref="ArgumentException">If a connection is invalid.</exception>
    void Upsert(IDataverseConnection connection, bool useConnection = false);

    /// <summary>
    /// Deletes a connection by a given name.
    /// </summary>
    /// <exception cref="ArgumentException">If a connection with the given name doesn't exist.</exception>
    /// <exception cref="ArgumentNullException">If the given name is null or whitespace.</exception>
    /// <param name="name">The name of the connection to delete.</param>
    void Delete(string name);

    /// <summary>
    /// Tries to delete a connection by a given name.
    /// </summary>
    /// <param name="name">The name of the connection to delete.</param>
    /// <returns>true if a connection with the given name is deleted else false</returns>
    bool TryDelete(string name);

    /// <summary>
    /// Deletes all connections in the connection store.
    /// </summary>
    void Purge();

    /// <summary>
    /// Sets a connection as current connection, to be used if no specific connection name is specified.
    /// </summary>
    /// <exception cref="ArgumentException">If a connection with the given name doesn't exist</exception>
    /// <param name="name">The name of the connection to use</param>
    void Use(string name);

    /// <summary>
    /// Tries to set a connection as current connection, to be used if no specific connection name is specified.
    /// </summary>
    /// <param name="name">The name of the connection to use</param>
    /// <returns>True if a connection with the given name is used else false</returns>
    bool TryUse(string name);
}