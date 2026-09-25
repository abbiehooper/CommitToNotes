using CommitToNotes.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBase = builder.Configuration["ApiBaseUrl"];

builder.Services.AddTransient<CredentialsHandler>();
builder.Services.AddHttpClient("Api", c => c.BaseAddress = new Uri(apiBase))
    .AddHttpMessageHandler<CredentialsHandler>();
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CookieAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CookieAuthStateProvider>());
builder.Services.AddScoped<AuthService>();
builder.Services.AddMudServices();

await builder.Build().RunAsync();