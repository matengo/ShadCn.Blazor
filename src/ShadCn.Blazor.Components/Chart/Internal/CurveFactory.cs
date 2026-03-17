namespace ShadCn.Blazor.Components.Chart.Internal;

/// <summary>
/// Generates SVG path "d" strings for different curve types.
/// Dispatches to the appropriate interpolation method.
/// </summary>
public static class CurveFactory
{
    /// <summary>
    /// Generates a line path through the given points using the specified curve type.
    /// </summary>
    public static string LinePath(IReadOnlyList<(double X, double Y)> points, CurveType curveType)
    {
        if (points.Count == 0) return string.Empty;

        return curveType switch
        {
            CurveType.Monotone => CurveMonotone.Path(points),
            CurveType.Step => StepPath(points),
            CurveType.Natural => CurveMonotone.Path(points), // close approximation
            _ => LinearPath(points)
        };
    }

    /// <summary>
    /// Generates a filled area path (line + close to baseline).
    /// </summary>
    public static string AreaPath(IReadOnlyList<(double X, double Y)> points, double baseline, CurveType curveType)
    {
        if (points.Count == 0) return string.Empty;

        return curveType switch
        {
            CurveType.Monotone => CurveMonotone.AreaPath(points, baseline),
            CurveType.Step => StepAreaPath(points, baseline),
            CurveType.Natural => CurveMonotone.AreaPath(points, baseline),
            _ => LinearAreaPath(points, baseline)
        };
    }

    private static string LinearPath(IReadOnlyList<(double X, double Y)> points)
    {
        var path = new SvgPathBuilder();
        path.MoveTo(points[0].X, points[0].Y);
        for (int i = 1; i < points.Count; i++)
        {
            path.LineTo(points[i].X, points[i].Y);
        }
        return path.Build();
    }

    private static string LinearAreaPath(IReadOnlyList<(double X, double Y)> points, double baseline)
    {
        var path = new SvgPathBuilder();
        path.MoveTo(points[0].X, baseline);
        path.LineTo(points[0].X, points[0].Y);
        for (int i = 1; i < points.Count; i++)
        {
            path.LineTo(points[i].X, points[i].Y);
        }
        path.LineTo(points[^1].X, baseline);
        path.ClosePath();
        return path.Build();
    }

    private static string StepPath(IReadOnlyList<(double X, double Y)> points)
    {
        var path = new SvgPathBuilder();
        path.MoveTo(points[0].X, points[0].Y);
        for (int i = 1; i < points.Count; i++)
        {
            var midX = (points[i - 1].X + points[i].X) / 2;
            path.HorizontalLineTo(midX);
            path.VerticalLineTo(points[i].Y);
            path.HorizontalLineTo(points[i].X);
        }
        return path.Build();
    }

    private static string StepAreaPath(IReadOnlyList<(double X, double Y)> points, double baseline)
    {
        var path = new SvgPathBuilder();
        path.MoveTo(points[0].X, baseline);
        path.LineTo(points[0].X, points[0].Y);
        for (int i = 1; i < points.Count; i++)
        {
            var midX = (points[i - 1].X + points[i].X) / 2;
            path.HorizontalLineTo(midX);
            path.VerticalLineTo(points[i].Y);
            path.HorizontalLineTo(points[i].X);
        }
        path.LineTo(points[^1].X, baseline);
        path.ClosePath();
        return path.Build();
    }
}
