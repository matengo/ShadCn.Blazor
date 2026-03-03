using Microsoft.AspNetCore.Components;

namespace ShadCn.Blazor.Components.Sheet;

public enum SheetSide
{
    Top,
    Right,
    Bottom,
    Left
}

public class SheetOptions
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public SheetSide Side { get; set; } = SheetSide.Right;
    public bool Dismissable { get; set; } = true;
    public string? Class { get; set; }
}

public class SheetParameters : Dictionary<string, object>
{
    public SheetParameters() { }
    public SheetParameters(IDictionary<string, object> dictionary) : base(dictionary) { }

    public new void Add(string name, object value) => base.Add(name, value);
    public T? Get<T>(string name) => this.TryGetValue(name, out var value) ? (T)value : default;
}

public class SheetReference
{
    public Guid Id { get; } = Guid.NewGuid();
    public TaskCompletionSource<object?> TaskCompletionSource { get; } = new();
    public Type? ComponentType { get; }
    public SheetParameters Parameters { get; }
    public SheetOptions Options { get; }
    public RenderFragment? Content { get; }
    public bool IsClosing { get; set; }
    internal object? PendingResult { get; set; }

    public SheetReference(Type componentType, SheetParameters parameters, SheetOptions options)
    {
        ComponentType = componentType;
        Parameters = parameters;
        Options = options;
    }
    
    // For rendering a simple RenderFragment directly if needed
    public SheetReference(RenderFragment content, SheetOptions options)
    {
        Content = content;
        Options = options;
        Parameters = new SheetParameters();
    }

    public void Close(object? result)
    {
        TaskCompletionSource.TrySetResult(result);
    }
    
    public void Dismiss()
    {
        TaskCompletionSource.TrySetResult(null); // Or TaskCanceledException if preferred
    }
}

public class SheetContext
{
    private readonly SheetReference _reference;
    private readonly SheetService _service;

    public SheetContext(SheetReference reference, SheetService service)
    {
        _reference = reference;
        _service = service;
    }

    public void Close(object? result = null)
    {
        _service.Close(_reference.Id, result);
    }
    
    public void Dismiss()
    {
        _service.Close(_reference.Id, null);
    }
}
