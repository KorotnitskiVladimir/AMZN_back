using AMZN.Data;
using AMZN.Extensions;
using AMZN.Middleware;
using AMZN.Middleware;
using AMZN.Models;
using AMZN.Models;
using AMZN.Repositories.Actions;
using AMZN.Repositories.Categories;
using AMZN.Repositories.Products;
using AMZN.Repositories.Users;
using AMZN.Security.Passwords;
using AMZN.Security.Tokens;
using AMZN.Services.Admin;
using AMZN.Services.Auth;
using AMZN.Services.Home;
using AMZN.Services.Product;
using AMZN.Services.Storage.Cloud;
using AMZN.Services.Storage.Local;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Swashbuckle.AspNetCore.Filters;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

/*  Note:
    
-  Старые refresh токены (revoked/expired) копятся в БД, можно добавить периодическую очистку (удалять ExpiresAt < UtcNow или IsRevoked=true).
-  Rate limiting на Auth использует RemoteIpAddress. При деплое за прокси (Azure/AWS/K8s) реальный IP может приходить в X-Forwarded-For. Тогда нужен ForwardedHeaders (см. Extensions/ForwardedHeadersExtensions).

 
 */


/*  TODO:
       - upload limits ограничение общего размера multipart/form-data (защита от слишком больших аплоадов)
       - CloudStirageServuce: sync методы сделать Async (блокирует поток)
    
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
// MSSQL
//builder.Services.AddDbContext<DataContext>(options => options
//    .UseSqlServer(builder
//        .Configuration.GetConnectionString("DefaultConnection")));

// MySQL
builder.Services.AddDbContext<DataContext>(options => options
    .UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
              ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

//builder.Services.AddDbContext<DataContext>(options => options
//    .UseMySql(
//        builder.Configuration.GetConnectionString("DefaultConnection"),
//        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")),
//        mySqlOptions => mySqlOptions.SchemaBehavior(MySqlSchemaBehavior.Ignore)  // ← Игнорировать schemas
//    ));


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

// Output cache (home page)
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("HomePage", policy =>
    {
        policy.Expire(TimeSpan.FromSeconds(10));
        policy.SetVaryByQuery("take");
    });
});


// DI services
builder.Services.AddScoped<FormsValidators>();
builder.Services.AddScoped<DataAccessor>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserRefreshTokenRepository, UserRefreshTokenRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<HomeService>();
builder.Services.AddScoped<AdminUserService>();
builder.Services.AddScoped<AdminCategoryService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<AdminProductService>();
builder.Services.AddScoped<IActionRepository, ActionRepository>();
builder.Services.AddScoped<AdminActionService>();

builder.Services.AddSingleton<ILocalsStorageService, LocalStorageService>();
builder.Services.AddSingleton<ICloudStorageService, CloudStorageService>();

builder.Services.AddAmznForwardedHeaders();
builder.Services.AddAmznRateLimiting();



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


builder.Services.AddAuthorization();


var app = builder.Build();
app.UseAmznForwardedHeaders();
app.UseMiddleware<ApiExceptionMiddleware>();

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

// Action validity check
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var checker = services.GetRequiredService<AdminActionService>();
    checker.CheckActionsValidity();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api"), appBuilder =>
{
    appBuilder.UseSession();
    appBuilder.UseAuthToken();
    appBuilder.UseAuthSession();
});

app.UseCors();

app.UseAmznRateLimiting();
app.UseAuthentication();
app.UseAuthorization();

app.UseOutputCache();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllers();


app.Run();