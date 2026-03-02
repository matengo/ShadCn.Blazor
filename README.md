# ShadCn.Blazor

ShadCn.Blazor is a Blazor component library inspired by shadcn/ui. It provides accessible, customizable, open source UI components for Blazor WebAssembly and Blazor Server, with a default theme driven by CSS variables.

## Highlights

- Built for .NET 8+ and both Blazor WebAssembly and Blazor Server.
- Components package plus a default theme package.
- Dark mode support via a `dark` class on the `html` element.
- Easy customization through CSS variables and a `Class` parameter on components.

## Quickstart

### 1) Install NuGet packages

```bash
dotnet add package ShadCn.Blazor.Components
dotnet add package ShadCn.Blazor.Theme.Default
```

### 2) Add the theme CSS

Add this to `index.html` (WebAssembly) or `_Host.cshtml` / `App.razor` (Server):

```html
<link rel="stylesheet" href="_content/ShadCn.Blazor.Theme.Default/theme.css" />
```

### 3) Register services

Add this to `Program.cs`:

```csharp
builder.Services.AddShadCnBlazorComponents();
```
Add `using ShadCn.Blazor.Components;` if needed.

### 4) Add component imports

Add component namespaces to your `_Imports.razor`:

```razor
@using ShadCn.Blazor.Components
@using ShadCn.Blazor.Components.Alert
@using ShadCn.Blazor.Components.Badge
@using ShadCn.Blazor.Components.Button
@using ShadCn.Blazor.Components.Card
@using ShadCn.Blazor.Components.Input
@using ShadCn.Blazor.Components.Separator
```

### 5) Use components

```razor
<Button>Click me</Button>

<Card>
    <CardHeader>
        <CardTitle>Welcome</CardTitle>
        <CardDescription>Get started with ShadCn.Blazor</CardDescription>
    </CardHeader>
    <CardContent>
        <p>Your content here</p>
    </CardContent>
</Card>
```
