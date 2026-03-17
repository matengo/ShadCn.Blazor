namespace ShadCn.Blazor.Components.Chart.Internal;

/// <summary>
/// Linear scale that maps a continuous numeric domain to a continuous pixel range.
/// Inspired by D3's scaleLinear.
/// </summary>
public class ScaleLinear
{
    public double DomainMin { get; private set; }
    public double DomainMax { get; private set; }
    public double RangeMin { get; private set; }
    public double RangeMax { get; private set; }

    public ScaleLinear(double domainMin, double domainMax, double rangeMin, double rangeMax)
    {
        DomainMin = domainMin;
        DomainMax = domainMax;
        RangeMin = rangeMin;
        RangeMax = rangeMax;
    }

    /// <summary>
    /// Maps a data value to a pixel position.
    /// </summary>
    public double Scale(double value)
    {
        var domainRange = DomainMax - DomainMin;
        if (domainRange == 0) return RangeMin;
        var t = (value - DomainMin) / domainRange;
        return RangeMin + t * (RangeMax - RangeMin);
    }

    /// <summary>
    /// Inverse mapping: pixel position → data value.
    /// </summary>
    public double Invert(double pixel)
    {
        var rangeSpan = RangeMax - RangeMin;
        if (rangeSpan == 0) return DomainMin;
        var t = (pixel - RangeMin) / rangeSpan;
        return DomainMin + t * (DomainMax - DomainMin);
    }

    /// <summary>
    /// Generates evenly-spaced tick values.
    /// </summary>
    public double[] Ticks(int count = 5) =>
        ChartMath.NiceTicks(DomainMin, DomainMax, count);

    /// <summary>
    /// Creates a new ScaleLinear with a "nice" domain (rounded to clean tick values).
    /// </summary>
    public ScaleLinear Nice(int tickCount = 5)
    {
        var (nMin, nMax) = ChartMath.NiceExtent(DomainMin, DomainMax, tickCount);
        return new ScaleLinear(nMin, nMax, RangeMin, RangeMax);
    }
}
