using System.Text;

namespace ShadCn.Blazor.Primitives.Utilities;

/// <summary>
/// A utility class for building CSS class strings dynamically.
/// Similar to classnames/clsx in JavaScript.
/// </summary>
public class ClassBuilder
{
    private readonly List<string> _classes = [];

    public ClassBuilder()
    {
    }

    public ClassBuilder(string? initialClasses)
    {
        if (!string.IsNullOrWhiteSpace(initialClasses))
        {
            _classes.AddRange(initialClasses.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }
    }

    /// <summary>
    /// Adds a class unconditionally.
    /// </summary>
    public ClassBuilder Add(string? className)
    {
        if (!string.IsNullOrWhiteSpace(className))
        {
            _classes.AddRange(className.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }
        return this;
    }

    /// <summary>
    /// Adds a class conditionally.
    /// </summary>
    public ClassBuilder Add(string? className, bool condition)
    {
        if (condition && !string.IsNullOrWhiteSpace(className))
        {
            _classes.AddRange(className.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }
        return this;
    }

    /// <summary>
    /// Adds a class conditionally based on a function.
    /// </summary>
    public ClassBuilder Add(string? className, Func<bool> condition)
    {
        if (condition() && !string.IsNullOrWhiteSpace(className))
        {
            _classes.AddRange(className.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }
        return this;
    }

    /// <summary>
    /// Adds multiple classes conditionally.
    /// </summary>
    public ClassBuilder AddMultiple(params (string? className, bool condition)[] items)
    {
        foreach (var (className, condition) in items)
        {
            Add(className, condition);
        }
        return this;
    }

    /// <summary>
    /// Merges additional classes, typically from a Class parameter.
    /// These classes are added at the end to allow overrides.
    /// </summary>
    public ClassBuilder Merge(string? additionalClasses)
    {
        return Add(additionalClasses);
    }

    /// <summary>
    /// Builds the final class string.
    /// </summary>
    public string Build()
    {
        return string.Join(" ", _classes.Distinct());
    }

    /// <summary>
    /// Builds the final class string with additional merged classes.
    /// </summary>
    public string Build(string? additionalClasses)
    {
        return Merge(additionalClasses).Build();
    }

    public override string ToString() => Build();

    /// <summary>
    /// Creates a new ClassBuilder with initial classes.
    /// </summary>
    public static ClassBuilder Create(string? initialClasses = null) => new(initialClasses);

    /// <summary>
    /// Implicitly convert to string.
    /// </summary>
    public static implicit operator string(ClassBuilder builder) => builder.Build();
}
