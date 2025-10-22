using Microsoft.AspNetCore.Authentication.Cookies;
using POE_Part2_PROG6212.Models;

var builder = WebApplication.CreateBuilder(args);

// ===== Register MVC =====
builder.Services.AddControllersWithViews();

// ===== Cookie Authentication =====
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Auth/Login";
        o.LogoutPath = "/Auth/Logout";
        o.AccessDeniedPath = "/Auth/Denied";
        o.SlidingExpiration = true;
        o.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();

// ===== Demo Data Stores =====
builder.Services.AddSingleton<IUserStore, InMemoryUserStore>();

// ===== Part 2 Services =====
builder.Services.AddSingleton<IClaimStore, InMemoryClaimStore>();
builder.Services.AddSingleton<IFileStorage, LocalFileStorage>();

// ===== Build App =====
var app = builder.Build();

// ===== Error Handling =====
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// ===== Middleware =====
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

/// ----------------------------------------------
/// Auto-skip the Welcome Page for authenticated users.
/// If a logged-in user hits "/" or "/Home/Index",
/// redirect them directly to the Dashboard.
/// ----------------------------------------------
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLowerInvariant();
    if (context.User.Identity?.IsAuthenticated == true &&
        (path == "/" || path == "/home" || path == "/home/index"))
    {
        context.Response.Redirect("/Dashboard/Index");
        return;
    }
    await next();
});


// ===== Default Route (Welcome Page first) =====
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


// ===== Demo Auth Types (Keep Below for Simplicity) =====
public interface IUserStore
{
    DemoUser? Validate(string username, string password);
}

public class DemoUser
{
    public string UserId { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Role { get; set; } = default!;
}

public class InMemoryUserStore : IUserStore
{
    private static readonly List<DemoUser> Users = new()
    {
        new() { UserId = "U1", Username = "lecturer", FullName = "Caleb Lecturer", Role = "Lecturer" },
        new() { UserId = "U2", Username = "pc1",        FullName = "Erin Coordinator", Role = "ProgrammeCoordinator" },
        new() { UserId = "U3", Username = "am1",        FullName = "Moe Manager", Role = "AcademicManager" },
        new() { UserId = "admin", Username = "admin",   FullName = "System Admin", Role = "Admin" },
    };

    // Demo password logic: password == username
    public DemoUser? Validate(string username, string password)
    {
        var u = Users.FirstOrDefault(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        return (u != null && password == u.Username) ? u : null;
    }
}
