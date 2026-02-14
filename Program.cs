using AMZN.Data;
using AMZN.Repositories.Users;
using AMZN.Security.Passwords;
using AMZN.Security.Tokens;
using AMZN.Services.Auth;
using AMZN.Shared.Api;
using AMZN.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Security.Claims;
using AMZN.Middleware;
using AMZN.Models;

var builder = WebApplication.CreateBuilder(args);

/*  Note:
    
-  Старые refresh токены (revoked/expired) копятся в БД, можно добавить периодическую очистку (удалять ExpiresAt < UtcNow или IsRevoked=true).

 
 */


/*  TODO:
    
    - сделать DB connection string и вынести в appsettings-Secrets.json
    - сделать глобальный обработчик ексепшенов и убрать try/catch из контроллеров.
    - rate limiting на login/register/refresh ?
 
    
 */



// Config (DB connection string, secrets)
builder.Configuration.AddJsonFile("appsettings-Secrets.json", optional: true, reloadOnChange: true);



// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddApiValidationErrors();      // единый формат 400 ответа для ошибок валидации DTO (ModelState)

//builder.Services.AddSingleton<IKDFService, PBKDFService>();


//  --- Swagger ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AMZN API",
        Version = "Swag v1"
    });

    c.ExampleFilters();

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT access token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// examples
builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

// -----------------------------------------------


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


//  DbContext 

//builder.Services.AddDbContext<DataContext>(options => options
//    .UseSqlServer(builder
//        .Configuration.GetConnectionString("LocalMs")));

//  DbContext 
builder.Services.AddDbContext<DataContext>(options => options
    .UseSqlServer(builder
        .Configuration.GetConnectionString("DefaultConnection")));  // <- было LocalMs в appsettings


builder.Services.AddScoped<FormsValidators>();
builder.Services.AddScoped<DataAccessor>();


//builder.Services.AddCors(options => 
//    options.AddDefaultPolicy(policy => { policy.AllowAnyOrigin().AllowAnyHeader();
//    }));

//  Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173"     // react local ?
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
        // .AllowCredentials();             // если будут cookies ?
    });
});


// DI services
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserRefreshTokenRepository, UserRefreshTokenRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();


// JWT auth

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        const int MinJwtKeyBytes = 32; // HS256 -> 256 bit secret

        var keyBase64 = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing");

        byte[] keyBytes;
        try
        {
            keyBytes = Convert.FromBase64String(keyBase64);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException("Jwt:Key must be a valid Base64 string.", ex);
        }

        if (keyBytes.Length < MinJwtKeyBytes)
            throw new InvalidOperationException($"Jwt:Key is too short. Need at least {MinJwtKeyBytes} bytes for HS256.");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

            NameClaimType = ClaimTypes.NameIdentifier,
            RoleClaimType = ClaimTypes.Role
        };
    });


builder.Services.AddAuthorization();        //  .?


var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AMZN API v1");
        c.RoutePrefix = "swagger";
    });

}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseMiddleware<ApiExceptionMiddleware>();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();
app.UseAuthToken();
app.UseAuthSession();
app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllers();



app.Run();