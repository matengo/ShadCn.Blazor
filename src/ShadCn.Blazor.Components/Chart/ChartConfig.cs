namespace ShadCn.Blazor.Components.Chart;

/// <summary>
/// Configuration for chart series — maps data keys to labels, colors, and theme values.
/// Mirrors the shadcn/ui ChartConfig pattern.
/// </summary>
public class ChartConfig : Dictionary<string, ChartConfigEntry>
{
    public ChartConfig() : base(StringComparer.OrdinalIgnoreCase) { }
}

/// <summary>
/// Configuration entry for a single chart series.
/// </summary>
public record ChartConfigEntry
{
    /// <summary>
    /// Human-readable label for the series.
    /// </summary>
    public string? Label { get; init; }

    /// <summary>
    /// Color value — can be hex, hsl, oklch, or a CSS variable reference.
    /// </summary>
    public string? Color { get; init; }

    /// <summary>
    /// Theme-aware colors with light/dark variants.
    /// </summary>
    public ChartThemeColors? Theme { get; init; }
}

/// <summary>
/// Light and dark color variants for theming.
/// </summary>
public record ChartThemeColors
{
    public string? Light { get; init; }
    public string? Dark { get; init; }
}
