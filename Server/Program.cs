using FastEndpoints;
using Server.UI;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Load secrets from the /run/secrets directory if it exists, otherwise load secrets from the .secrets directory (local development).
if (Directory.Exists("/run/secrets"))
{
    builder.Configuration.AddKeyPerFile("/run/secrets", optional: true);
}
else
{
    string contentRoot = builder.Environment.ContentRootPath; // Get the content root path of the application. (/Server)
    if (Directory.Exists(Path.GetFullPath(Path.Combine(contentRoot, "..", ".secrets"))))
    {
        builder.Configuration.AddKeyPerFile(Path.GetFullPath(Path.Combine(contentRoot, "..", ".secrets")), optional: true);
        Console.WriteLine("Loaded secrets from the .secrets directory for local development.");
    }
    else
    {
        throw new InvalidOperationException("Neither the /run/secrets directory nor the .secrets directory was found. Please ensure that one of these directories exists and contains the necessary secrets.");
    }
}

// Set the connection string from either the Docker secrets or the local development secrets.
string? dockerConnectionString = builder.Configuration["app_connection_string.txt"];
if (!string.IsNullOrWhiteSpace(dockerConnectionString))
{
    builder.Configuration["ConnectionStrings:DefaultConnection"] = dockerConnectionString;
}
else
{
    throw new InvalidOperationException("The connection string was not found in the configuration. Please ensure that the app_connection_string.txt file is present in the secrets directory and contains the correct connection string.");
}


// Services:
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddFastEndpoints();


// Defines WebApplication and the HTTP request pipeline.
WebApplication app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.UseFastEndpoints();

app.Run();