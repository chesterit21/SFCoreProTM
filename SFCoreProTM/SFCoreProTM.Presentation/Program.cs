using FluentValidation.AspNetCore;
using SFCoreProTM.Application.Extensions;
using SFCoreProTM.Application.Interfaces.Security;
using SFCoreProTM.Persistence.Extensions;
using SFCoreProTM.Presentation.Middlewares;
using SFCoreProTM.Presentation.Extensions;
using System;
using SFCoreProTM.Presentation.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Menambahkan logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddApplicationLayer();
builder.Services.AddPersistenceLayer(builder.Configuration);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAutoMapper(_ => { }, AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSingleton<ViteManifestService>(); //
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Menambahkan session management
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() ? 
        Microsoft.AspNetCore.Http.CookieSecurePolicy.None : 
        Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
});

var app = builder.Build();

app.UseGlobalExceptionHandler();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
// Menambahkan middleware khusus untuk memastikan klaim pengguna tersedia dalam konteks MVC
app.UseMiddleware<UserClaimsMiddleware>();
// Menambahkan session middleware
app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();