using System.IO;
using InventarioGanadero.Api.Repositories;
using InventarioGanadero.Api.Services.Auth;
using InventarioGanadero.Api.Services.Historial;
using InventarioGanadero.Api.Services.Reproduccion;
using InventarioGanadero.Api.Services.Security;
using InventarioGanadero.Api.Services.Tareas;
using InventarioGanadero.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
    builder.WebHost.UseUrls("http://127.0.0.1:5180");

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<RegistroGanaderoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RegistroGanadero")));

builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPartoService, PartoService>();
builder.Services.AddScoped<IAnimalHistorialService, AnimalHistorialService>();
builder.Services.AddScoped<ITareaGanaderaService, TareaGanaderaService>();

var keysPath = builder.Configuration["Hosting:DataProtectionKeysPath"];
var dataProtectionAppName = builder.Configuration["Hosting:ApplicationName"] ?? "RegistroGanadero";
if (!string.IsNullOrWhiteSpace(keysPath))
{
    Directory.CreateDirectory(keysPath);
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
        .SetApplicationName(dataProtectionAppName);
}

var cookieName = builder.Configuration["Hosting:CookieName"] ?? "RegistroGanadero.Auth";

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = cookieName;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddAuthorization();

var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
if (corsOrigins is { Length: > 0 })
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("PublicFrontend", policy =>
            policy.WithOrigins(corsOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());
    });
}

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    var uploadsPath = app.Configuration["Hosting:UploadsPath"];
    if (!string.IsNullOrWhiteSpace(uploadsPath))
        Directory.CreateDirectory(uploadsPath);
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost,
    KnownIPNetworks = { },
    KnownProxies = { }
});

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
{
    app.UseExceptionHandler("/Home/Error");
    if (app.Environment.IsProduction())
        app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

if (corsOrigins is { Length: > 0 })
    app.UseCors("PublicFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<RegistroGanaderoDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var auth = scope.ServiceProvider.GetRequiredService<IAuthService>();
        await AuthDataSeeder.SeedAsync(db, hasher);
        await auth.MigratePlainPasswordsAsync();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Error al inicializar base de datos. Verifique SQL Server y la cadena de conexión.");
        if (app.Environment.IsDevelopment())
            throw;
    }
}

await app.RunAsync();
