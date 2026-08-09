using CommitToNotes.Server.Data;
using CommitToNotes.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// ---------- Database ----------
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// ---------- Identity + cookie auth ----------
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies();

builder.Services.AddAuthorization();

builder.Services.AddIdentityCore<ApplicationUser>(o =>
{
    o.User.RequireUniqueEmail = true;
    o.Password.RequiredLength = 8;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(o =>
{
    o.Cookie.HttpOnly = true;
    o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    o.Cookie.SameSite = SameSiteMode.Strict;
    o.Cookie.Name = "CommitToNotes.Auth";
    o.ExpireTimeSpan = TimeSpan.FromHours(8);
    o.SlidingExpiration = true;
    o.Events.OnRedirectToLogin = ctx =>
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    o.Events.OnRedirectToAccessDenied = ctx =>
    {
        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});

// ---------- CORS ----------
const string ClientCors = "ClientCors";

builder.Services.AddCors(o => o.AddPolicy(ClientCors, p => p
    .WithOrigins(
        "https://localhost:7204",
        "http://localhost:5042")
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()));

var app = builder.Build();

// ---------- Migrate on startup ----------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.UseHttpsRedirection();
app.UseCors(ClientCors);
app.UseAuthentication();
app.UseAuthorization();

// ---------- Auth endpoints ----------
var auth = app.MapGroup("/auth");

auth.MapPost("/register", async (
    RegisterRequest req,
    UserManager<ApplicationUser> users) =>
{
    var user = new ApplicationUser { UserName = req.Email, Email = req.Email };
    var result = await users.CreateAsync(user, req.Password);
    return result.Succeeded
        ? Results.Ok()
        : Results.ValidationProblem(result.Errors.ToDictionary(
            e => e.Code, e => new[] { e.Description }));
});

auth.MapPost("/login", async (
    LoginRequest req,
    SignInManager<ApplicationUser> signIn) =>
{
    var result = await signIn.PasswordSignInAsync(
        req.Email, req.Password, isPersistent: true, lockoutOnFailure: true);
    return result.Succeeded ? Results.Ok() : Results.Unauthorized();
});

auth.MapPost("/logout", async (SignInManager<ApplicationUser> signIn) =>
{
    await signIn.SignOutAsync();
    return Results.Ok();
}).RequireAuthorization();

auth.MapGet("/me", (ClaimsPrincipal user) =>
    Results.Ok(new UserInfo(user.Identity!.Name ?? "")))
    .RequireAuthorization();

// ---------- Dictionary endpoints ----------
var entries = app.MapGroup("/entries").RequireAuthorization();

entries.MapGet("/", async (ClaimsPrincipal user, AppDbContext db) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
    return await db.DictionaryEntries
        .Where(e => e.UserId == userId)
        .Select(e => new DictionaryEntryDto(e.Id, e.Key, e.Value))
        .ToListAsync();
});

entries.MapPost("/", async (
    CreateEntryRequest req, ClaimsPrincipal user, AppDbContext db) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
    var entry = new DictionaryEntry
    {
        UserId = userId,
        Key = req.Key,
        Value = req.Value
    };
    db.DictionaryEntries.Add(entry);
    await db.SaveChangesAsync();
    return Results.Created($"/entries/{entry.Id}",
        new DictionaryEntryDto(entry.Id, entry.Key, entry.Value));
});

entries.MapPut("/{id:int}", async (
    int id, UpdateEntryRequest req, ClaimsPrincipal user, AppDbContext db) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
    var entry = await db.DictionaryEntries
        .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
    if (entry is null) return Results.NotFound();
    entry.Key = req.Key;
    entry.Value = req.Value;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

entries.MapDelete("/{id:int}", async (
    int id, ClaimsPrincipal user, AppDbContext db) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
    var entry = await db.DictionaryEntries
        .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);
    if (entry is null) return Results.NotFound();
    db.DictionaryEntries.Remove(entry);
    await db.SaveChangesAsync();
    return Results.NoContent();
});


app.Run();