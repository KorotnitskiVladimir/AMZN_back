using AMZN.Data;
using AMZN.Security;
using AMZN.Security.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

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



builder.Services.AddCors(options => 
    options.AddDefaultPolicy(policy => { policy.AllowAnyOrigin().AllowAnyHeader();
    }));


// DI services

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();




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
app.UseAuthorization();
app.UseSession();
app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();