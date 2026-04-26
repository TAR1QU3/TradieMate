using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TradieMateWeb.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// API URL
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("https://tradiemate.onrender.com/api/")
});

await builder.Build().RunAsync();