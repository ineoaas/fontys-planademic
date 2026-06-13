using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Planademic.Domain;

namespace Planademic.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    // IConfiguration is built into ASP.NET Core and gives access to appsettings.json
    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(
            "SELECT Id, Email, PasswordHash, Role, FirstName, LastName, CreatedAt FROM Users WHERE Email = @Email",
            conn);
        cmd.Parameters.AddWithValue("@Email", email);

        // ExecuteReaderAsync runs the SELECT and gives back rows
        await using var reader = await cmd.ExecuteReaderAsync();

        // ReadAsync moves to the next row; return false if there are no rows
        if (await reader.ReadAsync())
            return MapUser(reader);

        return null;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(
            "SELECT COUNT(1) FROM Users WHERE Email = @Email",
            conn);
        cmd.Parameters.AddWithValue("@Email", email);

        // ExecuteScalarAsync runs the SQL and gives back the first column of the first row (the count)
        var count = (int)(await cmd.ExecuteScalarAsync())!;
        return count > 0;
    }

    public async Task AddAsync(User user)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        // The SQL command uses parameters to prevent SQL injection.
        await using var cmd = new SqlCommand(
            "INSERT INTO Users (Email, PasswordHash, Role, FirstName, LastName) VALUES (@Email, @PasswordHash, @Role, @FirstName, @LastName)",
            conn);
        cmd.Parameters.AddWithValue("@Email", user.Email);
        cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
        cmd.Parameters.AddWithValue("@Role", user.Role);
        cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
        cmd.Parameters.AddWithValue("@LastName", user.LastName);

        // ExecuteNonQueryAsync runs the SQL without expecting rows back
        await cmd.ExecuteNonQueryAsync();
    }

    // Reads each column from the current row and puts the values into a User object
    private static User MapUser(SqlDataReader reader) => new User
    {
        Id           = (int)reader["Id"],
        Email        = (string)reader["Email"],
        PasswordHash = (string)reader["PasswordHash"],
        Role         = (string)reader["Role"],
        FirstName    = (string)reader["FirstName"],
        LastName     = (string)reader["LastName"],
        CreatedAt    = (DateTime)reader["CreatedAt"]
    };
}
