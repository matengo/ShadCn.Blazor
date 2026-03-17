namespace ShadCn.Blazor.Components.Chart.Internal;

/// <summary>
/// Generates SVG arc paths for pie and radial charts.
/// </summary>
public static class ArcGenerator
{
    /// <summary>
    /// Generates an SVG path "d" string for an arc sector (pie slice).
    /// </summary>
    /// <param name="cx">Center X.</param>
    /// <param name="cy">Center Y.</param>
    /// <param name="outerRadius">Outer radius of the arc.</param>
    /// <param name="innerRadius">Inner radius (0 for full pie, >0 for donut).</param>
    /// <param name="startAngle">Start angle in radians (0 = top/12 o'clock).</param>
    /// <param name="endAngle">End angle in radians.</param>
    /// <param name="cornerRadius">Corner radius for rounded arcs.</param>
    public static string Arc(double cx, double cy, double outerRadius, double innerRadius,
        double startAngle, double endAngle, double cornerRadius = 0)
    {
        var path = new SvgPathBuilder();
        var angleDiff = Math.Abs(endAngle - startAngle);

        // Full circle case
        if (angleDiff >= 2 * Math.PI - 1e-6)
        {
            return FullCircleArc(cx, cy, outerRadius, innerRadius);
        }

        var outerStartX = cx + outerRadius * Math.Sin(startAngle);
        var outerStartY = cy - outerRadius * Math.Cos(startAngle);
        var outerEndX = cx + outerRadius * Math.Sin(endAngle);
        var outerEndY = cy - outerRadius * Math.Cos(endAngle);

        var largeArc = angleDiff > Math.PI;

        path.MoveTo(outerStartX, outerStartY);
        path.ArcTo(outerRadius, outerRadius, 0, largeArc, true, outerEndX, outerEndY);

        if (innerRadius > 0)
        {
            var innerEndX = cx + innerRadius * Math.Sin(endAngle);
            var innerEndY = cy - innerRadius * Math.Cos(endAngle);
            var innerStartX = cx + innerRadius * Math.Sin(startAngle);
            var innerStartY = cy - innerRadius * Math.Cos(startAngle);

            path.LineTo(innerEndX, innerEndY);
            path.ArcTo(innerRadius, innerRadius, 0, largeArc, false, innerStartX, innerStartY);
        }
        else
        {
            path.LineTo(cx, cy);
        }

        path.ClosePath();
        return path.Build();
    }

    /// <summary>
    /// Generates an SVG path for a full circle (or donut).
    /// Uses two semicircular arcs because SVG cannot draw a full circle with a single arc.
    /// </summary>
    private static string FullCircleArc(double cx, double cy, double outerRadius, double innerRadius)
    {
        var path = new SvgPathBuilder();

        // Outer circle: two semicircles
        path.MoveTo(cx, cy - outerRadius);
        path.ArcTo(outerRadius, outerRadius, 0, false, true, cx, cy + outerRadius);
        path.ArcTo(outerRadius, outerRadius, 0, false, true, cx, cy - outerRadius);

        if (innerRadius > 0)
        {
            // Inner circle: two semicircles (reverse direction for cutout)
            path.MoveTo(cx, cy - innerRadius);
            path.ArcTo(innerRadius, innerRadius, 0, false, false, cx, cy + innerRadius);
            path.ArcTo(innerRadius, innerRadius, 0, false, false, cx, cy - innerRadius);
        }

        path.ClosePath();
        return path.Build();
    }

    /// <summary>
    /// Computes the centroid (center point) of an arc — useful for label positioning.
    /// </summary>
    public static (double X, double Y) Centroid(double cx, double cy, double outerRadius,
        double innerRadius, double startAngle, double endAngle)
    {
        var midAngle = (startAngle + endAngle) / 2;
        var midRadius = (outerRadius + innerRadius) / 2;
        return (cx + midRadius * Math.Sin(midAngle), cy - midRadius * Math.Cos(midAngle));
    }
}
