using DingTopUp.Integration;
using DingTopUp.Integration.Auth;
using DingTopUp.Integration.Options;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// Options
builder.Services.Configure<DingOptions>(builder.Configuration.GetSection("Ding"));

// Handlers & caches
builder.Services.AddSingleton<OAuthTokenCache>();
builder.Services.AddTransient<OAuthHandler>();
builder.Services.AddTransient<CorrelationHandler>();

// OAuth bare client (used by OAuthHandler to call token endpoint)
builder.Services.AddHttpClient("oauth");

// DingApi client
builder.Services.AddHttpClient<IDingApi, DingApi>((sp, http) =>
{
    var opt = sp.GetRequiredService<IOptions<DingOptions>>().Value;
    http.BaseAddress = new Uri(opt.BaseUrl.TrimEnd('/')); // ends with /api/V1
})
.AddHttpMessageHandler<CorrelationHandler>()
.AddHttpMessageHandler<OAuthHandler>()
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .OrResult(r => (int)r.StatusCode == 429)
    .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(Math.Pow(2, i)) + TimeSpan.FromMilliseconds(Random.Shared.Next(100, 400))));

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();
app.UseStaticFiles();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
