namespace ShadCn.Blazor.Primitives.Utilities;

/// <summary>
/// A class-variance-authority (CVA) inspired utility for building variant-based CSS classes.
/// Provides type-safe variant management for component styling.
/// </summary>
/// <typeparam name="TVariant">The variant enum type.</typeparam>
/// <typeparam name="TSize">The size enum type.</typeparam>
public class VariantBuilder<TVariant, TSize>
    where TVariant : struct, Enum
    where TSize : struct, Enum
{
    private string _baseClasses = string.Empty;
    private readonly Dictionary<TVariant, string> _variantClasses = [];
    private readonly Dictionary<TSize, string> _sizeClasses = [];
    private TVariant _defaultVariant;
    private TSize _defaultSize;

    /// <summary>
    /// Sets the base classes that are always applied.
    /// </summary>
    public VariantBuilder<TVariant, TSize> Base(string classes)
    {
        _baseClasses = classes;
        return this;
    }

    /// <summary>
    /// Adds classes for a specific variant.
    /// </summary>
    public VariantBuilder<TVariant, TSize> Variant(TVariant variant, string classes)
    {
        _variantClasses[variant] = classes;
        return this;
    }

    /// <summary>
    /// Adds classes for a specific size.
    /// </summary>
    public VariantBuilder<TVariant, TSize> Size(TSize size, string classes)
    {
        _sizeClasses[size] = classes;
        return this;
    }

    /// <summary>
    /// Sets the default variant.
    /// </summary>
    public VariantBuilder<TVariant, TSize> DefaultVariant(TVariant variant)
    {
        _defaultVariant = variant;
        return this;
    }

    /// <summary>
    /// Sets the default size.
    /// </summary>
    public VariantBuilder<TVariant, TSize> DefaultSize(TSize size)
    {
        _defaultSize = size;
        return this;
    }

    /// <summary>
    /// Builds the final class string for the given variant and size.
    /// </summary>
    public string Build(TVariant? variant = null, TSize? size = null, string? additionalClasses = null)
    {
        var builder = ClassBuilder.Create(_baseClasses);

        var actualVariant = variant ?? _defaultVariant;
        var actualSize = size ?? _defaultSize;

        if (_variantClasses.TryGetValue(actualVariant, out var variantClass))
        {
            builder.Add(variantClass);
        }

        if (_sizeClasses.TryGetValue(actualSize, out var sizeClass))
        {
            builder.Add(sizeClass);
        }

        return builder.Build(additionalClasses);
    }
}

/// <summary>
/// A simpler variant builder with only one variant type (no size).
/// </summary>
/// <typeparam name="TVariant">The variant enum type.</typeparam>
public class VariantBuilder<TVariant>
    where TVariant : struct, Enum
{
    private string _baseClasses = string.Empty;
    private readonly Dictionary<TVariant, string> _variantClasses = [];
    private TVariant _defaultVariant;

    /// <summary>
    /// Sets the base classes that are always applied.
    /// </summary>
    public VariantBuilder<TVariant> Base(string classes)
    {
        _baseClasses = classes;
        return this;
    }

    /// <summary>
    /// Adds classes for a specific variant.
    /// </summary>
    public VariantBuilder<TVariant> Variant(TVariant variant, string classes)
    {
        _variantClasses[variant] = classes;
        return this;
    }

    /// <summary>
    /// Sets the default variant.
    /// </summary>
    public VariantBuilder<TVariant> DefaultVariant(TVariant variant)
    {
        _defaultVariant = variant;
        return this;
    }

    /// <summary>
    /// Builds the final class string for the given variant.
    /// </summary>
    public string Build(TVariant? variant = null, string? additionalClasses = null)
    {
        var builder = ClassBuilder.Create(_baseClasses);

        var actualVariant = variant ?? _defaultVariant;

        if (_variantClasses.TryGetValue(actualVariant, out var variantClass))
        {
            builder.Add(variantClass);
        }

        return builder.Build(additionalClasses);
    }
}
