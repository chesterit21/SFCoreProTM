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
builder.Services.AddHttpClient();

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

// Proxy for llama-server to bypass CORS
app.Map("/api/v1/chat/{**catchall}", async (HttpContext ctx, IHttpClientFactory httpFactory, CancellationToken cancellationToken) =>
{
    if (HttpMethods.IsOptions(ctx.Request.Method))
    {
        ctx.Response.Headers["Access-Control-Allow-Origin"] = ctx.Request.Headers.Origin.ToString() ?? "*";
        ctx.Response.Headers["Access-control-allow-methods"] = "POST, OPTIONS";
        ctx.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type, Authorization";
        ctx.Response.StatusCode = 204;
        return;
    }

    var client = httpFactory.CreateClient();

    var targetUri = new Uri($"http://127.0.0.1:8083/v1/chat/{ctx.GetRouteValue("catchall")}{ctx.Request.QueryString}");
    using var req = new HttpRequestMessage(new HttpMethod(ctx.Request.Method), targetUri);

    if (ctx.Request.ContentLength > 0)
    {
        req.Content = new StreamContent(ctx.Request.Body);
        if (ctx.Request.ContentType is not null)
            req.Content.Headers.TryAddWithoutValidation("Content-Type", ctx.Request.ContentType);
    }

    using var res = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

    ctx.Response.StatusCode = (int)res.StatusCode;

    // Copy Content-Type from target response. For SSE, it should be "text/event-stream".
    if (res.Content.Headers.ContentType != null)
    {
        ctx.Response.ContentType = res.Content.Headers.ContentType.ToString();
    }

    // Set other necessary headers for SSE.
    ctx.Response.Headers["Cache-Control"] = "no-cache";
    ctx.Response.Headers["Connection"] = "keep-alive";
    ctx.Response.Headers["X-Accel-Buffering"] = "no";
    ctx.Response.Headers.Remove("transfer-encoding");

    // Now we can start sending the response body
    await res.Content.CopyToAsync(ctx.Response.Body, cancellationToken);
});

// ✅ Proxy DMR dengan dukungan streaming (SSE compatible)
app.Map("/dmr/{**catchall}", async (HttpContext ctx, IHttpClientFactory httpFactory) =>
{
    if (HttpMethods.IsOptions(ctx.Request.Method))
    {
        ctx.Response.Headers["Access-Control-Allow-Origin"] = ctx.Request.Headers.Origin.ToString() ?? "*";
        ctx.Response.Headers["Access-Control-Allow-Methods"] = "GET,POST,OPTIONS";
        ctx.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type,Authorization";
        ctx.Response.StatusCode = 204;
        return;
    }

    var client = httpFactory.CreateClient();

    var targetUri = new Uri($"http://127.0.0.1:12434{ctx.Request.Path}{ctx.Request.QueryString}");
    using var req = new HttpRequestMessage(new HttpMethod(ctx.Request.Method), targetUri);

    // Copy body jika ada
    if (ctx.Request.ContentLength > 0)
    {
        req.Content = new StreamContent(ctx.Request.Body);
        if (ctx.Request.ContentType is not null)
            req.Content.Headers.TryAddWithoutValidation("Content-Type", ctx.Request.ContentType);
    }

    // Kirim request ke DMR dan stream langsung ke client
    using var res = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ctx.RequestAborted);

    ctx.Response.StatusCode = (int)res.StatusCode;
    ctx.Response.Headers["Access-Control-Allow-Origin"] = "*";
    ctx.Response.Headers["Access-Control-Expose-Headers"] = "*";
    ctx.Response.Headers["Cache-Control"] = "no-cache";
    ctx.Response.Headers.Remove("transfer-encoding");

    // ⚡️ Stream body langsung tanpa buffer (biar real-time)
    await using var responseStream = await res.Content.ReadAsStreamAsync(ctx.RequestAborted);
    await responseStream.CopyToAsync(ctx.Response.Body, ctx.RequestAborted);
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();



app.Run();