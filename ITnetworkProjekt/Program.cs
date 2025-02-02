﻿using ITnetworkProjekt;
using ITnetworkProjekt.Data;
using ITnetworkProjekt.Interfaces;
using ITnetworkProjekt.Managers;
using ITnetworkProjekt.Repositories;
using ITnetworkProjekt.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlServerOptionsAction =>
    {
        sqlServerOptionsAction.EnableRetryOnFailure();
    }));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//Login informations
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddControllersWithViews();
//tady to vymazat i komentáře
//builder.Services.AddScoped<InsuranceService>();
//builder.Services.AddScoped<InsuredPersonService>();


builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddScoped<IInsuranceRepository, InsuranceRepository>();
builder.Services.AddScoped<InsuranceManager>();

builder.Services.AddScoped<IInsuredPersonRepository, InsuredPersonRepository>();
builder.Services.AddScoped<InsuredPersonManager>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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


