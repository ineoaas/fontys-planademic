using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Data.SqlClient;
using Planademic.BLL.Services;
using Planademic.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// Register the repository and service so they can be injected into page models
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ICourseTaskRepository, CourseTaskRepository>();
builder.Services.AddScoped<ICourseTaskService, CourseTaskService>();
builder.Services.AddScoped<IStudentTaskRepository, StudentTaskRepository>();
builder.Services.AddScoped<IStudentTaskService, StudentTaskService>();
builder.Services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();
builder.Services.AddScoped<IAvailabilityService, AvailabilityService>();
builder.Services.AddScoped<ISchedulingService, SchedulingService>();

// Set up cookie-based authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Dashboard";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

var app = builder.Build();

// Seed the default teacher account if it doesn't exist yet
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
await using var seedConn = new SqlConnection(connectionString);
await seedConn.OpenAsync();

const string teacherEmail = "justinleomaas@gmail.com";
await using var checkCmd = new SqlCommand("SELECT COUNT(1) FROM Users WHERE Email = @Email", seedConn);
checkCmd.Parameters.AddWithValue("@Email", teacherEmail);
var teacherExists = (int)(await checkCmd.ExecuteScalarAsync())! > 0;

if (!teacherExists)
{
    await using var insertCmd = new SqlCommand(@"
        INSERT INTO Users (Email, PasswordHash, Role, FirstName, LastName)
        VALUES (@Email, @Password, @Role, @FirstName, @LastName)",
        seedConn);
    insertCmd.Parameters.AddWithValue("@Email", teacherEmail);
    insertCmd.Parameters.AddWithValue("@Password", "password");
    insertCmd.Parameters.AddWithValue("@Role", "Teacher");
    insertCmd.Parameters.AddWithValue("@FirstName", "Justin");
    insertCmd.Parameters.AddWithValue("@LastName", "Maas");
    await insertCmd.ExecuteNonQueryAsync();
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
