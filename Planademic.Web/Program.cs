using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Planademic.BLL.Services;
using Planademic.DAL;
using Planademic.DAL.Repositories;
using Planademic.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Register EF Core with the MSSQL connection string from appsettings.json
builder.Services.AddDbContext<PlanademicDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the repository and service so they can be injected into page models
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// Set up cookie-based authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PlanademicDbContext>();
    const string teacherEmail = "justinleomaas@gmail.com";
    if (!db.Users.Any(u => u.Email == teacherEmail))
    {
        db.Users.Add(new User
        {
            Email = teacherEmail,
            PasswordHash = "password",
            Role = "Teacher",
            FirstName = "Justin",
            LastName = "Maas",
            CreatedAt = DateTime.UtcNow,
        });
        db.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Authentication must be registered BEFORE Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();
