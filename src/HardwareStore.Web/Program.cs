using System.Security.Claims;
using HardwareStore.Application.Configuration;
using HardwareStore.Domain.Enums;
using HardwareStore.Infrastructure.Configuration;
using HardwareStore.Infrastructure.Options;
using HardwareStore.Infrastructure.Persistence;
using HardwareStore.Web.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

var storageOptions = builder.Configuration.GetSection(StorageOptions.SectionName).Get<StorageOptions>() ?? new StorageOptions();
var logDirectory = Path.IsPathRooted(storageOptions.LogDirectory)
    ? storageOptions.LogDirectory
    : Path.Combine(builder.Environment.ContentRootPath, storageOptions.LogDirectory);
Directory.CreateDirectory(logDirectory);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddProvider(new PlainTextFileLoggerProvider(Path.Combine(logDirectory, "hardware-store.log")));

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, UserRole.Admin.ToString()));
    options.AddPolicy("CashierOrAdmin", policy => policy.RequireClaim(ClaimTypes.Role, UserRole.Admin.ToString(), UserRole.Cashier.ToString()));
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AuthorizePage("/Products/Index", "CashierOrAdmin");
    options.Conventions.AuthorizePage("/Sales/New", "CashierOrAdmin");
    options.Conventions.AuthorizePage("/Sales/History", "CashierOrAdmin");
    options.Conventions.AuthorizePage("/Sales/Receipt", "CashierOrAdmin");
    options.Conventions.AuthorizePage("/Sales/Reprint", "CashierOrAdmin");
    options.Conventions.AuthorizePage("/Products/Edit", "AdminOnly");
    options.Conventions.AuthorizeFolder("/Settings", "AdminOnly");
    options.Conventions.AuthorizeFolder("/Backup", "AdminOnly");
    options.Conventions.AuthorizeFolder("/Categories", "AdminOnly");
    options.Conventions.AuthorizeFolder("/Suppliers", "AdminOnly");
    options.Conventions.AuthorizeFolder("/Purchases", "AdminOnly");
});

var app = builder.Build();

await DbInitializer.InitializeAsync(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
