using BookTrack.Components;
using BookTrack.Services;

LoadLocalEnvironmentFile();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure the HTTP client used by the Open Library service.
builder.Services.AddHttpClient<OpenLibraryService>(client =>
{
    client.BaseAddress = new Uri("https://openlibrary.org/");
    client.Timeout = TimeSpan.FromSeconds(10);
    client.DefaultRequestHeaders.UserAgent.ParseAdd(
        "MarginReadingTracker/0.1 (mailto:d00239038@student.dkit.ie)");
});
builder.Services.AddHttpClient<BigBookApiService>((services, client) =>
{
    client.BaseAddress = new Uri("https://api.bigbookapi.com/");
    client.Timeout = TimeSpan.FromSeconds(30);
    var apiKey = services.GetRequiredService<IConfiguration>()["BigBookApi:ApiKey"];
    if (!string.IsNullOrWhiteSpace(apiKey))
    {
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);
    }
});
builder.Services.AddScoped<ReadingListService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static void LoadLocalEnvironmentFile()
{
    var path = Path.Combine(Directory.GetCurrentDirectory(), ".env");
    if (!File.Exists(path))
    {
        return;
    }

    foreach (var line in File.ReadLines(path))
    {
        var entry = line.Trim();
        if (entry.Length == 0 || entry.StartsWith('#'))
        {
            continue;
        }

        var separator = entry.IndexOf('=');
        if (separator <= 0)
        {
            continue;
        }

        var name = entry[..separator].Trim();
        var value = entry[(separator + 1)..].Trim();
        if (value.Length >= 2 &&
            ((value[0] == '"' && value[^1] == '"') || (value[0] == '\'' && value[^1] == '\'')))
        {
            value = value[1..^1];
        }

        if (Environment.GetEnvironmentVariable(name) is null)
        {
            Environment.SetEnvironmentVariable(name, value);
        }
    }
}
