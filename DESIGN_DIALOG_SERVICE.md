# Design Document: DialogService

## Overview
This document outlines the design and implementation plan for a `DialogService` in `ShadCn.Blazor`. The goal is to provide a programmatic way to open dialogs from C# code, passing data to them and awaiting a result, similar to how `SonnerService` works for toasts but with a focus on user interaction and data return.

## Objective
Enable developers to:
1.  Open a dialog dynamically using a C# service.
2.  Pass a model/input (`TInput`) to the dialog component.
3.  Await the dialog closing and receive a result (`TResult`).
4.  Reuse the existing `Dialog` UI components (`Dialog`, `DialogContent`, etc.).

## Proposed API

### IDialogService
```csharp
public interface IDialogService
{
    /// <summary>
    /// Shows a dialog with a specific component and waits for the result.
    /// The input model is passed as a parameter named "Data".
    /// </summary>
    Task<TResult> ShowAsync<TComponent, TResult, TInput>(TInput model, DialogOptions? options = null) 
        where TComponent : ComponentBase;

    /// <summary>
    /// Shows a dialog with a specific component and parameters, waiting for the result.
    /// </summary>
    Task<TResult> ShowAsync<TComponent, TResult>(DialogParameters parameters, DialogOptions? options = null) 
        where TComponent : ComponentBase;
}
```

### DialogOptions
Configuration for the dialog appearance and behavior.
```csharp
public class DialogOptions
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string Width { get; set; } = "max-w-lg";
    public bool Dismissable { get; set; } = true;
    // Other styling options
}
```

### DialogParameters
A dictionary wrapper for passing arbitrary parameters to the component.
```csharp
public class DialogParameters : Dictionary<string, object>
{
    public void Add(string name, object value) => base.Add(name, value);
    public T Get<T>(string name) => (T)this[name];
}
```

## Architecture

### 1. DialogService (Singleton/Scoped)
Manages the list of active dialogs and notifies the `DialogProvider`.

- **State**: `List<DialogReference> _dialogs`
- **Events**: `event Action OnChange;`
- **Methods**:
    - `ShowAsync(...)`: Creates a `DialogReference`, adds it to the list, triggers `OnChange`, and returns `reference.Task`.
    - `Close(Guid id)`: Removes dialog, triggers `OnChange`.

### 2. DialogReference
Represents an open dialog instance.

- `Guid Id`: Unique identifier.
- `TaskCompletionSource<object?> TaskCompletionSource`: To manage the awaitable result.
- `Type ComponentType`: The Razor component to render.
- `DialogParameters Parameters`: Parameters for the component.
- `DialogOptions Options`: Styling/behavior options.
- `void Close(object? result)`: Sets the task result and notifies service to remove.

### 3. DialogProvider (Component)
Placed in `App.razor` or `MainLayout.razor`. Subscribes to `DialogService`.

- **Rendering**:
    - Iterates over active `DialogReference`s.
    - Renders a `Dialog` component for each.
    - Inside `DialogContent`, renders the target `ComponentType` using `DynamicComponent`.
    - **Crucial**: Cascades the `DialogReference` (or a `DialogContext` wrapper) to the child component so it can call `Close()`.

### 4. Integration in Components
The component shown in the dialog (`TComponent`) can access the dialog context to close itself.

```csharp
// MyCustomDialog.razor
@code {
    [CascadingParameter] public DialogContext Context { get; set; }
    [Parameter] public MyInputModel Data { get; set; }

    private void OnSave()
    {
        var result = new MyResult { ... };
        Context.Close(result);
    }
}
```

## Implementation Steps

1.  **Define Core Classes**:
    - `DialogOptions.cs`
    - `DialogParameters.cs`
    - `DialogReference.cs` (Internal state)
    - `DialogContext.cs` (Public API for child components)

2.  **Implement DialogService**:
    - Register as Scoped service.
    - Implement `ShowAsync` logic.

3.  **Implement DialogProvider**:
    - Create `DialogProvider.razor`.
    - Logic to render stacked dialogs (z-index handling if needed, though ShadCn Dialog handles overlay).
    - Handle `OnDismiss` to cancel the task.

4.  **Register Service**:
    - Update `ServiceCollectionExtensions.cs`.

5.  **Add to Layout**:
    - Add `<DialogProvider />` to `App.razor` (likely) or `MainLayout`.

## Usage Example

```csharp
// In a page or component
@inject IDialogService DialogService

private async Task OpenEditDialog()
{
    var model = new EditUserModel { Name = "John" };
    
    // Opens EditUserDialog, passes 'model' as [Parameter] Data
    UserResult result = await DialogService.ShowAsync<EditUserDialog, UserResult, EditUserModel>(model);

    if (result != null)
    {
        // Handle result
    }
}
```
