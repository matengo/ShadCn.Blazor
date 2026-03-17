using ShadCn.Blazor.Components.Chart.Internal;

namespace ShadCn.Blazor.Components.Chart;

/// <summary>
/// Shared chart state passed via CascadingValue to child chart components.
/// </summary>
public class ChartContext
{
    public ChartConfig Config { get; set; } = new();
    public IReadOnlyList<object> Data { get; set; } = [];
    public double Width { get; set; }
    public double Height { get; set; }
    public ChartMargin Margin { get; set; } = new();

    /// <summary>
    /// The usable plot area after margins.
    /// </summary>
    public double PlotLeft => Margin.Left;
    public double PlotTop => Margin.Top;
    public double PlotWidth => Width - Margin.Left - Margin.Right;
    public double PlotHeight => Height - Margin.Top - Margin.Bottom;
    public double PlotRight => Width - Margin.Right;
    public double PlotBottom => Height - Margin.Bottom;

    // Scales (set by the parent chart component)
    public ScaleLinear? YScale { get; set; }
    public ScaleBand? XBandScale { get; set; }
    public ScaleLinear? XLinearScale { get; set; }

    /// <summary>
    /// The X data key used for categories (e.g., "month").
    /// </summary>
    public string? XDataKey { get; set; }

    // Tooltip state
    public int ActiveIndex { get; set; } = -1;
    public double MouseX { get; set; }
    public double MouseY { get; set; }

    public Action? OnStateChanged { get; set; }

    public void SetActiveIndex(int index, double mouseX, double mouseY)
    {
        ActiveIndex = index;
        MouseX = mouseX;
        MouseY = mouseY;
        OnStateChanged?.Invoke();
    }

    public void ClearActiveIndex()
    {
        ActiveIndex = -1;
        OnStateChanged?.Invoke();
    }
}

/// <summary>
/// Chart margin (padding between SVG edge and plot area).
/// </summary>
public record ChartMargin
{
    public double Top { get; init; } = 20;
    public double Right { get; init; } = 20;
    public double Bottom { get; init; } = 30;
    public double Left { get; init; } = 40;
}
