using EnterpriseManagementApp;
using EnterpriseManagementApp.Data;
using EnterpriseManagementApp.Models;
using EnterpriseManagementApp.Models.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Verify and log the current environment
Console.WriteLine($"Current Environment: {builder.Environment.EnvironmentName}");

// Database Context - Ensure the connection string is correct in appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add session support for anti-forgery token validation
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
});

// Cookie Settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/SignIn"; // Ensure this matches your sign-in controller/action
    options.AccessDeniedPath = "/Home/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    options.SlidingExpiration = true;

    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EmployeePolicy", policy => policy.RequireRole("Employee"));
    options.AddPolicy("ManagerPolicy", policy => policy.RequireRole("Manager"));
    options.AddPolicy("HRPolicy", policy => policy.RequireClaim("Module", "HR"));
    options.AddPolicy("CarePolicy", policy => policy.RequireClaim("Module", "Care"));
    options.AddPolicy("HousingPolicy", policy => policy.RequireClaim("Module", "Housing"));
});

// MVC setup
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Database Migration & Seeding
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //dbContext.Database.EnsureCreated();


    // dbContext.Database.EnsureCreated();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();


    Console.WriteLine("Resetting database...");
    await dbContext.Database.EnsureDeletedAsync();
    await dbContext.Database.EnsureCreatedAsync();
    Console.WriteLine("Database reset successfully.");


    // Seed data (add users, renters, assets)
    try
    {
        await SeedData.Initialize(scope.ServiceProvider);
        Console.WriteLine("Seeding completed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during seeding: {ex.Message}");
        Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
    }
}

// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Enable detailed errors in Development
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Custom error handling in production
    app.UseHsts(); // HTTP Strict Transport Security for production
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Route Configuration
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();