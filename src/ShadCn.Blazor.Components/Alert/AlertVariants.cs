using ShadCn.Blazor.Primitives.Utilities;

namespace ShadCn.Blazor.Components.Alert;

/// <summary>
/// Alert variant styling definitions.
/// </summary>
public static class AlertVariants
{
    private static readonly VariantBuilder<AlertVariant> _builder =
        new VariantBuilder<AlertVariant>()
            .Base("relative w-full rounded-lg border p-4 [&>svg~*]:pl-7 [&>svg+div]:translate-y-[-3px] [&>svg]:absolute [&>svg]:left-4 [&>svg]:top-4 [&>svg]:text-foreground")
            .Variant(AlertVariant.Default, "bg-background text-foreground")
            .Variant(AlertVariant.Destructive, "border-destructive/50 text-destructive dark:border-destructive [&>svg]:text-destructive")
            .DefaultVariant(AlertVariant.Default);

    public static string GetClasses(AlertVariant variant, string? additionalClasses = null)
        => _builder.Build(variant, additionalClasses);
}
