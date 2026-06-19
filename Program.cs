using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Authorization;
using MorgueManager.Web;
using MorgueManager.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.HostEnvironment.IsDevelopment() 
    ? "http://localhost:5075" 
    : "https://morguemanager-api.onrender.com";

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ExportService>();
builder.Services.AddScoped<TourService>();
builder.Services.AddSingleton<TemperatureSimulatorService>();

var supabaseSection = builder.Configuration.GetSection("Supabase");
var supabaseUrl = supabaseSection["Url"] ?? "";
var supabaseKey = supabaseSection["Key"] ?? "";
builder.Services.AddSingleton(sp => new Supabase.Client(supabaseUrl, supabaseKey, new Supabase.SupabaseOptions
{
    AutoRefreshToken = true,
    AutoConnectRealtime = true
}));

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<IContactService, ContactService>();

var host = builder.Build();

await host.RunAsync();
