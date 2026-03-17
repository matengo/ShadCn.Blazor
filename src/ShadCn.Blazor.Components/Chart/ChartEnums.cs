namespace ShadCn.Blazor.Components.Chart;

/// <summary>
/// Curve interpolation type for line and area charts.
/// </summary>
public enum CurveType
{
    Linear,
    Monotone,
    Step,
    Natural
}

/// <summary>
/// Tooltip indicator style.
/// </summary>
public enum TooltipIndicator
{
    Dot,
    Line,
    Dashed
}

/// <summary>
/// Bar chart layout direction.
/// </summary>
public enum BarLayout
{
    Vertical,
    Horizontal
}

/// <summary>
/// Stacking strategy for multi-series charts.
/// </summary>
public enum StackType
{
    None,
    Stacked,
    Percentage
}

/// <summary>
/// Legend alignment position.
/// </summary>
public enum LegendAlign
{
    Top,
    Bottom
}

/// <summary>
/// Axis orientation.
/// </summary>
public enum AxisOrientation
{
    Top,
    Bottom,
    Left,
    Right
}
