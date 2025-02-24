using ITnetworkProjekt;
using ITnetworkProjekt.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Razor;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using Serilog;
using MediatR;
using ITnetworkProjekt.Decorators;
using ITnetworkProjekt.Features.Insurances.Queries;
using ITnetworkProjekt.Models;
using ITnetworkProjekt.Extensions;
using ITnetworkProjekt.Data.Entities;
using ITnetworkProjekt.Data.Repositories;
using ITnetworkProjekt.Data.Repositories.Interfaces;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Starting up the application...");

    var builder = WebApplication.CreateBuilder(args);

    // Nastavení Serilogu jako hlavního logovacího systému
    builder.Host.UseSerilog();

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString, sqlServerOptionsAction =>
        {
            sqlServerOptionsAction.EnableRetryOnFailure();
        }));

    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

    builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

    builder.Services.AddScoped<IInsuranceRepository, InsuranceRepository>();
    builder.Services.AddScoped<IInsuredPersonRepository, InsuredPersonRepository>();


    builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

    builder.Services.AddControllersWithViews()
        .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
        .AddDataAnnotationsLocalization(options =>
        {
            options.DataAnnotationLocalizerProvider = (type, factory) =>
                factory.Create(typeof(SharedResource));
        });

    builder.Services.AddOpenTelemetry()
        .WithTracing(tracerProviderBuilder =>
        {
            tracerProviderBuilder
                .AddAspNetCoreInstrumentation() // Sleduje požadavky v ASP.NET Core
                .AddHttpClientInstrumentation() // Sleduje HTTP požadavky
                .AddConsoleExporter();          // Exportuje data do konzole
        })
        .WithMetrics(metricsProviderBuilder =>
        {
            metricsProviderBuilder
                .AddRuntimeInstrumentation()   // Sleduje runtime metriky (paměť, GC, atd.)
                .AddConsoleExporter();         // Exportuje metriky do konzole
        });

    builder.Services.AddScoped<IBaseRepository<Insurance>, InsuranceRepository>();
    builder.Services.AddScoped<IBaseRepository<InsuredPerson>, InsuredPersonRepository>();

    // 1) Registrace MediatR – předáme typ handleru, který je ve stejném assembly jako ostatní CQRS třídy
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetInsuranceForLoggedUserQueryHandler).Assembly));

    // 2
    builder.Services
        .AddEntityHandlers<Insurance, InsuranceViewModel>()
        .AddEntityHandlers<InsuredPerson, InsuredPersonViewModel>();


    // 7) Dekorování všech MediatR handlerů LoggingDecoratorem 
    builder.Services.Decorate(typeof(IRequestHandler<,>), typeof(LoggingDecorator<,>));

    var app = builder.Build();

    app.UseRequestLocalization();

    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    using (IServiceScope scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (!await dbContext.Database.CanConnectAsync()) 
        {
            await dbContext.Database.MigrateAsync();
        }

        RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        UserManager<IdentityUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        string adminEmail = "nitzkemario@gmail.com";
        string adminPassword = "Password123+";

        IdentityUser? defaultAdminUser = await userManager.FindByEmailAsync(adminEmail);

        if (defaultAdminUser == null)
        {
            defaultAdminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                NormalizedUserName = adminEmail.ToUpper(),
                NormalizedEmail = adminEmail.ToUpper(),
                EmailConfirmed = true
            };

            IdentityResult result = await userManager.CreateAsync(defaultAdminUser, adminPassword);
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create admin account: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
            await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

        if (defaultAdminUser is not null && !await userManager.IsInRoleAsync(defaultAdminUser, UserRoles.Admin))
            await userManager.AddToRoleAsync(defaultAdminUser, UserRoles.Admin);
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
