using Microsoft.Extensions.DependencyInjection;

namespace ShadCn.Blazor.Components;

/// <summary>
/// Extension methods for registering ShadCn.Blazor component services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds ShadCn.Blazor component services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddShadCnBlazorComponents(this IServiceCollection services)
    {
        services.AddScoped<ISonnerService, SonnerService>();
        
        // Register DialogService as concrete type first
        services.AddScoped<DialogService>();
        // Then register IDialogService to resolve to the same instance
        services.AddScoped<IDialogService>(sp => sp.GetRequiredService<DialogService>());

        // Register SheetService as concrete type first
        services.AddScoped<SheetService>();
        // Then register ISheetService to resolve to the same instance
        services.AddScoped<ISheetService>(sp => sp.GetRequiredService<SheetService>());

        return services;
    }
}
