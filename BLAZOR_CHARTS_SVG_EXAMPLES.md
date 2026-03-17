# SVG Chart Examples & Generation Patterns

## 1. LINE CHART SVG OUTPUT

### Simple Line Chart (Recharts format)
\\\xml
<?xml version="1.0" encoding="UTF-8"?>
<svg viewBox="0 0 500 300" xmlns="http://www.w3.org/2000/svg">
  <!-- Background -->
  <rect width="500" height="300" fill="white"/>
  
  <!-- Grid lines -->
  <g class="recharts-cartesian-grid" stroke="#ddd">
    <line x1="50" x2="450" y1="80" y2="80" stroke-dasharray="3 3"/>
    <line x1="50" x2="450" y1="130" y2="130" stroke-dasharray="3 3"/>
    <line x1="50" x2="450" y1="180" y2="180" stroke-dasharray="3 3"/>
    <line x1="50" x2="450" y1="230" y2="230" stroke-dasharray="3 3"/>
  </g>
  
  <!-- Y-Axis -->
  <g text-anchor="end" class="recharts-cartesian-axis">
    <line x1="50" y1="50" x2="50" y2="250" stroke="black"/>
    <text x="40" y="85">100</text>
    <text x="40" y="135">200</text>
    <text x="40" y="185">300</text>
    <text x="40" y="235">400</text>
  </g>
  
  <!-- X-Axis -->
  <g class="recharts-cartesian-axis">
    <line x1="50" y1="250" x2="450" y2="250" stroke="black"/>
    <text x="90" y="270" text-anchor="middle">Jan</text>
    <text x="150" y="270" text-anchor="middle">Feb</text>
    <text x="210" y="270" text-anchor="middle">Mar</text>
    <text x="270" y="270" text-anchor="middle">Apr</text>
    <text x="330" y="270" text-anchor="middle">May</text>
    <text x="390" y="270" text-anchor="middle">Jun</text>
  </g>
  
  <!-- Data Line -->
  <polyline 
    points="90,195 150,140 210,170 270,215 330,170 390,160"
    fill="none"
    stroke="#3b82f6"
    stroke-width="2"/>
  
  <!-- Data Points -->
  <g class="recharts-data-points">
    <circle cx="90" cy="195" r="3" fill="#3b82f6"/>
    <circle cx="150" cy="140" r="3" fill="#3b82f6"/>
    <circle cx="210" cy="170" r="3" fill="#3b82f6"/>
    <circle cx="270" cy="215" r="3" fill="#3b82f6"/>
    <circle cx="330" cy="170" r="3" fill="#3b82f6"/>
    <circle cx="390" cy="160" r="3" fill="#3b82f6"/>
  </g>
</svg>
\\\

### How to Generate in C#

\\\csharp
public string GenerateLineChartSVG(List<ChartDataPoint> data, ChartConfig config)
{
    var svg = new StringBuilder();
    svg.AppendLine("<svg viewBox='0 0 500 300' xmlns='http://www.w3.org/2000/svg'>");
    
    // Coordinate system setup
    double plotLeft = 50, plotTop = 50, plotWidth = 400, plotHeight = 200;
    var allValues = data.SelectMany(d => d.Values.Values).ToList();
    double minVal = allValues.Min();
    double maxVal = allValues.Max();
    
    // Grid
    svg.AppendLine("<g class='grid' stroke='#ddd'>");
    for (int i = 0; i <= 4; i++)
    {
        double y = plotTop + (i * plotHeight / 4);
        svg.AppendLine(\$"<line x1='{plotLeft}' x2='{plotLeft + plotWidth}' y1='{y}' y2='{y}' stroke-dasharray='3 3'/>");
    }
    svg.AppendLine("</g>");
    
    // Lines for each series
    foreach (var seriesKey in config.Series.Keys)
    {
        var points = data.Select((d, i) =>
        {
            double x = plotLeft + (i * plotWidth / (data.Count - 1));
            double value = d.Values[seriesKey];
            double normalized = (value - minVal) / (maxVal - minVal);
            double y = plotTop + plotHeight * (1 - normalized);
            return \$"{x},{y}";
        });
        
        string pointsStr = string.Join(" ", points);
        var color = config.Series[seriesKey].Color;
        
        svg.AppendLine(\$"<polyline points='{pointsStr}' fill='none' stroke='{color}' stroke-width='2'/>");
    }
    
    svg.AppendLine("</svg>");
    return svg.ToString();
}
\\\

---

## 2. AREA CHART SVG OUTPUT

### Area Chart with Gradient Fill

\\\xml
<svg viewBox="0 0 500 300" xmlns="http://www.w3.org/2000/svg">
  <!-- Define gradient -->
  <defs>
    <linearGradient id="areaGradient" x1="0%" y1="0%" x2="0%" y2="100%">
      <stop offset="0%" stop-color="#3b82f6" stop-opacity="0.4"/>
      <stop offset="100%" stop-color="#3b82f6" stop-opacity="0"/>
    </linearGradient>
  </defs>
  
  <!-- Area (filled region) -->
  <path d="M 90,195 L 150,140 L 210,170 L 270,215 L 330,170 L 390,160
           L 390,250 L 330,250 L 270,250 L 210,250 L 150,250 L 90,250 Z"
        fill="url(#areaGradient)"
        stroke="none"/>
  
  <!-- Line (on top of area) -->
  <polyline points="90,195 150,140 210,170 270,215 330,170 390,160"
           fill="none"
           stroke="#3b82f6"
           stroke-width="2"/>
</svg>
\\\

### C# Generation

\\\csharp
public string GenerateAreaPath(List<ChartDataPoint> data, string seriesKey, 
                               double plotLeft, double plotWidth, 
                               double plotTop, double plotHeight,
                               double minVal, double maxVal)
{
    var points = new List<(double x, double y)>();
    
    // Forward pass (top line)
    foreach (var (d, i) in data.Select((d, i) => (d, i)))
    {
        double x = plotLeft + (i * plotWidth / (data.Count - 1));
        double value = d.Values[seriesKey];
        double normalized = (value - minVal) / (maxVal - minVal);
        double y = plotTop + plotHeight * (1 - normalized);
        points.Add((x, y));
    }
    
    // Build path
    var path = new StringBuilder();
    path.Append(\$"M {points[0].x},{points[0].y}");
    
    foreach (var p in points.Skip(1))
        path.Append(\$" L {p.x},{p.y}");
    
    // Close path with bottom line
    double bottomY = plotTop + plotHeight;
    path.Append(\$" L {points.Last().x},{bottomY}");
    
    // Return to start
    foreach (var p in points.Reverse<(double, double)>().Skip(1))
        path.Append(\$" L {p.x},{bottomY}");
    
    path.Append(" Z");
    
    return path.ToString();
}
\\\

---

## 3. BAR CHART SVG OUTPUT

### Grouped Bar Chart
\\\xml
<svg viewBox="0 0 500 300" xmlns="http://www.w3.org/2000/svg">
  <!-- Bar groups -->
  <g class="bar-group">
    <!-- January: Sales & Revenue -->
    <rect x="50" y="140" width="20" height="100" fill="#3b82f6" rx="2"/>
    <rect x="75" y="180" width="20" height="60" fill="#10b981" rx="2"/>
    
    <!-- February -->
    <rect x="120" y="80" width="20" height="160" fill="#3b82f6" rx="2"/>
    <rect x="145" y="120" width="20" height="120" fill="#10b981" rx="2"/>
    
    <!-- March -->
    <rect x="190" y="120" width="20" height="120" fill="#3b82f6" rx="2"/>
    <rect x="215" y="160" width="20" height="80" fill="#10b981" rx="2"/>
    
    <!-- April -->
    <rect x="260" y="195" width="20" height="45" fill="#3b82f6" rx="2"/>
    <rect x="285" y="120" width="20" height="120" fill="#10b981" rx="2"/>
    
    <!-- May -->
    <rect x="330" y="150" width="20" height="90" fill="#3b82f6" rx="2"/>
    <rect x="355" y="160" width="20" height="80" fill="#10b981" rx="2"/>
    
    <!-- June -->
    <rect x="400" y="140" width="20" height="100" fill="#3b82f6" rx="2"/>
    <rect x="425" y="130" width="20" height="110" fill="#10b981" rx="2"/>
  </g>
  
  <!-- X-Axis labels -->
  <text x="62" y="270" text-anchor="middle">Jan</text>
  <text x="132" y="270" text-anchor="middle">Feb</text>
  <text x="202" y="270" text-anchor="middle">Mar</text>
  <text x="272" y="270" text-anchor="middle">Apr</text>
  <text x="342" y="270" text-anchor="middle">May</text>
  <text x="412" y="270" text-anchor="middle">Jun</text>
</svg>
\\\

### C# Bar Generation

\\\csharp
public void GenerateBars(StringBuilder svg, List<ChartDataPoint> data, 
                         ChartConfig config, double plotArea)
{
    double barGroupWidth = plotArea.Width / data.Count;
    double barWidth = barGroupWidth / config.Series.Count * 0.8;
    double spacing = (barGroupWidth - (barWidth * config.Series.Count)) / 2;
    
    double minVal = data.SelectMany(d => d.Values.Values).Min();
    double maxVal = data.SelectMany(d => d.Values.Values).Max();
    
    for (int i = 0; i < data.Count; i++)
    {
        double groupX = plotArea.Left + (i * barGroupWidth);
        var seriesKeys = config.Series.Keys.ToList();
        
        for (int j = 0; j < seriesKeys.Count; j++)
        {
            string seriesKey = seriesKeys[j];
            double value = data[i].Values[seriesKey];
            
            double x = groupX + spacing + (j * barWidth);
            double normalized = (value - minVal) / (maxVal - minVal);
            double height = normalized * plotArea.Height;
            double y = plotArea.Bottom - height;
            
            string color = config.Series[seriesKey].Color;
            
            svg.AppendLine(\$"<rect x='{x}' y='{y}' width='{barWidth}' height='{height}' " +
                          \$"fill='{color}' rx='2'/>");
        }
    }
}
\\\

---

## 4. PIE CHART SVG OUTPUT

### Donut Chart with Arcs
\\\xml
<svg viewBox="0 0 400 400" xmlns="http://www.w3.org/2000/svg">
  <!-- Legend: Chrome 25% -->
  <path d="M 200,200 L 300,200 A 100,100 0 0 1 270.7,70.7 Z"
        fill="#3b82f6" stroke="white" stroke-width="2"/>
  <text x="250" y="120" text-anchor="middle" fill="white">25%</text>
  
  <!-- Safari 20% -->
  <path d="M 200,200 L 270.7,70.7 A 100,100 0 0 1 229.3,29.3 Z"
        fill="#10b981" stroke="white" stroke-width="2"/>
  <text x="240" y="60" text-anchor="middle" fill="white">20%</text>
  
  <!-- Firefox 30% -->
  <path d="M 200,200 L 229.3,29.3 A 100,100 0 0 1 100,100 Z"
        fill="#f59e0b" stroke="white" stroke-width="2"/>
  <text x="150" y="80" text-anchor="middle" fill="white">30%</text>
  
  <!-- Edge 15% -->
  <path d="M 200,200 L 100,100 A 100,100 0 0 1 129.3,329.3 Z"
        fill="#8b5cf6" stroke="white" stroke-width="2"/>
  <text x="110" y="200" text-anchor="middle" fill="white">15%</text>
  
  <!-- Other 10% -->
  <path d="M 200,200 L 129.3,329.3 A 100,100 0 0 1 300,200 Z"
        fill="#ec4899" stroke="white" stroke-width="2"/>
  <text x="200" y="300" text-anchor="middle" fill="white">10%</text>
  
  <!-- Center label (donut hole) -->
  <text x="200" y="190" text-anchor="middle" font-size="32" font-weight="bold">1260</text>
  <text x="200" y="215" text-anchor="middle" font-size="14" fill="#999">Visitors</text>
</svg>
\\\

### C# Pie Generation

\\\csharp
public string GeneratePiePath(double centerX, double centerY, double radius,
                              double startAngleDeg, double endAngleDeg,
                              bool isDonut = false, double innerRadius = 0)
{
    // Convert degrees to radians
    double startAngle = startAngleDeg * Math.PI / 180;
    double endAngle = endAngleDeg * Math.PI / 180;
    
    // Outer arc endpoints
    double x1 = centerX + radius * Math.Cos(startAngle);
    double y1 = centerY + radius * Math.Sin(startAngle);
    double x2 = centerX + radius * Math.Cos(endAngle);
    double y2 = centerY + radius * Math.Sin(endAngle);
    
    // Determine large-arc-flag (angle > 180°)
    int largeArc = (endAngleDeg - startAngleDeg) > 180 ? 1 : 0;
    
    var path = new StringBuilder();
    
    if (isDonut)
    {
        // Start from outer arc
        path.Append(\$"M {x1},{y1}");
        path.Append(\$" A {radius},{radius} 0 {largeArc} 1 {x2},{y2}");
        
        // Line to inner arc
        double x3 = centerX + innerRadius * Math.Cos(endAngle);
        double y3 = centerY + innerRadius * Math.Sin(endAngle);
        path.Append(\$" L {x3},{y3}");
        
        // Inner arc (reverse)
        path.Append(\$" A {innerRadius},{innerRadius} 0 {largeArc} 0 " +
                   \$"{centerX + innerRadius * Math.Cos(startAngle)}," +
                   \$"{centerY + innerRadius * Math.Sin(startAngle)}");
        
        path.Append(" Z");
    }
    else
    {
        // Pie slice (with center point)
        path.Append(\$"M {centerX},{centerY}");
        path.Append(\$" L {x1},{y1}");
        path.Append(\$" A {radius},{radius} 0 {largeArc} 1 {x2},{y2}");
        path.Append(" Z");
    }
    
    return path.ToString();
}
\\\

---

## 5. RADAR CHART SVG OUTPUT

### Multi-Series Radar
\\\xml
<svg viewBox="0 0 400 400" xmlns="http://www.w3.org/2000/svg">
  <!-- Concentric circles (grid) -->
  <circle cx="200" cy="200" r="40" fill="none" stroke="#ddd"/>
  <circle cx="200" cy="200" r="80" fill="none" stroke="#ddd"/>
  <circle cx="200" cy="200" r="120" fill="none" stroke="#ddd"/>
  
  <!-- Radial grid lines -->
  <line x1="200" y1="80" x2="200" y2="320" stroke="#ddd"/>
  <line x1="100" y1="135" x2="300" y2="265" stroke="#ddd"/>
  <line x1="100" y1="265" x2="300" y2="135" stroke="#ddd"/>
  <!-- ... more radial lines ... -->
  
  <!-- Data polygon 1 -->
  <polygon points="200,80 280,135 310,210 280,285 200,310 120,285 90,210 120,135"
           fill="#3b82f6" fill-opacity="0.2" stroke="#3b82f6" stroke-width="2"/>
  
  <!-- Data polygon 2 -->
  <polygon points="200,100 260,145 290,200 260,270 200,300 140,270 110,200 140,145"
           fill="#10b981" fill-opacity="0.2" stroke="#10b981" stroke-width="2"/>
  
  <!-- Axis labels -->
  <text x="200" y="70" text-anchor="middle">January</text>
  <text x="310" y="210" text-anchor="middle">February</text>
  <!-- ... more labels ... -->
</svg>
\\\

### C# Radar Generation

\\\csharp
public string GenerateRadarPolygon(List<double> values, double centerX, double centerY,
                                    double maxRadius, int numAxes)
{
    var points = new List<(double x, double y)>();
    
    for (int i = 0; i < numAxes; i++)
    {
        double angle = (2 * Math.PI * i) / numAxes;
        double value = i < values.Count ? values[i] : 0;
        double radius = (value / 100) * maxRadius;  // Normalize value to 0-100
        
        double x = centerX + radius * Math.Cos(angle - Math.PI / 2);
        double y = centerY + radius * Math.Sin(angle - Math.PI / 2);
        
        points.Add((x, y));
    }
    
    // Close polygon
    points.Add(points[0]);
    
    // Build polygon points string
    string pointsStr = string.Join(" ", points.Select(p => \$"{p.x},{p.y}"));
    
    return \$"<polygon points='{pointsStr}' fill='#3b82f6' fill-opacity='0.2' stroke='#3b82f6'/>";
}
\\\

---

## 6. RADIAL BAR CHART SVG OUTPUT

### Circular Progress Bars
\\\xml
<svg viewBox="0 0 400 400" xmlns="http://www.w3.org/2000/svg">
  <!-- Background circles -->
  <circle cx="200" cy="200" r="80" fill="#f0f0f0" stroke="none"/>
  <circle cx="200" cy="200" r="100" fill="none" stroke="#f0f0f0" stroke-width="20"/>
  <circle cx="200" cy="200" r="120" fill="none" stroke="#f0f0f0" stroke-width="20"/>
  
  <!-- Data bars (arcs) -->
  <!-- Safari: 1260 visitors -->
  <path d="M 200,80 A 120,120 0 1 1 199.99,80.01"
        fill="none" stroke="#10b981" stroke-width="20" stroke-linecap="round"/>
  
  <!-- Center label -->
  <text x="200" y="190" text-anchor="middle" font-size="48" font-weight="bold">1260</text>
  <text x="200" y="220" text-anchor="middle" font-size="14" fill="#999">Visitors</text>
</svg>
\\\

### C# Radial Bar Generation

\\\csharp
public string GenerateRadialBar(double value, double maxValue,
                                 double centerX, double centerY,
                                 double innerRadius, double outerRadius)
{
    // Calculate angle based on value (0-360 degrees)
    double angle = (value / maxValue) * 360;
    double angleRad = angle * Math.PI / 180;
    
    // Start point (top)
    double x1 = centerX;
    double y1 = centerY - outerRadius;
    
    // End point
    double x2 = centerX + outerRadius * Math.Sin(angleRad);
    double y2 = centerY - outerRadius * Math.Cos(angleRad);
    
    // Determine large-arc-flag
    int largeArc = angle > 180 ? 1 : 0;
    
    // Build arc path
    var path = new StringBuilder();
    path.Append(\$"M {x1},{y1}");
    path.Append(\$" A {outerRadius},{outerRadius} 0 {largeArc} 1 {x2},{y2}");
    
    return path.ToString();
}
\\\

---

## 7. SMOOTH CURVE GENERATION (Monotone Cubic)

### Algorithm for Line Smoothing

Instead of straight lines, generate smooth bezier curves:

\\\csharp
public class MonotoneCurveGenerator
{
    /// <summary>
    /// Generate smooth SVG path with cubic bezier curves
    /// </summary>
    public static string GenerateSmoothPath(List<(double x, double y)> points)
    {
        if (points.Count < 2) return "";
        
        var path = new StringBuilder();
        path.Append(\$"M {points[0].x},{points[0].y}");
        
        if (points.Count == 2)
        {
            path.Append(\$" L {points[1].x},{points[1].y}");
            return path.ToString();
        }
        
        // Generate cubic bezier curves between each pair of points
        for (int i = 1; i < points.Count; i++)
        {
            var (cp1, cp2) = CalculateControlPoints(points, i - 1);
            
            path.Append(\$" C {cp1.x},{cp1.y} {cp2.x},{cp2.y} {points[i].x},{points[i].y}");
        }
        
        return path.ToString();
    }
    
    private static ((double x, double y), (double x, double y)) CalculateControlPoints(
        List<(double x, double y)> points, int index)
    {
        var p0 = index > 0 ? points[index - 1] : points[index];
        var p1 = points[index];
        var p2 = points[index + 1];
        var p3 = index + 2 < points.Count ? points[index + 2] : points[index + 1];
        
        // Calculate slopes using monotone cubic interpolation
        double m1 = GetMonotoneSlope(p0, p1, p2);
        double m2 = GetMonotoneSlope(p1, p2, p3);
        
        // Control points
        double cp1x = p1.x + (p2.x - p1.x) / 3;
        double cp1y = p1.y + m1 * (p2.x - p1.x) / 3;
        
        double cp2x = p2.x - (p2.x - p1.x) / 3;
        double cp2y = p2.y - m2 * (p2.x - p1.x) / 3;
        
        return ((cp1x, cp1y), (cp2x, cp2y));
    }
    
    private static double GetMonotoneSlope(
        (double x, double y) p0,
        (double x, double y) p1,
        (double x, double y) p2)
    {
        double dx1 = p1.x - p0.x;
        double dy1 = p1.y - p0.y;
        double dx2 = p2.x - p1.x;
        double dy2 = p2.y - p1.y;
        
        if (Math.Abs(dx1) < 0.001 || Math.Abs(dx2) < 0.001)
            return 0;
        
        double slope1 = dy1 / dx1;
        double slope2 = dy2 / dx2;
        
        // Monotone condition
        if (Math.Sign(slope1) != Math.Sign(slope2))
            return 0;
        
        return (slope1 + slope2) / 2;
    }
}
\\\

---

## Summary

Each chart type maps to standard SVG elements:
- **Lines/Areas** → polyline + path
- **Bars** → rect
- **Pie** → path with arc command
- **Radar** → polygon
- **Radial** → path with circular arc

The math is straightforward:
1. Normalize data values to 0-1 range
2. Scale to SVG coordinate space
3. Use standard geometry/trigonometry for arcs

All generation is possible in pure C# with StringBuilder!

