namespace ShadCn.Blazor.Components;

public sealed class SonnerToast
{
    public SonnerToast(string title, string? description = null)
    {
        Title = title;
        Description = description;
    }

    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; set; }
    public string? Description { get; set; }
    public SonnerToastVariant Variant { get; set; } = SonnerToastVariant.Default;
    public int Duration { get; set; } = 4000;
    public bool Dismissible { get; set; } = true;
}
