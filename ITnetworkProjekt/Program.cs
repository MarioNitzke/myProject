﻿using System.Globalization;
using Microsoft.AspNetCore.Localization;
using ITnetworkProjekt;
using ITnetworkProjekt.Data;
using ITnetworkProjekt.Interfaces;
using ITnetworkProjekt.Managers;
using ITnetworkProjekt.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ITnetworkProjekt.Resources;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Localization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlServerOptionsAction =>
    {
        sqlServerOptionsAction.EnableRetryOnFailure();
    }));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Login informations
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddScoped<IInsuranceRepository, InsuranceRepository>();
builder.Services.AddScoped<InsuranceManager>();
builder.Services.AddScoped<IInsuredPersonRepository, InsuredPersonRepository>();
builder.Services.AddScoped<InsuredPersonManager>();

//     @SharedResource.ResourceManager.GetString("EditButton")


builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
        .AddDataAnnotationsLocalization(options => {
            options.DataAnnotationLocalizerProvider = (type, factory) =>
                factory.Create(typeof(SharedResource));
        });


// ✅ Nastavení podporovaných jazyků
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "cs", "en" };
    options.DefaultRequestCulture = new RequestCulture("cs");
    options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
    options.SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
});

var app = builder.Build();

// ✅ Použití lokalizace
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
            throw new Exception($"Nepodařilo se vytvořit admin účet: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }

    if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
        await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

    if (defaultAdminUser is not null && !await userManager.IsInRoleAsync(defaultAdminUser, UserRoles.Admin))
        await userManager.AddToRoleAsync(defaultAdminUser, UserRoles.Admin);
}

app.Run();
