namespace ShadCn.Blazor.Components.Chart.Internal;

/// <summary>
/// Monotone cubic Hermite interpolation for smooth curves that preserve monotonicity.
/// Produces SVG cubic bezier (C) commands. This prevents overshoot at local extrema.
/// Implements the Fritsch-Carlson method.
/// </summary>
public static class CurveMonotone
{
    /// <summary>
    /// Generates an SVG path "d" string for a smooth curve through the given points.
    /// </summary>
    public static string Path(IReadOnlyList<(double X, double Y)> points)
    {
        if (points.Count == 0) return string.Empty;
        if (points.Count == 1)
        {
            var p = points[0];
            return new SvgPathBuilder().MoveTo(p.X, p.Y).Build();
        }
        if (points.Count == 2)
        {
            return new SvgPathBuilder()
                .MoveTo(points[0].X, points[0].Y)
                .LineTo(points[1].X, points[1].Y)
                .Build();
        }

        var tangents = ComputeTangents(points);
        var path = new SvgPathBuilder();
        path.MoveTo(points[0].X, points[0].Y);

        for (int i = 0; i < points.Count - 1; i++)
        {
            var p0 = points[i];
            var p1 = points[i + 1];
            var dx = (p1.X - p0.X) / 3;

            path.CurveTo(
                p0.X + dx, p0.Y + dx * tangents[i],
                p1.X - dx, p1.Y - dx * tangents[i + 1],
                p1.X, p1.Y);
        }

        return path.Build();
    }

    /// <summary>
    /// Generates an SVG path "d" string for a smooth closed area shape
    /// (from curve down to baseline and back).
    /// </summary>
    public static string AreaPath(IReadOnlyList<(double X, double Y)> topPoints, double baseline)
    {
        if (topPoints.Count == 0) return string.Empty;

        var curvePath = Path(topPoints);

        // Close down to baseline
        var last = topPoints[^1];
        var first = topPoints[0];
        var path = new SvgPathBuilder();

        return curvePath +
            new SvgPathBuilder()
                .LineTo(last.X, baseline)
                .LineTo(first.X, baseline)
                .ClosePath()
                .Build();
    }

    /// <summary>
    /// Computes monotone tangent slopes using the Fritsch-Carlson method.
    /// </summary>
    private static double[] ComputeTangents(IReadOnlyList<(double X, double Y)> points)
    {
        int n = points.Count;
        var delta = new double[n - 1];
        var m = new double[n];

        // Step 1: Compute slopes of secant lines
        for (int i = 0; i < n - 1; i++)
        {
            var dx = points[i + 1].X - points[i].X;
            delta[i] = dx == 0 ? 0 : (points[i + 1].Y - points[i].Y) / dx;
        }

        // Step 2: Initialize tangents as average of adjacent secants
        m[0] = delta[0];
        for (int i = 1; i < n - 1; i++)
        {
            m[i] = (delta[i - 1] + delta[i]) / 2;
        }
        m[n - 1] = delta[n - 2];

        // Step 3: Ensure monotonicity
        for (int i = 0; i < n - 1; i++)
        {
            if (Math.Abs(delta[i]) < 1e-10)
            {
                m[i] = 0;
                m[i + 1] = 0;
                continue;
            }

            var alpha = m[i] / delta[i];
            var beta = m[i + 1] / delta[i];
            var s2 = alpha * alpha + beta * beta;

            if (s2 > 9)
            {
                var s = 3 / Math.Sqrt(s2);
                m[i] = s * alpha * delta[i];
                m[i + 1] = s * beta * delta[i];
            }
        }

        return m;
    }
}
