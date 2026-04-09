using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Web;
using FCOldBoysTeamSelector.Services;
using FCOldBoysTeamSelector.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<TeamGeneratorService>();
builder.Services.AddScoped<LocalStorageService>();

await builder.Build().RunAsync();

