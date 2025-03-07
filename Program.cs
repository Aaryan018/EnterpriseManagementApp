using EnterpriseManagementApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EnterpriseManagementApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Using connection string: {connectionString}");
builder.Services.AddDbContext<ManageHousingDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add ASP.NET Core Identity with Role Support
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ManageHousingDbContext>()
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

// Configure authentication cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/SignIn"; // Path to login page
        options.LogoutPath = "/Account/Logout"; // Path to logout page
        options.AccessDeniedPath = "/Account/AccessDenied"; // Path to access denied
        options.Cookie.SameSite = SameSiteMode.Lax; // Cookie configuration
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
    });

// Configure anti-forgery options
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
});

// Build the app
var app = builder.Build();

// Ensure database is reset and seeded
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ManageHousingDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    Console.WriteLine("Resetting database...");
    await dbContext.Database.EnsureDeletedAsync();
    await dbContext.Database.EnsureCreatedAsync();
    Console.WriteLine("Database reset successfully.");

    // Ensure roles are created
    string[] roles = { "Admin", "Manager", "Customer" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
            Console.WriteLine($"Role '{role}' created successfully.");
        }
    }

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

    // Verify database state after seeding
    try
    {
        var renterCount = dbContext.Renters.Count();
        Console.WriteLine($"After seeding: Found {renterCount} renters in the database.");
        foreach (var renter in dbContext.Renters)
        {
            Console.WriteLine($"Renter after seeding: RenterId={renter.RenterId}, Name={renter.Name}");
        }

        var assetCount = dbContext.Assets.Count();
        Console.WriteLine($"After seeding: Found {assetCount} assets in the database.");
        foreach (var asset in dbContext.Assets)
        {
            Console.WriteLine($"Asset after seeding: AssetId={asset.AssetId}, Name={asset.Name}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error querying database after seeding: {ex.Message}");
        Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Add session middleware
app.UseSession();

app.UseRouting();

// Add authentication and authorization middleware
app.UseAuthentication(); // Ensure authentication middleware is here
app.UseAuthorization();

// Middleware for logging requests & responses
app.Use(async (context, next) =>
{
    Console.WriteLine($"Incoming Request: {context.Request.Method} {context.Request.Path} {context.Request.QueryString}");
    Console.WriteLine("Request Cookies:");
    foreach (var cookie in context.Request.Cookies)
    {
        Console.WriteLine($"Cookie: {cookie.Key} = {cookie.Value}");
    }
    try
    {
        await next.Invoke();
        Console.WriteLine($"Response Status: {context.Response.StatusCode}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Middleware error: {ex.Message}");
        Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("An error occurred during request processing: " + ex.Message);
    }
});

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=SignIn}/{id?}");

app.Run();
