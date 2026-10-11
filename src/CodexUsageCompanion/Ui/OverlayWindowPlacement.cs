using Avalonia;
using CodexUsageCompanion.Configuration;

namespace CodexUsageCompanion.Ui;

internal static class OverlayWindowPlacement
{
    public static PixelPoint Calculate(PixelRect workingArea, PixelSize size, int margin, string position)
    {
        var x = position switch
        {
            WindowPosition.LeftTop or WindowPosition.LeftCenter or WindowPosition.LeftBottom => workingArea.X + margin,
            WindowPosition.MiddleTop or WindowPosition.MiddleCenter or WindowPosition.MiddleBottom => workingArea.X + (workingArea.Width - size.Width) / 2,
            _ => workingArea.Right - size.Width - margin
        };
        var y = position switch
        {
            WindowPosition.LeftTop or WindowPosition.MiddleTop or WindowPosition.RightTop => workingArea.Y + margin,
            WindowPosition.LeftCenter or WindowPosition.MiddleCenter or WindowPosition.RightCenter => workingArea.Y + (workingArea.Height - size.Height) / 2,
            _ => workingArea.Bottom - size.Height - margin
        };
        return new PixelPoint(x, y);
    }
}
