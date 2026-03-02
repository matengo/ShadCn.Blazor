using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ShadCn.Blazor.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddShadCnBlazorComponents();

await builder.Build().RunAsync();
