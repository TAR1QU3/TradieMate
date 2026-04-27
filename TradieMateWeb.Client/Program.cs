using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TradieMateWeb.Client;
using TradieMateWeb.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://tradiemate.onrender.com/api/")
});

builder.Services.AddScoped<AuthService>();

var host = builder.Build();

// Initialize auth from session
var auth = host.Services.GetRequiredService<AuthService>();
await auth.InitializeAsync();

await host.RunAsync();