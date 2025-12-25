using DingConnect.State;
using DingTopUp.Integration.Api;
using DingTopUp.Integration.Auth;
using DingTopUp.Integration.Options;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// Bind Ding options
builder.Services.Configure<DingOptions>(builder.Configuration.GetSection("Ding"));

// Caches & handlers
builder.Services.AddSingleton<OAuthTokenCache>();
builder.Services.AddTransient<OAuthHandler>();
builder.Services.AddTransient<CorrelationHandler>();

// OAuth HttpClient (for token requests)
builder.Services.AddHttpClient("oauth")
    .ConfigurePrimaryHttpMessageHandler(() => DevHttpHandlers.CreateUnsafeHandler());

// Ding API HttpClient (for GetBalance and later others)
builder.Services.AddHttpClient<IDingApi, DingApi>((sp, http) =>
{
    var opt = sp.GetRequiredService<IOptions<DingOptions>>().Value;
    http.BaseAddress = new Uri(opt.BaseUrl); // we configured trailing slash in appsettings
})
.ConfigurePrimaryHttpMessageHandler(() => DevHttpHandlers.CreateUnsafeHandler())
.AddHttpMessageHandler<CorrelationHandler>()
.AddHttpMessageHandler<OAuthHandler>()
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .OrResult(r => (int)r.StatusCode == 429)
    .WaitAndRetryAsync(3, i =>
        TimeSpan.FromSeconds(Math.Pow(2, i)) +
        TimeSpan.FromMilliseconds(Random.Shared.Next(100, 400))));

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<TopUpState>();
builder.Services.Configure<DingConnect.State.TransfersOptions>(
    builder.Configuration.GetSection("Transfers"));


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
