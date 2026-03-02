namespace ShadCn.Blazor.Components;

public interface ISonnerService
{
    event Action? OnChange;
    IReadOnlyList<SonnerToast> Toasts { get; }
    Guid Show(SonnerToast toast);
    Guid Show(string title, string? description = null, SonnerToastVariant variant = SonnerToastVariant.Default, int duration = 4000);
    void Dismiss(Guid id);
    void DismissAll();
}
