# Sheet Component & Service Design

## Overview
The `Sheet` component (also known as a Drawer) is a panel that slides out from the edge of the screen. It is used for navigation, details, or editing forms. This document outlines the implementation of the `Sheet` component and its corresponding `SheetService` for programmatic control.

## Architecture

### Primitives
The implementation will mirror the `Dialog` component structure but with specific animations and positioning for sliding panels.

*   **Sheet**: Root component managing the open state.
*   **SheetTrigger**: Button/element that opens the sheet.
*   **SheetContent**: The main container that slides in. Accepts a `Side` parameter (`Top`, `Right`, `Bottom`, `Left`).
*   **SheetHeader**: Container for title and description.
*   **SheetFooter**: Container for action buttons.
*   **SheetTitle**: The title of the sheet.
*   **SheetDescription**: The description of the sheet.
*   **SheetClose**: A button that closes the sheet.
*   **SheetProvider**: Renders sheets managed by the `SheetService`.

### SheetService
A service to programmatically open sheets, similar to `DialogService`.

*   **ISheetService**: Interface for dependency injection.
*   **SheetService**: Manages the list of active sheets.
*   **SheetReference**: Represents an open sheet, allowing access to its result and methods to close it.
*   **SheetContext**: Cascading parameter providing context to the sheet content (e.g., `Close()`).

### Usage

#### Declarative
```razor
<Sheet>
    <SheetTrigger>Open Sheet</SheetTrigger>
    <SheetContent Side="SheetSide.Right">
        <SheetHeader>
            <SheetTitle>Edit Profile</SheetTitle>
            <SheetDescription>Make changes to your profile here.</SheetDescription>
        </SheetHeader>
        <!-- Content -->
        <SheetFooter>
            <SheetClose>Cancel</SheetClose>
            <Button>Save</Button>
        </SheetFooter>
    </SheetContent>
</Sheet>
```

#### Programmatic
```csharp
@inject ISheetService SheetService

var result = await SheetService.ShowAsync<EditProfileSheet, SheetResult, UserProfile>(
    userProfile, 
    new SheetOptions { Side = SheetSide.Right }
);
```

## Styling & Animation
We will use Tailwind CSS utility classes for animations.

*   **Overlay**: `fixed inset-0 z-50 bg-black/80 data-[state=open]:animate-in data-[state=closed]:animate-out fade-in-0 fade-out-0`
*   **Content Base**: `fixed z-50 gap-4 bg-background p-6 shadow-lg transition ease-in-out data-[state=open]:animate-in data-[state=closed]:animate-out duration-300`
*   **Sides**:
    *   **Top**: `inset-x-0 top-0 border-b data-[state=closed]:slide-out-to-top data-[state=open]:slide-in-from-top`
    *   **Bottom**: `inset-x-0 bottom-0 border-t data-[state=closed]:slide-out-to-bottom data-[state=open]:slide-in-from-bottom`
    *   **Left**: `inset-y-0 left-0 h-full w-3/4 border-r data-[state=closed]:slide-out-to-left data-[state=open]:slide-in-from-left sm:max-w-sm`
    *   **Right**: `inset-y-0 right-0 h-full w-3/4 border-l data-[state=closed]:slide-out-to-right data-[state=open]:slide-in-from-right sm:max-w-sm`

## Implementation Steps
1.  Define `SheetSide` enum and `SheetOptions`.
2.  Implement `SheetService` and `SheetProvider`.
3.  Implement `Sheet` primitives (`Sheet.razor`, `SheetContent.razor`, etc.).
4.  Register services in `ServiceCollectionExtensions`.
5.  Add documentation and examples.
