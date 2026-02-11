using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Configure HttpClient with base address from host
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Add Radzen services
builder.Services.AddRadzenComponents();

// Register custom app services
builder.Services.AddScoped<PoLingual.Web.Client.Services.AppNotificationService>();

await builder.Build().RunAsync();
