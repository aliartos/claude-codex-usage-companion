using Avalonia;
using CodexUsageCompanion.Configuration;
using CodexUsageCompanion.Ui;
using Xunit;

namespace CodexUsageCompanion.Tests;

public sealed class OverlayWindowPlacementTests
{
    [Fact]
    public void LeftBottomAvoidsLeftAndBottomTaskbars()
    {
        var area = new PixelRect(54, 0, 1866, 1040);
        var position = OverlayWindowPlacement.Calculate(area, new PixelSize(430, 349), 16, WindowPosition.LeftBottom);
        Assert.Equal(new PixelPoint(70, 675), position);
    }

    [Theory]
    [InlineData(WindowPosition.LeftTop)]
    [InlineData(WindowPosition.MiddleTop)]
    [InlineData(WindowPosition.RightTop)]
    [InlineData(WindowPosition.LeftCenter)]
    [InlineData(WindowPosition.MiddleCenter)]
    [InlineData(WindowPosition.RightCenter)]
    [InlineData(WindowPosition.LeftBottom)]
    [InlineData(WindowPosition.MiddleBottom)]
    [InlineData(WindowPosition.RightBottom)]
    public void EveryPresetStaysInsideReservedWorkingAreaOnOffsetMonitor(string preset)
    {
        var area = new PixelRect(-1920, 40, 1866, 1000);
        var size = new PixelSize(600, 450);
        var position = OverlayWindowPlacement.Calculate(area, size, 24, preset);
        Assert.InRange(position.X, area.X + 24, area.Right - size.Width - 24);
        Assert.InRange(position.Y, area.Y + 24, area.Bottom - size.Height - 24);
    }
}
