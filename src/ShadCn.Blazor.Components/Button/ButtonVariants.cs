using ShadCn.Blazor.Primitives.Utilities;

namespace ShadCn.Blazor.Components.Button;

/// <summary>
/// Button variant styling definitions using CVA-like pattern.
/// </summary>
public static class ButtonVariants
{
    private static readonly VariantBuilder<ButtonVariant, ButtonSize> _builder =
        new VariantBuilder<ButtonVariant, ButtonSize>()
            .Base("inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-md text-sm font-medium " +
                  "ring-offset-background transition-colors focus-visible:outline-none focus-visible:ring-2 " +
                  "focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50 " +
                  "[&_svg]:pointer-events-none [&_svg]:size-4 [&_svg]:shrink-0")
            .Variant(ButtonVariant.Default, "bg-primary text-primary-foreground hover:bg-primary/90")
            .Variant(ButtonVariant.Destructive, "bg-destructive/15 text-destructive hover:bg-destructive/25")
            .Variant(ButtonVariant.Outline, "border border-input bg-background hover:bg-accent hover:text-accent-foreground")
            .Variant(ButtonVariant.Secondary, "bg-secondary text-secondary-foreground hover:bg-secondary/80")
            .Variant(ButtonVariant.Ghost, "hover:bg-accent hover:text-accent-foreground")
            .Variant(ButtonVariant.Link, "text-primary underline-offset-4 hover:underline")
            .Size(ButtonSize.Default, "h-10 px-4 py-2")
            .Size(ButtonSize.Sm, "h-9 rounded-md px-3")
            .Size(ButtonSize.Lg, "h-11 rounded-md px-8")
            .Size(ButtonSize.Icon, "h-10 w-10")
            .DefaultVariant(ButtonVariant.Default)
            .DefaultSize(ButtonSize.Default);

    /// <summary>
    /// Gets the CSS classes for the given button variant and size.
    /// </summary>
    public static string GetClasses(ButtonVariant variant, ButtonSize size, string? additionalClasses = null)
        => _builder.Build(variant, size, additionalClasses);
}
