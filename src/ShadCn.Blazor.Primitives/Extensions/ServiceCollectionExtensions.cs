using Microsoft.Extensions.DependencyInjection;

namespace ShadCn.Blazor.Primitives.Extensions;

/// <summary>
/// Extension methods for registering ShadCn.Blazor services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds ShadCn.Blazor services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddShadCnBlazor(this IServiceCollection services)
    {
        // Register primitive services
        // TODO: Register FloatingService when implemented
        // TODO: Register OverlayManager when implemented
        
        return services;
    }
}
