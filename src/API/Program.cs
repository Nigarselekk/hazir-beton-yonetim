using System.Text;
using HazirBeton.API.Authorization;
using HazirBeton.Application.Common;
using HazirBeton.Application.Features.Auth;
using HazirBeton.Application.Features.ConcreteRequests;
using HazirBeton.Application.Features.Customers;
using HazirBeton.Application.Features.Personnel;
using HazirBeton.Application.Features.Sites;
using HazirBeton.Application.Features.Users;
using HazirBeton.Application.Features.VehiclePersonnel;
using HazirBeton.Application.Features.Vehicles;
using HazirBeton.Domain.Entities;
using HazirBeton.Domain.Enums;
using HazirBeton.Infrastructure.Persistence;
using HazirBeton.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// HTTP context (needed for CurrentUserService)
builder.Services.AddHttpContextAccessor();

// Services
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ISiteService, SiteService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IPersonnelService, PersonnelService>();
builder.Services.AddScoped<IVehiclePersonnelService, VehiclePersonnelService>();
builder.Services.AddScoped<IConcreteRequestService, ConcreteRequestService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is not configured.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Use the legacy JwtSecurityTokenHandler for validation to match token generation in AuthService.
        // .NET 8 JwtBearer defaults to JsonWebTokenHandler which has different key-matching behavior.
        options.UseSecurityTokenValidators = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

// Authorization — permission policies
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy($"Permission:{nameof(UserRole.HeadManager)}", p => p.RequireRole(nameof(UserRole.HeadManager)));

foreach (var permission in Enum.GetValues<Permission>())
{
    builder.Services.AddAuthorizationBuilder()
        .AddPolicy($"Permission:{permission}", p =>
            p.AddRequirements(new PermissionRequirement(permission)));
}

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hazır Beton API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT token giriniz. Örnek: Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            []
        }
    });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

// Seed initial HeadManager
await SeedHeadManagerAsync(app);

app.UseSwagger();
app.UseSwaggerUI(c => c.InjectJavascript("/swagger-jwt-helper.js"));

// Intercepts /api/auth/login responses in Swagger UI and auto-applies the access token.
// Prevents the "paste entire JSON" mistake that causes a malformed Authorization header.
app.MapGet("/swagger-jwt-helper.js", () => Results.Content("""
(function () {
    const orig = window.fetch;
    window.fetch = async function (input, init) {
        const resp = await orig(input, init);
        const url = (typeof input === 'string' ? input : (input?.url ?? ''));
        if (url.includes('/api/auth/login') && init?.method?.toUpperCase?.() === 'POST') {
            try {
                const data = await resp.clone().json();
                if (data.accessToken) {
                    const apply = () => {
                        if (window.ui) {
                            window.ui.preauthorizeApiKey('Bearer', data.accessToken);
                        } else {
                            setTimeout(apply, 200);
                        }
                    };
                    apply();
                }
            } catch (_) {}
        }
        return resp;
    };
})();
""", "application/javascript")).AllowAnonymous();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

static async Task SeedHeadManagerAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var hasUsers = await db.Users.AnyAsync();
    if (hasUsers) return;

    var config = app.Configuration;
    var password = config["Seed:HeadManagerPassword"];
    if (string.IsNullOrWhiteSpace(password))
        throw new InvalidOperationException(
            "Seed:HeadManagerPassword is not configured. " +
            "Set it in appsettings.Development.json or via the SEED__HEADMANAGERPASSWORD environment variable.");

    var username = config["Seed:HeadManagerUsername"] ?? "admin";
    var fullName = config["Seed:HeadManagerFullName"] ?? "Ana Yönetici";

    db.Users.Add(new User
    {
        Id = Guid.NewGuid(),
        Username = username,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
        FullName = fullName,
        Role = UserRole.HeadManager,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    });

    await db.SaveChangesAsync();
}
