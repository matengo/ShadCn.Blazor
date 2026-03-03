# Dialog Service Design & Implementation Plan

## Overview
This document outlines the design and implementation of the `DialogService` for `ShadCn.Blazor`. The goal is to provide a programmatic API for opening dialogs, similar to `SonnerService` for toasts, allowing developers to invoke dialogs from C# code, pass data to them, and await results.

## Architecture

### Components
1.  **IDialogService**: The public interface for opening and closing dialogs.
2.  **DialogService**: The implementation that manages the state of active dialogs.
3.  **DialogProvider**: A Razor component that subscribes to `DialogService` events and renders the active dialogs using `DynamicComponent`.
4.  **DialogReference**: A handle for an open dialog, containing its ID, parameters, and a `TaskCompletionSource` for the result.
5.  **DialogContext**: A cascading object passed to the dialog content component, allowing it to call `Close(result)`.

### Data Flow
1.  Developer calls `DialogService.ShowAsync<TComponent, TResult, TInput>(inputModel)`.
2.  `DialogService` creates a `DialogReference`, adds it to an internal list, and triggers an update event.
3.  `DialogProvider` re-renders, iterating through the list of active dialogs.
4.  Each dialog is rendered inside a `Dialog` component (from ShadCn), with the specified content component rendered via `DynamicComponent`.
5.  The content component receives the input model as a `[Parameter]` (named `Data`) and the `DialogContext` as a `[CascadingParameter]`.
6.  The user interacts with the dialog.
7.  The content component calls `DialogContext.Close(result)`.
8.  `DialogService` removes the dialog from the list, triggers an update (closing the UI), and sets the result on the `TaskCompletionSource`.
9.  The original `ShowAsync` call completes, returning the result to the developer.

## API Usage

```csharp
// Inject the service
@inject IDialogService DialogService

// Define input and result types
public class UserProfile { ... }
public class DialogResult { ... }

// Open the dialog
var input = new UserProfile { Name = "John" };
var result = await DialogService.ShowAsync<EditProfileDialog, DialogResult, UserProfile>(input);

if (result.Success)
{
    // Handle success
}
```

## Implementation Details

### DialogService.cs
-   Maintains a `List<DialogReference>`.
-   `ShowAsync` methods return `Task<TResult>`.
-   Uses `TaskCompletionSource` to bridge the UI interaction with the async method call.

### DialogProvider.razor
-   Placed in `MainLayout.razor` (or `App.razor`).
-   Renders `Dialog` components dynamically.
-   Cascades `DialogContext` to children.

### DialogContext.cs
-   Provides `Close(object result)` and `Dismiss()` methods.
-   Wraps the internal logic to keep the API clean for component authors.

## Future Considerations
-   Support for multiple stacked dialogs (z-index handling).
-   Animation customization.
-   Global configuration for dialog defaults.
