using Microsoft.AspNetCore.Components;

namespace ShadCn.Blazor.Components.Dialog;

public class DialogOptions
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string Width { get; set; } = "max-w-lg";
    public bool Dismissable { get; set; } = true;
    public string? Class { get; set; }
}

public class DialogParameters : Dictionary<string, object>
{
    public DialogParameters() { }
    public DialogParameters(IDictionary<string, object> dictionary) : base(dictionary) { }

    public new void Add(string name, object value) => base.Add(name, value);
    public T? Get<T>(string name) => this.TryGetValue(name, out var value) ? (T)value : default;
}

public class DialogReference
{
    public Guid Id { get; } = Guid.NewGuid();
    public TaskCompletionSource<object?> TaskCompletionSource { get; } = new();
    public Type? ComponentType { get; }
    public DialogParameters Parameters { get; }
    public DialogOptions Options { get; }
    public RenderFragment? Content { get; }

    public DialogReference(Type componentType, DialogParameters parameters, DialogOptions options)
    {
        ComponentType = componentType;
        Parameters = parameters;
        Options = options;
    }
    
    // For rendering a simple RenderFragment directly if needed
    public DialogReference(RenderFragment content, DialogOptions options)
    {
        Content = content;
        Options = options;
        Parameters = new DialogParameters();
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

public class DialogContext
{
    private readonly DialogReference _reference;
    private readonly DialogService _service;

    public DialogContext(DialogReference reference, DialogService service)
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
