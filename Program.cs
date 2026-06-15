using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using MorgueManager.Web;
using MorgueManager.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5075") });
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<TemperatureSimulatorService>();

var host = builder.Build();

// Khởi tạo Firebase từ cấu hình trong appsettings.json
var js = host.Services.GetRequiredService<IJSRuntime>();
var firebaseSection = builder.Configuration.GetSection("Firebase");
var firebaseConfig = new Dictionary<string, string?>
{
    { "apiKey", firebaseSection["ApiKey"] },
    { "authDomain", firebaseSection["AuthDomain"] },
    { "projectId", firebaseSection["ProjectId"] },
    { "storageBucket", firebaseSection["StorageBucket"] },
    { "messagingSenderId", firebaseSection["MessagingSenderId"] },
    { "appId", firebaseSection["AppId"] },
    { "measurementId", firebaseSection["MeasurementId"] }
};

try
{
    await js.InvokeVoidAsync("initializeFirebase", firebaseConfig);
}
catch { }

await host.RunAsync();
