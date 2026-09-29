using Xunit;

namespace CodexUsageCompanion.Tests;

public sealed class WaylandBackendTests
{
    [Theory]
    [InlineData("wayland-0", ":0", false)]
    [InlineData("wayland-0", null, true)]
    [InlineData(null, ":0", false)]
    public void UsesNativeWaylandOnlyWithoutXWaylandDisplay(
        string? waylandDisplay,
        string? xDisplay,
        bool expected)
    {
        Assert.Equal(expected, Program.ShouldUseNativeWayland(waylandDisplay, xDisplay));
    }
}
