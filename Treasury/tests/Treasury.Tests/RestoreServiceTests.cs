using Treasury.Api.Services;
using Xunit;

namespace Treasury.Tests;

public class RestoreServiceTests
{
    private readonly RestoreService _service = new();

    [Theory]
    [InlineData("DRAGON", "GOD", "ANR")]
    [InlineData("DRAGON", "NOGARD", "")]
    [InlineData("DRAGON", "", "ADGNOR")]
    [InlineData("A", "A", "")]
    [InlineData("ZYXA", "Y", "AXZ")]
    public void Restore_ReturnsSortedMissingSymbols(string reference, string damaged, string expected)
    {
        var result = _service.Restore(reference, damaged);

        Assert.Equal(expected, result.MissingSymbols);
        Assert.Equal(expected.Length == 0, result.IsComplete);
    }
}
