using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Planademic.Domain;

namespace Planademic.DAL.Repositories;

public class CourseTaskRepository : ICourseTaskRepository
{
    private readonly string _connectionString;

    public CourseTaskRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task AddAsync(Assignment assignment)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(@"
            INSERT INTO Assignments (CourseId, Title, Description, Deadline, Complexity)
            VALUES (@CourseId, @Title, @Description, @Deadline, @Complexity)",
            conn);
        cmd.Parameters.AddWithValue("@CourseId", assignment.CourseId);
        cmd.Parameters.AddWithValue("@Title", assignment.Title);
        cmd.Parameters.AddWithValue("@Description", (object?)assignment.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Deadline", assignment.Deadline);
        cmd.Parameters.AddWithValue("@Complexity", assignment.Complexity);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<Assignment>> GetByCourseIdAsync(int courseId)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(
            "SELECT Id, CourseId, Title, Description, Deadline, Complexity, CreatedAt FROM Assignments WHERE CourseId = @CourseId",
            conn);
        cmd.Parameters.AddWithValue("@CourseId", courseId);

        await using var reader = await cmd.ExecuteReaderAsync();
        var assignments = new List<Assignment>();
        while (await reader.ReadAsync())
            assignments.Add(MapAssignment(reader));
        return assignments;
    }

    public async Task<List<Assignment>> GetByEnrolledStudentAsync(int studentId)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        // INNER JOIN replaces the two-step EF query to get assignments for a student's enrolled courses
        await using var cmd = new SqlCommand(@"
            SELECT a.Id, a.CourseId, a.Title, a.Description, a.Deadline, a.Complexity, a.CreatedAt
            FROM Assignments a
            INNER JOIN CourseEnrollments e ON e.CourseId = a.CourseId
            WHERE e.StudentId = @StudentId",
            conn);
        cmd.Parameters.AddWithValue("@StudentId", studentId);

        await using var reader = await cmd.ExecuteReaderAsync();
        var assignments = new List<Assignment>();
        while (await reader.ReadAsync())
            assignments.Add(MapAssignment(reader));
        return assignments;
    }

    public async Task<bool> DeleteAsync(int assignmentId)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        // Delete student tasks first
        await using var deleteTasksCmd = new SqlCommand(
            "DELETE FROM Tasks WHERE AssignmentId = @AssignmentId",
            conn);
        deleteTasksCmd.Parameters.AddWithValue("@AssignmentId", assignmentId);
        await deleteTasksCmd.ExecuteNonQueryAsync();

        await using var deleteCmd = new SqlCommand(
            "DELETE FROM Assignments WHERE Id = @AssignmentId",
            conn);
        deleteCmd.Parameters.AddWithValue("@AssignmentId", assignmentId);
        var rowsDeleted = await deleteCmd.ExecuteNonQueryAsync();
        return rowsDeleted > 0; // false means the assignment didn't exist
    }

    private static Assignment MapAssignment(SqlDataReader reader) => new Assignment
    {
        Id          = (int)reader["Id"],
        CourseId    = (int)reader["CourseId"],
        Title       = (string)reader["Title"],
        Description = reader["Description"] == DBNull.Value ? string.Empty : (string)reader["Description"],
        Deadline    = (DateTime)reader["Deadline"],
        Complexity  = (int)reader["Complexity"],
        CreatedAt   = (DateTime)reader["CreatedAt"]
    };
}
