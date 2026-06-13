using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Planademic.Domain;

namespace Planademic.DAL.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly string _connectionString;

    public CourseRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task AddAsync(Course course)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        // Description can be null, use DBNull.Value when it is, otherwise SQL will error
        await using var cmd = new SqlCommand(
            "INSERT INTO Courses (Name, Description, JoinCode, TeacherId) VALUES (@Name, @Description, @JoinCode, @TeacherId)",
            conn);
        cmd.Parameters.AddWithValue("@Name", course.Name);
        cmd.Parameters.AddWithValue("@Description", (object?)course.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@JoinCode", course.JoinCode);
        cmd.Parameters.AddWithValue("@TeacherId", course.TeacherId);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<Course?> GetByJoinCodeAsync(string joinCode)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(
            "SELECT Id, Name, Description, JoinCode, TeacherId, CreatedAt FROM Courses WHERE JoinCode = @JoinCode",
            conn);
        cmd.Parameters.AddWithValue("@JoinCode", joinCode);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return MapCourse(reader);
        return null;
    }

    public async Task<List<Course>> GetByUserIdAsync(int userId)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        // UNION combines two SELECTs into one list without duplicates
        // First half: courses where the user is the teacher
        // Second half: courses where the user is enrolled as a student
        await using var cmd = new SqlCommand(@"
            SELECT Id, Name, Description, JoinCode, TeacherId, CreatedAt
            FROM Courses WHERE TeacherId = @UserId
            UNION
            SELECT c.Id, c.Name, c.Description, c.JoinCode, c.TeacherId, c.CreatedAt
            FROM Courses c
            INNER JOIN CourseEnrollments e ON e.CourseId = c.Id
            WHERE e.StudentId = @UserId",
            conn);
        cmd.Parameters.AddWithValue("@UserId", userId);

        await using var reader = await cmd.ExecuteReaderAsync();
        var courses = new List<Course>();
        while (await reader.ReadAsync()) // loop through every row returned
            courses.Add(MapCourse(reader));
        return courses;
    }

    public async Task AddEnrollmentAsync(CourseEnrollment enrollment)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(
            "INSERT INTO CourseEnrollments (StudentId, CourseId) VALUES (@StudentId, @CourseId)",
            conn);
        cmd.Parameters.AddWithValue("@StudentId", enrollment.StudentId);
        cmd.Parameters.AddWithValue("@CourseId", enrollment.CourseId);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<bool> IsEnrolledAsync(int courseId, int studentId)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(
            "SELECT COUNT(1) FROM CourseEnrollments WHERE CourseId = @CourseId AND StudentId = @StudentId",
            conn);
        cmd.Parameters.AddWithValue("@CourseId", courseId);
        cmd.Parameters.AddWithValue("@StudentId", studentId);
        var count = (int)(await cmd.ExecuteScalarAsync())!;
        return count > 0;
    }

    public async Task<List<User>> GetStudentsByCourseIdAsync(int courseId, int teacherId)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        // First verify this course actually belongs to the teacher before returning anything
        await using var checkCmd = new SqlCommand(
            "SELECT COUNT(1) FROM Courses WHERE Id = @CourseId AND TeacherId = @TeacherId",
            conn);
        checkCmd.Parameters.AddWithValue("@CourseId", courseId);
        checkCmd.Parameters.AddWithValue("@TeacherId", teacherId);
        if ((int)(await checkCmd.ExecuteScalarAsync())! == 0)
            return [];

        // INNER JOIN links enrollments to their user records in one query
        await using var cmd = new SqlCommand(@"
            SELECT u.Id, u.Email, u.PasswordHash, u.Role, u.FirstName, u.LastName, u.CreatedAt
            FROM Users u
            INNER JOIN CourseEnrollments e ON e.StudentId = u.Id
            WHERE e.CourseId = @CourseId
            ORDER BY u.LastName, u.FirstName",
            conn);
        cmd.Parameters.AddWithValue("@CourseId", courseId);

        await using var reader = await cmd.ExecuteReaderAsync();
        var students = new List<User>();
        while (await reader.ReadAsync())
            students.Add(MapUser(reader));
        return students;
    }

    public async Task<bool> DeleteAsync(int courseId, int teacherId)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        // Delete student tasks linked to this course's assignments first
        // The DB won't do this automatically because Tasks has ON DELETE SET NULL, not CASCADE
        await using var deleteTasksCmd = new SqlCommand(@"
            DELETE FROM Tasks WHERE AssignmentId IN (
                SELECT Id FROM Assignments WHERE CourseId = @CourseId
            )",
            conn);
        deleteTasksCmd.Parameters.AddWithValue("@CourseId", courseId);
        await deleteTasksCmd.ExecuteNonQueryAsync();

        // Now delete the course itself, but only if the teacher owns it
        await using var deleteCourseCmd = new SqlCommand(
            "DELETE FROM Courses WHERE Id = @CourseId AND TeacherId = @TeacherId",
            conn);
        deleteCourseCmd.Parameters.AddWithValue("@CourseId", courseId);
        deleteCourseCmd.Parameters.AddWithValue("@TeacherId", teacherId);
        var rowsDeleted = await deleteCourseCmd.ExecuteNonQueryAsync();
        return rowsDeleted > 0;
    }

    // Helper methods to map data reader rows to domain models
    private static Course MapCourse(SqlDataReader reader) => new Course
    {
        Id          = (int)reader["Id"],
        Name        = (string)reader["Name"],
        Description = reader["Description"] == DBNull.Value ? null : (string)reader["Description"],
        JoinCode    = (string)reader["JoinCode"],
        TeacherId   = (int)reader["TeacherId"],
        CreatedAt   = (DateTime)reader["CreatedAt"]
    };

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
