using System.Security;

namespace Empowered.Dataverse.Connection.Store.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ShouldReturnEmptySecureString(string? unsecureString)
    {
        unsecureString!.ToSecureString().Should().BeEquivalentTo(new SecureString());
    }
}