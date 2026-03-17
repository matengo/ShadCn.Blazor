namespace ShadCn.Blazor.Components.Chart.Internal;

/// <summary>
/// Band scale that maps categorical values to pixel bands with configurable padding.
/// Inspired by D3's scaleBand.
/// </summary>
public class ScaleBand
{
    private readonly string[] _domain;
    private readonly double _rangeMin;
    private readonly double _rangeMax;
    private readonly double _paddingInner;
    private readonly double _paddingOuter;
    private readonly double _bandwidth;
    private readonly double _step;

    public ScaleBand(IEnumerable<string> domain, double rangeMin, double rangeMax,
        double paddingInner = 0.1, double paddingOuter = 0.1)
    {
        _domain = domain.ToArray();
        _rangeMin = rangeMin;
        _rangeMax = rangeMax;
        _paddingInner = paddingInner;
        _paddingOuter = paddingOuter;

        var n = _domain.Length;
        var totalRange = rangeMax - rangeMin;

        if (n == 0)
        {
            _step = 0;
            _bandwidth = 0;
            return;
        }

        // step = totalRange / (n - paddingInner + 2 * paddingOuter)
        _step = totalRange / (n - _paddingInner + 2 * _paddingOuter);
        _bandwidth = _step * (1 - _paddingInner);
    }

    /// <summary>
    /// The width of each band.
    /// </summary>
    public double Bandwidth => _bandwidth;

    /// <summary>
    /// The step size between bands (including padding).
    /// </summary>
    public double Step => _step;

    /// <summary>
    /// Maps a category name to the start pixel position of its band.
    /// </summary>
    public double Scale(string category)
    {
        var index = Array.IndexOf(_domain, category);
        if (index < 0) return _rangeMin;
        return _rangeMin + _paddingOuter * _step + index * _step;
    }

    /// <summary>
    /// Maps a category to the center pixel position of its band.
    /// </summary>
    public double ScaleCenter(string category) => Scale(category) + _bandwidth / 2;

    /// <summary>
    /// Returns all domain values.
    /// </summary>
    public IReadOnlyList<string> Domain => _domain;
}
