using BlogMVC.Db;
using BlogMVC.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.MSSqlServer;

Log.Logger = new LoggerConfiguration().WriteTo
    .Console()
    .WriteTo.MSSqlServer(
        "Server=.; Database=BlogMvc;User Id=sa; Password=sasa@123; TrustServerCertificate= true",
        new MSSqlServerSinkOptions() { TableName = "Tbl_LogEvents", AutoCreateSqlTable = true})
    .CreateLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);
    
    // If using Kestrel:
    builder.Services.Configure<KestrelServerOptions>(options =>
    {
        options.AllowSynchronousIO = true;
    });

// If using IIS:
    builder.Services.Configure<IISServerOptions>(options =>
    {
        options.AllowSynchronousIO = true;
    });

    builder.Services.AddSerilog(); // <-- Add this line

    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(x =>
        {
            x.LoginPath = "/"; // /Home/Index => /Login/Index
        });
    builder.Services.AddAuthorization();
    builder.Services.AddHttpContextAccessor();

    // builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

    builder.Services.AddMemoryCache(); // Adds a default in-memory 
    // implementation of 
    // IDistributedCache

    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(20);
        options.Cookie.IsEssential = true;
    });

    builder.Services.AddTransient<CookieMiddleware>();
    
    builder.Services.AddControllersWithViews();

    builder.Services.AddDbContext<AppDbContext>(
        opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")), ServiceLifetime.Transient,
        ServiceLifetime.Transient);

    var app = builder.Build();
    // app.MapGet("/", () => "Hello World!");

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();


    // Home/Index = /
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}