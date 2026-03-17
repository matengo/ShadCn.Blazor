using System.Globalization;
using System.Text;

namespace ShadCn.Blazor.Components.Chart.Internal;

/// <summary>
/// Fluent builder for SVG path "d" attribute strings.
/// Uses invariant culture for number formatting to ensure consistent output.
/// </summary>
public class SvgPathBuilder
{
    private readonly StringBuilder _sb = new();

    public SvgPathBuilder MoveTo(double x, double y)
    {
        AppendCommand('M', x, y);
        return this;
    }

    public SvgPathBuilder LineTo(double x, double y)
    {
        AppendCommand('L', x, y);
        return this;
    }

    public SvgPathBuilder HorizontalLineTo(double x)
    {
        _sb.Append('H');
        _sb.Append(Format(x));
        return this;
    }

    public SvgPathBuilder VerticalLineTo(double y)
    {
        _sb.Append('V');
        _sb.Append(Format(y));
        return this;
    }

    public SvgPathBuilder CurveTo(double cx1, double cy1, double cx2, double cy2, double x, double y)
    {
        _sb.Append('C');
        _sb.Append(Format(cx1)); _sb.Append(','); _sb.Append(Format(cy1));
        _sb.Append(' ');
        _sb.Append(Format(cx2)); _sb.Append(','); _sb.Append(Format(cy2));
        _sb.Append(' ');
        _sb.Append(Format(x)); _sb.Append(','); _sb.Append(Format(y));
        return this;
    }

    public SvgPathBuilder ArcTo(double rx, double ry, double rotation, bool largeArc, bool sweep, double x, double y)
    {
        _sb.Append('A');
        _sb.Append(Format(rx)); _sb.Append(','); _sb.Append(Format(ry));
        _sb.Append(' ');
        _sb.Append(Format(rotation));
        _sb.Append(' ');
        _sb.Append(largeArc ? '1' : '0');
        _sb.Append(' ');
        _sb.Append(sweep ? '1' : '0');
        _sb.Append(' ');
        _sb.Append(Format(x)); _sb.Append(','); _sb.Append(Format(y));
        return this;
    }

    public SvgPathBuilder ClosePath()
    {
        _sb.Append('Z');
        return this;
    }

    public override string ToString() => _sb.ToString();

    public string Build() => _sb.ToString();

    private void AppendCommand(char cmd, double x, double y)
    {
        _sb.Append(cmd);
        _sb.Append(Format(x));
        _sb.Append(',');
        _sb.Append(Format(y));
    }

    private static string Format(double value) =>
        value.ToString("G6", CultureInfo.InvariantCulture);
}
