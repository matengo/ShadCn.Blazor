using Microsoft.AspNetCore.Components;
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

    /// <summary>
    /// Incremented when data changes, used with @key to re-trigger animations.
    /// </summary>
    public int DataVersion { get; set; }

    // Tooltip state
    public int ActiveIndex { get; set; } = -1;

    /// <summary>
    /// Tooltip left position as percentage of chart width (0-100).
    /// </summary>
    public double TooltipLeftPct { get; set; }

    /// <summary>
    /// Tooltip top position as percentage of chart height (0-100).
    /// </summary>
    public double TooltipTopPct { get; set; }

    // Tooltip configuration (set by ChartTooltip registration component)
    public bool TooltipEnabled { get; set; }
    public bool TooltipHideLabel { get; set; }
    public bool TooltipHideIndicator { get; set; }
    public TooltipIndicator TooltipIndicator { get; set; } = TooltipIndicator.Dot;
    public string? TooltipLabelKey { get; set; }
    public string? TooltipNameKey { get; set; }
    public RenderFragment? TooltipCustomContent { get; set; }
    public string? TooltipClass { get; set; }

    /// <summary>
    /// True for pie/radial charts where each data item is a category (not a series).
    /// </summary>
    public bool IsPieStyle { get; set; }

    /// <summary>
    /// The data key for numeric values in pie/radial charts (e.g., "visitors").
    /// </summary>
    public string? ValueDataKey { get; set; }

    /// <summary>
    /// The data key for category names in pie/radial charts (e.g., "browser").
    /// </summary>
    public string? NameDataKey { get; set; }

    public Action? OnStateChanged { get; set; }

    /// <summary>
    /// Set the active data point index and tooltip position using SVG viewBox coordinates.
    /// </summary>
    public void SetActiveIndex(int index, double svgX, double svgY)
    {
        ActiveIndex = index;
        if (Width > 0) TooltipLeftPct = (svgX / Width) * 100;
        if (Height > 0) TooltipTopPct = (svgY / Height) * 100;
        OnStateChanged?.Invoke();
    }

    public void ClearActiveIndex()
    {
        if (ActiveIndex != -1)
        {
            ActiveIndex = -1;
            OnStateChanged?.Invoke();
        }
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
