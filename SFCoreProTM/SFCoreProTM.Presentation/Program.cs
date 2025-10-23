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
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

// Menambahkan logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// 1. Menambahkan service untuk Response Compression (Gzip & Brotli)
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

// 2. Menambahkan service untuk Response Caching
builder.Services.AddResponseCaching();

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

// 3. Menggunakan Response Compression di awal pipeline
app.UseResponseCompression();

app.UseGlobalExceptionHandler();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// 5. Menambahkan Caching untuk File Statis (JS, CSS, Gambar, dll.)
//var cachePeriod = "20"; //"604800"; // 7 hari dalam detik
// app.UseStaticFiles(new StaticFileOptions
// {
//     OnPrepareResponse = ctx =>
//     {
//         ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={cachePeriod}");
//     }
// });

// 4. Menggunakan Response Caching
app.UseResponseCaching();

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