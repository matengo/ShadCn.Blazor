using Microsoft.AspNetCore.Components;
using ShadCn.Blazor.Components.Sheet;

namespace ShadCn.Blazor.Components;

public interface ISheetService
{
    Task<TResult> ShowAsync<TComponent, TResult>(Dictionary<string, object> parameters, SheetOptions? options = null) 
        where TComponent : ComponentBase;

    Task<TResult> ShowAsync<TComponent, TResult, TInput>(TInput model, SheetOptions? options = null) 
        where TComponent : ComponentBase;

    void CloseAll();
}

public class SheetService : ISheetService
{
    public event Action? OnChange;
    private readonly List<SheetReference> _sheets = new();

    public IReadOnlyList<SheetReference> Sheets => _sheets.AsReadOnly();

    public async Task<TResult> ShowAsync<TComponent, TResult>(Dictionary<string, object> parameters, SheetOptions? options = null)
        where TComponent : ComponentBase
    {
        var reference = new SheetReference(typeof(TComponent), new SheetParameters(parameters), options ?? new SheetOptions());

        _sheets.Add(reference);
        OnChange?.Invoke();

        var result = await reference.TaskCompletionSource.Task;
        return (TResult)result!;
    }

    public Task<TResult> ShowAsync<TComponent, TResult, TInput>(TInput model, SheetOptions? options = null)
        where TComponent : ComponentBase
    {
        var parameters = new Dictionary<string, object> { { "Data", model! } };
        return ShowAsync<TComponent, TResult>(parameters, options);
    }

    public void Close(Guid id, object? result)
    {
        var reference = _sheets.FirstOrDefault(x => x.Id == id);
        if (reference == null || reference.IsClosing) return;

        reference.IsClosing = true;
        reference.PendingResult = result;
        OnChange?.Invoke();

        _ = RemoveAfterAnimation(reference);
    }

    public void FinalizeClose(Guid id, object? result)
    {
        var reference = _sheets.FirstOrDefault(x => x.Id == id);
        if (reference != null)
        {
            _sheets.Remove(reference);
            reference.TaskCompletionSource.TrySetResult(result);
            OnChange?.Invoke();
        }
    }

    private async Task RemoveAfterAnimation(SheetReference reference)
    {
        await Task.Delay(300);
        _sheets.Remove(reference);
        reference.TaskCompletionSource.TrySetResult(reference.PendingResult);
        OnChange?.Invoke();
    }

    public void CloseAll()
    {
        var all = _sheets.ToList();
        foreach (var reference in all)
        {
            if (!reference.IsClosing)
            {
                reference.IsClosing = true;
                reference.PendingResult = null;
                _ = RemoveAfterAnimation(reference);
            }
        }
        OnChange?.Invoke();
    }
}
