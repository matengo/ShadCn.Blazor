using ShadCn.Blazor.Primitives.Utilities;

namespace ShadCn.Blazor.Components.Badge;

/// <summary>
/// Badge variant styling definitions.
/// </summary>
public static class BadgeVariants
{
    private static readonly VariantBuilder<BadgeVariant> _builder =
        new VariantBuilder<BadgeVariant>()
            .Base("inline-flex items-center rounded-full border px-2.5 py-0.5 text-xs font-semibold " +
                  "transition-colors focus:outline-none focus:ring-2 focus:ring-ring focus:ring-offset-2")
            .Variant(BadgeVariant.Default, "border-transparent bg-primary text-primary-foreground hover:bg-primary/80")
            .Variant(BadgeVariant.Secondary, "border-transparent bg-secondary text-secondary-foreground hover:bg-secondary/80")
            .Variant(BadgeVariant.Destructive, "border-transparent bg-destructive/15 text-destructive hover:bg-destructive/25")
            .Variant(BadgeVariant.Outline, "text-foreground")
            .Variant(BadgeVariant.Ghost, "border-transparent hover:bg-accent hover:text-accent-foreground")
            .DefaultVariant(BadgeVariant.Default);

    public static string GetClasses(BadgeVariant variant, string? additionalClasses = null)
        => _builder.Build(variant, additionalClasses);
}
