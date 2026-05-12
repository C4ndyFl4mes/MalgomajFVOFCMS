using System.Text;
using FastEndpoints;
using FastEndpoints.Swagger;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.API.Data;
using Server.API.Exceptions;
using Server.API.Models;
using Server.API.Routes.ImageFile.DELETE;
using Server.API.Routes.ImageFile.GET;
using Server.API.Routes.ImageFile.POST;
using Server.API.Routes.ImageFile.PUT;
using Server.API.Routes.Menu.GET.State;
using Server.API.Routes.Menu.POST;
using Server.API.Routes.Page.GET.Editor;
using Server.API.Routes.Page.GET.List;
using Server.API.Routes.Page.POST;
using Server.API.Routes.User.SignIn;
using Server.UI;
using Server.UI.Layout;

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
builder.Services.AddFastEndpoints().SwaggerDocument();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<NavigationState>();
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<IValidator<SignInRequest>, SignInValidator>();
builder.Services.AddScoped<IValidator<PostImageRequest>, PostImageRequestValidator>();
builder.Services.AddScoped<IValidator<GetImagesRequest>, GetImagesRequestValidator>();
builder.Services.AddScoped<IValidator<PutImageRequest>, PutImageRequestValidator>();
builder.Services.AddScoped<IValidator<PostMenuRequest>, PostMenuRequestValidator>();

builder.Services.AddScoped<ImagePostData>();
builder.Services.AddScoped<GetImagesData>();
builder.Services.AddScoped<ImagePutData>();
builder.Services.AddScoped<DeleteImageData>();
builder.Services.AddScoped<GetPageEditorData>();
builder.Services.AddScoped<PostPageData>();
builder.Services.AddScoped<GetPageListData>();
builder.Services.AddScoped<GetMenuStateData>();
builder.Services.AddScoped<PostMenuData>();

string secretKey = builder.Configuration["secret_key.txt"] ?? throw new InvalidOperationException("Secret key is not configured.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["issuer.txt"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["audience.txt"],
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuerSigningKey = true
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue("accessToken", out string? token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    });

// Defines WebApplication and the HTTP request pipeline.
WebApplication app = builder.Build();

using var scope = app.Services.CreateScope();
AppDbContext dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
await dbContext.Database.MigrateAsync();

if (!await dbContext.Roles.AnyAsync())
{
    RoleModel adminRole = new()
    {
        Id = Guid.NewGuid(),
        Name = "Administrator",
        Description = "Har fullständig åtkomst till alla funktioner och inställningar."
    };
    RoleModel editorRole = new()
    {
        Id = Guid.NewGuid(),
        Name = "Editor",
        Description = "Kan redigera innehåll och hantera vissa inställningar."
    };
    dbContext.Roles.AddRange(adminRole, editorRole);
    await dbContext.SaveChangesAsync();
}

if (!await dbContext.Users.AnyAsync())
{
    string adminName = builder.Configuration["admin_name.txt"] ??
        throw new InvalidOperationException("Admin name is not configured.");
    string adminEmail = builder.Configuration["admin_email.txt"] ??
        throw new InvalidOperationException("Admin email is not configured.");
    string adminPassword = builder.Configuration["admin_password.txt"] ??
        throw new InvalidOperationException("Admin password is not configured.");
    string roleId = (await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "Administrator"))?.Id.ToString() ??
        throw new InvalidOperationException("Admin role is not configured in the database.");
    
    UserModel adminUser = new()
    {
        Id = Guid.NewGuid(),
        Name = adminName,
        Email = adminEmail,
        PasswordHash = new PasswordHasher<UserModel>().HashPassword(null!, adminPassword),
        RoleId = Guid.Parse(roleId),
        Role = null! // Will automatically be set by EF Core due to the RoleId FK.
    };
    dbContext.Users.Add(adminUser);
    await dbContext.SaveChangesAsync();

}


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseMiddleware<GlobalExceptionHandler>();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints().UseSwaggerGen();

app.Run();