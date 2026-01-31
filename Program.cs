using AMZN.Data;
using AMZN.Repositories;
using AMZN.Repositories.Interfaces;
using AMZN.Security;
using AMZN.Security.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);




/*  TODO:
    
    - сделать DB connection string и вынести в appsettings-Secrets.json
    - в RegisterReqyestDTO сделать валидацию на имена - только буквы.
 
    
 */



// Config (DB connection string, secrets)
builder.Configuration.AddJsonFile("appsettings-Secrets.json", optional: true, reloadOnChange: true);



// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

//builder.Services.AddSingleton<IKDFService, PBKDFService>();


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



builder.Services.AddDbContext<DataContext>(options => options
    .UseSqlServer(builder
        .Configuration.GetConnectionString("LocalMs")));



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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();
app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllers();



app.Run();