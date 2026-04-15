using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Web;
using FCOldBoysTeamSelector.Services;
using FCOldBoysTeamSelector.Components;
using Microsoft.Extensions.Logging;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<TeamGeneratorService>();
builder.Services.AddScoped<LocalStorageService>();

// Rejestracja SupabaseService z konfiguracją
builder.Services.Configure<SupabaseServiceOptions>(
    builder.Configuration.GetSection("Supabase"));
builder.Services.AddScoped<SupabaseService>();

builder.Logging.AddConfiguration(
    builder.Configuration.GetSection("Logging"));

await builder.Build().RunAsync();

