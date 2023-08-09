using System.Security;

namespace Empowered.Dataverse.Connection.Store.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Returns a Secure string from the source string
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static SecureString ToSecureString(this string source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return new SecureString();
        }

        var result = new SecureString();
        foreach (var c in source)
        {
            result.AppendChar(c);
        }

        return result;
    }
}