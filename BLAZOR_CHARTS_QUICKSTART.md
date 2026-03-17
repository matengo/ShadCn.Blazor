# Quick Start: Building a Blazor LineChart Component

## File Structure
\\\
Components/Charts/
├── Models/
│   └── ChartConfig.cs
├── Utilities/
│   ├── CoordinateTransformer.cs
│   └── AxisCalculator.cs
├── Base/
│   ├── ChartBase.razor
│   ├── CartesianGrid.razor
│   └── XAxis.razor
└── LineChart.razor
\\\

## Step 1: Create Data Models

### ChartConfig.cs
\\\csharp
namespace ShadCn.Blazor.Components.Charts;

public record ChartConfig
{
    public Dictionary<string, ChartSeriesConfig> Series { get; set; } = new();
}

public record ChartSeriesConfig
{
    public string Label { get; set; } = "";
    public string Color { get; set; } = "#3b82f6";
    public bool HideInLegend { get; set; }
}

public class ChartDataPoint
{
    public string Label { get; set; } = "";
    public Dictionary<string, double> Values { get; set; } = new();
}

public class ChartOptions
{
    public double MarginLeft { get; set; } = 12;
    public double MarginRight { get; set; } = 12;
    public double MarginTop { get; set; } = 12;
    public double MarginBottom { get; set; } = 12;
    public bool ShowGrid { get; set; } = true;
    public bool ShowLegend { get; set; } = true;
    public int AnimationDurationMs { get; set; } = 300;
}
\\\

## Step 2: Create Utility Classes

### CoordinateTransformer.cs
\\\csharp
namespace ShadCn.Blazor.Components.Charts.Utilities;

public static class CoordinateTransformer
{
    /// <summary>Transform data value to SVG Y coordinate</summary>
    public static double DataToSvgY(
        double value, 
        double minValue,
        double maxValue,
        double svgTopPixel,
        double svgHeightPixels)
    {
        // Edge case: constant values
        if (minValue >= maxValue)
            return svgTopPixel + (svgHeightPixels / 2);
        
        // Normalize to 0-1 range
        double normalized = (value - minValue) / (maxValue - minValue);
        
        // Flip (SVG Y increases downward)
        double flipped = 1.0 - normalized;
        
        // Scale to SVG height
        return svgTopPixel + (flipped * svgHeightPixels);
    }
    
    /// <summary>Transform data index to SVG X coordinate</summary>
    public static double DataIndexToSvgX(
        int index,
        int totalDataPoints,
        double svgLeftPixel,
        double svgWidthPixels)
    {
        if (totalDataPoints <= 1)
            return svgLeftPixel + (svgWidthPixels / 2);
        
        double spacing = svgWidthPixels / (totalDataPoints - 1);
        return svgLeftPixel + (index * spacing);
    }
}
\\\

### AxisCalculator.cs
\\\csharp
namespace ShadCn.Blazor.Components.Charts.Utilities;

public static class AxisCalculator
{
    /// <summary>Calculate nice tick values for Y-axis</summary>
    public static List<(double value, string label)> CalculateTicks(
        double min,
        double max,
        int desiredTickCount = 5)
    {
        if (min >= max) return new();
        
        double range = max - min;
        double magnitude = Math.Pow(10, Math.Floor(Math.Log10(range)));
        double roughInterval = range / (desiredTickCount - 1);
        double normalizedInterval = roughInterval / magnitude;
        
        // Find nice interval: 1, 2, 5, 10, etc.
        double niceInterval = new[] { 1, 2, 5, 10 }
            .OrderBy(n => Math.Abs(n - normalizedInterval))
            .First() * magnitude;
        
        var ticks = new List<(double, string)>();
        double tickValue = Math.Ceiling(min / niceInterval) * niceInterval;
        
        while (tickValue <= max + niceInterval * 0.0001)
        {
            if (tickValue >= min - niceInterval * 0.0001)
            {
                string label = FormatLabel(tickValue);
                ticks.Add((tickValue, label));
            }
            tickValue += niceInterval;
        }
        
        return ticks;
    }
    
    private static string FormatLabel(double value)
    {
        if (Math.Abs(value) >= 1_000_000)
            return \$"{value / 1_000_000:F1}M";
        if (Math.Abs(value) >= 1_000)
            return \$"{value / 1_000:F1}K";
        return Math.Abs(value % 1) < 0.0001 
            ? value.ToString("F0") 
            : value.ToString("F2");
    }
}
\\\

## Step 3: Create Base Component

### ChartBase.razor
\\\azor
@using ShadCn.Blazor.Components.Charts.Utilities
@inherits ComponentBase

@code {
    [Parameter] public List<ChartDataPoint> Data { get; set; } = new();
    [Parameter] public ChartConfig Config { get; set; } = new();
    [Parameter] public ChartOptions? Options { get; set; }
    [Parameter] public string? Class { get; set; }
    
    // Dimensions
    protected double ViewportWidth { get; set; } = 500;
    protected double ViewportHeight { get; set; } = 300;
    
    protected ChartOptions ChartOptions => Options ?? new();
    protected double MarginLeft => ChartOptions.MarginLeft;
    protected double MarginRight => ChartOptions.MarginRight;
    protected double MarginTop => ChartOptions.MarginTop;
    protected double MarginBottom => ChartOptions.MarginBottom;
    
    protected double PlotWidth => ViewportWidth - MarginLeft - MarginRight;
    protected double PlotHeight => ViewportHeight - MarginTop - MarginBottom;
    
    protected string ViewBox => 
        \$"0 0 {ViewportWidth} {ViewportHeight}";
    
    // Data bounds
    protected double MinValue { get; private set; }
    protected double MaxValue { get; private set; }
    
    protected override void OnParametersSet()
    {
        if (Data.Count == 0)
        {
            MinValue = 0;
            MaxValue = 1;
        }
        else
        {
            var allValues = Data.SelectMany(d => d.Values.Values).ToList();
            MinValue = allValues.Min();
            MaxValue = allValues.Max();
            
            // Ensure range
            if (MinValue >= MaxValue)
                MaxValue = MinValue + 1;
        }
    }
    
    // Coordinate transformations
    protected double DataToSvgY(double value) =>
        CoordinateTransformer.DataToSvgY(
            value, MinValue, MaxValue, 
            MarginTop, PlotHeight);
    
    protected double DataIndexToSvgX(int index) =>
        CoordinateTransformer.DataIndexToSvgX(
            index, Data.Count,
            MarginLeft, PlotWidth);
    
    protected string GetSeriesColor(string seriesKey) =>
        Config.Series.TryGetValue(seriesKey, out var config)
            ? config.Color
            : "#999";
}
\\\

## Step 4: Create CartesianGrid Component

### CartesianGrid.razor
\\\azor
@using ShadCn.Blazor.Components.Charts.Utilities
@inherits ComponentBase

<g class="recharts-cartesian-grid" stroke="#ccc">
    @foreach (var tick in Ticks)
    {
        <line x1="@MarginLeft" y1="@(MarginTop + (1.0 - (tick.value - MinValue) / (MaxValue - MinValue)) * PlotHeight)"
              x2="@(MarginLeft + PlotWidth)" 
              y2="@(MarginTop + (1.0 - (tick.value - MinValue) / (MaxValue - MinValue)) * PlotHeight)"
              stroke-dasharray="5,5" stroke-opacity="0.5"/>
    }
</g>

@code {
    [Parameter] public double MinValue { get; set; }
    [Parameter] public double MaxValue { get; set; }
    [Parameter] public double MarginLeft { get; set; } = 12;
    [Parameter] public double MarginTop { get; set; } = 12;
    [Parameter] public double PlotWidth { get; set; } = 500;
    [Parameter] public double PlotHeight { get; set; } = 300;
    
    private List<(double value, string label)> Ticks { get; set; } = new();
    
    protected override void OnParametersSet()
    {
        Ticks = AxisCalculator.CalculateTicks(MinValue, MaxValue, 5);
    }
}
\\\

## Step 5: Create LineChart Component

### LineChart.razor
\\\azor
@using ShadCn.Blazor.Components.Charts.Utilities
@inherits ChartBase

@if (Data.Count > 0)
{
    <svg viewBox="@ViewBox" preserveAspectRatio="xMidYMid meet" 
         class="w-full h-auto @Class"
         @onmousemove="HandleMouseMove"
         @onmouseleave="HandleMouseLeave">
        
        <!-- Grid -->
        <CartesianGrid MinValue="@MinValue" MaxValue="@MaxValue"
                      MarginLeft="@MarginLeft" MarginTop="@MarginTop"
                      PlotWidth="@PlotWidth" PlotHeight="@PlotHeight" />
        
        <!-- Lines for each series -->
        @foreach (var seriesKey in Config.Series.Keys)
        {
            var points = GenerateLinePoints(seriesKey);
            var polylinePoints = string.Join(" ", points.Select(p => \$"{p.x},{p.y}"));
            var color = GetSeriesColor(seriesKey);
            
            <polyline points="@polylinePoints" 
                     fill="none" 
                     stroke="@color" 
                     stroke-width="2"
                     class="line-shape"/>
        }
        
        <!-- Data points (circles) -->
        @foreach (var seriesKey in Config.Series.Keys)
        {
            @for (int i = 0; i < Data.Count; i++)
            {
                var value = Data[i].Values[seriesKey];
                var x = DataIndexToSvgX(i);
                var y = DataToSvgY(value);
                var color = GetSeriesColor(seriesKey);
                
                <circle cx="@x.ToString("F2")" 
                       cy="@y.ToString("F2")" 
                       r="3" 
                       fill="@color"
                       class="data-point"
                       style="cursor: pointer;"/>
            }
        }
        
    </svg>
}

@code {
    private List<(double x, double y)> GenerateLinePoints(string seriesKey)
    {
        return Data
            .Select((d, i) => (DataIndexToSvgX(i), DataToSvgY(d.Values[seriesKey])))
            .ToList();
    }
    
    private void HandleMouseMove(MouseEventArgs e)
    {
        // Tooltip handling would go here
    }
    
    private void HandleMouseLeave()
    {
        // Hide tooltip
    }
}
\\\

## Step 6: Usage Example

### MyChart.razor
\\\azor
@page "/charts/line"

<h1>Line Chart Example</h1>

<LineChart Data="@chartData" Config="@chartConfig" 
          Options="@chartOptions" />

@code {
    private List<ChartDataPoint> chartData = new();
    private ChartConfig chartConfig = new();
    private ChartOptions chartOptions = new();
    
    protected override void OnInitialized()
    {
        // Setup data
        chartData = new()
        {
            new() { Label = "Jan", Values = new() { { "sales", 186 }, { "revenue", 80 } } },
            new() { Label = "Feb", Values = new() { { "sales", 305 }, { "revenue", 200 } } },
            new() { Label = "Mar", Values = new() { { "sales", 237 }, { "revenue", 120 } } },
            new() { Label = "Apr", Values = new() { { "sales", 73 }, { "revenue", 190 } } },
            new() { Label = "May", Values = new() { { "sales", 209 }, { "revenue", 130 } } },
            new() { Label = "Jun", Values = new() { { "sales", 214 }, { "revenue", 140 } } },
        };
        
        // Setup config
        chartConfig = new()
        {
            Series = new()
            {
                { "sales", new() { Label = "Sales", Color = "#3b82f6" } },
                { "revenue", new() { Label = "Revenue", Color = "#10b981" } },
            }
        };
        
        // Setup options
        chartOptions = new()
        {
            MarginLeft = 40,
            MarginRight = 20,
            MarginTop = 20,
            MarginBottom = 40,
            ShowGrid = true,
            ShowLegend = true,
        };
    }
}
\\\

## Step 7: Add Styling

### chart.css
\\\css
.line-shape {
    animation: drawLine 0.8s ease-in-out forwards;
    stroke-dasharray: var(--path-length, 1000);
    stroke-dashoffset: var(--path-length, 1000);
}

@keyframes drawLine {
    from {
        stroke-dashoffset: var(--path-length, 1000);
    }
    to {
        stroke-dashoffset: 0;
    }
}

.data-point {
    transition: fill 0.2s ease;
}

.data-point:hover {
    fill: currentColor !important;
    r: 5;
}
\\\

## Next Steps

1. ✅ Create ChartConfig, ChartDataPoint, ChartOptions
2. ✅ Create CoordinateTransformer utility
3. ✅ Create AxisCalculator utility
4. ✅ Create ChartBase component
5. ✅ Create CartesianGrid component
6. ✅ Create LineChart component
7. ⏭️ Add interactivity (tooltips, selection)
8. ⏭️ Create AreaChart (based on LineChart)
9. ⏭️ Create BarChart
10. ⏭️ Create PieChart
11. ⏭️ Create RadarChart

---

## Key Principles

1. **Pure SVG** - No JavaScript, no third-party charting libraries
2. **Type-Safe** - C# records and classes for configuration
3. **Composable** - Base components can be combined
4. **Extensible** - Easy to add new chart types
5. **ShadCn-Compatible** - Uses same patterns and styling
6. **Blazor-First** - Leverages Razor components and event handlers

