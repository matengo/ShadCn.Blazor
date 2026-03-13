# ShadCn.Blazor

ShadCn.Blazor is a Blazor component library inspired by shadcn/ui. It provides accessible, customizable, open source UI components for Blazor WebAssembly and Blazor Server, with a default theme driven by CSS variables. Components are pure Blazor and require no JavaScript/JS interop at runtime.

**[Live Documentation](https://shadcnblazordocs.ashymoss-4232cb31.swedencentral.azurecontainerapps.io/)**

## Highlights

- Built for .NET 8+ and both Blazor WebAssembly and Blazor Server.
- No JavaScript/JS interop required at runtime.
- Single package install.
- Dark mode support via a `dark` class on the `html` element.
- Easy customization through CSS variables and a `Class` parameter on components.

## Prerequisites

- .NET 8.0 or later
- Blazor WebAssembly or Blazor Server project

## Installation

### 1) Install NuGet package

```bash
dotnet add package ShadCn.Blazor.Components
```

Package Manager Console:

```powershell
Install-Package ShadCn.Blazor.Components
```

### 2) Add the theme CSS

Add this to `index.html` (WebAssembly) or `_Host.cshtml` / `App.razor` (Server):

```html
<link rel="stylesheet" href="_content/ShadCn.Blazor.Theme.Default/theme.css" />
```
The theme uses CSS variables for colors. Values are in HSL format without the `hsl()` wrapper.

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

## Dark mode

Add the `dark` class to your `html` element:

```html
<html class="dark">
  ...
</html>
```
You can toggle dark mode by adding/removing the class (for example via JavaScript or a persisted user preference).

## Customization

All components accept a `Class` parameter for additional Tailwind CSS classes:

```razor
<Button Class="w-full">Full Width Button</Button>
<Card Class="max-w-md mx-auto">...</Card>
```

Override colors by redefining the CSS variables in your own stylesheet (loaded after `theme.css`). Values use HSL format without the `hsl()` wrapper:

```css
:root {
  --background: 0 0% 100%;
  --foreground: 222.2 84% 4.9%;
  --primary: 222.2 47.4% 11.2%;
  --primary-foreground: 210 40% 98%;
  --secondary: 210 40% 96.1%;
  --secondary-foreground: 222.2 47.4% 11.2%;
  --muted: 210 40% 96.1%;
  --muted-foreground: 215.4 16.3% 46.9%;
  --accent: 210 40% 96.1%;
  --accent-foreground: 222.2 47.4% 11.2%;
  --destructive: 0 84.2% 60.2%;
  --destructive-foreground: 210 40% 98%;
  --card: 0 0% 100%;
  --card-foreground: 222.2 84% 4.9%;
  --popover: 0 0% 100%;
  --popover-foreground: 222.2 84% 4.9%;
  --border: 214.3 31.8% 91.4%;
  --input: 214.3 31.8% 91.4%;
  --ring: 222.2 84% 4.9%;
  --radius: 0.5rem;

  /* Sidebar (defaults inherit from the above) */
  --sidebar: var(--background);
  --sidebar-foreground: var(--foreground);
  --sidebar-primary: var(--primary);
  --sidebar-primary-foreground: var(--primary-foreground);
  --sidebar-accent: var(--accent);
  --sidebar-accent-foreground: var(--accent-foreground);
  --sidebar-border: var(--border);
  --sidebar-ring: var(--ring);
}
```

Override dark mode with `.dark { ... }`. Override fonts with `@theme { --font-sans: ...; }`.

Tip: use https://ui.shadcn.com/themes to generate color palettes, then copy the CSS variables into your project.
