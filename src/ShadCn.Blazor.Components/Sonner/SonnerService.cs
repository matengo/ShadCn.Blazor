namespace ShadCn.Blazor.Components;

public sealed class SonnerService : ISonnerService
{
    private readonly List<SonnerToast> _toasts = new();
    private readonly Dictionary<Guid, CancellationTokenSource> _timers = new();

    public event Action? OnChange;

    public IReadOnlyList<SonnerToast> Toasts => _toasts;

    public Guid Show(SonnerToast toast)
    {
        _toasts.Add(toast);

        if (toast.Duration > 0)
        {
            var cts = new CancellationTokenSource();
            _timers[toast.Id] = cts;
            _ = DismissAfterAsync(toast.Id, toast.Duration, cts.Token);
        }

        Notify();
        return toast.Id;
    }

    public Guid Show(string title, string? description = null, SonnerToastVariant variant = SonnerToastVariant.Default, int duration = 4000)
    {
        var toast = new SonnerToast(title, description)
        {
            Variant = variant,
            Duration = duration
        };

        return Show(toast);
    }

    public void Dismiss(Guid id)
    {
        if (_toasts.RemoveAll(toast => toast.Id == id) == 0)
        {
            return;
        }

        if (_timers.TryGetValue(id, out var cts))
        {
            cts.Cancel();
            cts.Dispose();
            _timers.Remove(id);
        }

        Notify();
    }

    public void DismissAll()
    {
        foreach (var cts in _timers.Values)
        {
            cts.Cancel();
            cts.Dispose();
        }

        _timers.Clear();

        if (_toasts.Count == 0)
        {
            return;
        }

        _toasts.Clear();
        Notify();
    }

    private async Task DismissAfterAsync(Guid id, int duration, CancellationToken token)
    {
        try
        {
            await Task.Delay(duration, token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        Dismiss(id);
    }

    private void Notify() => OnChange?.Invoke();
}
