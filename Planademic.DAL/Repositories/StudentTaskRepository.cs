using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Planademic.Domain;

namespace Planademic.DAL.Repositories;

public class StudentTaskRepository : IStudentTaskRepository
{
    private readonly string _connectionString;

    public StudentTaskRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task AddAsync(StudentTask task)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        // AssignmentId and PriorityScore are nullable. Must use DBNull.Value when they are null
        await using var cmd = new SqlCommand(@"
            INSERT INTO Tasks (StudentId, AssignmentId, Title, Deadline, Complexity, IsPersonal, IsCompleted, PriorityScore)
            VALUES (@StudentId, @AssignmentId, @Title, @Deadline, @Complexity, @IsPersonal, @IsCompleted, @PriorityScore)",
            conn);
        cmd.Parameters.AddWithValue("@StudentId", task.StudentId);
        cmd.Parameters.AddWithValue("@AssignmentId", (object?)task.AssignmentId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Title", task.Title);
        cmd.Parameters.AddWithValue("@Deadline", task.Deadline);
        cmd.Parameters.AddWithValue("@Complexity", task.Complexity);
        cmd.Parameters.AddWithValue("@IsPersonal", task.IsPersonal);
        cmd.Parameters.AddWithValue("@IsCompleted", task.IsCompleted);
        cmd.Parameters.AddWithValue("@PriorityScore", (object?)task.PriorityScore ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<StudentTask>> GetByStudentIdAsync(int studentId)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(
            "SELECT Id, StudentId, AssignmentId, Title, Deadline, Complexity, IsPersonal, IsCompleted, PriorityScore, CreatedAt FROM Tasks WHERE StudentId = @StudentId",
            conn);
        cmd.Parameters.AddWithValue("@StudentId", studentId);

        await using var reader = await cmd.ExecuteReaderAsync();
        var tasks = new List<StudentTask>();
        while (await reader.ReadAsync())
            tasks.Add(MapStudentTask(reader));
        return tasks;
    }

    public async Task<bool> MarkCompleteAsync(int taskId, int studentId)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        // Only mark the task complete if it belongs to this student. If 0 rows updated, either task doesn't exist or doesn't belong to this student
        await using var cmd = new SqlCommand(
            "UPDATE Tasks SET IsCompleted = 1 WHERE Id = @TaskId AND StudentId = @StudentId",
            conn);
        cmd.Parameters.AddWithValue("@TaskId", taskId);
        cmd.Parameters.AddWithValue("@StudentId", studentId);
        var rowsUpdated = await cmd.ExecuteNonQueryAsync();
        return rowsUpdated > 0; // false = task not found or doesn't belong to this student
    }

    public async Task<bool> DeleteAsync(int taskId, int studentId)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(
            "DELETE FROM Tasks WHERE Id = @TaskId AND StudentId = @StudentId",
            conn);
        cmd.Parameters.AddWithValue("@TaskId", taskId);
        cmd.Parameters.AddWithValue("@StudentId", studentId);
        var rowsDeleted = await cmd.ExecuteNonQueryAsync();
        return rowsDeleted > 0;
    }

    private static StudentTask MapStudentTask(SqlDataReader reader) => new StudentTask
    {
        Id            = (int)reader["Id"],
        StudentId     = (int)reader["StudentId"],
        // AssignmentId is nullable in the DB. Check for DBNull before casting to int?
        AssignmentId  = reader["AssignmentId"] == DBNull.Value ? null : (int?)reader["AssignmentId"],
        Title         = (string)reader["Title"],
        Deadline      = (DateTime)reader["Deadline"],
        Complexity    = (int)reader["Complexity"],
        IsPersonal    = (bool)reader["IsPersonal"],
        IsCompleted   = (bool)reader["IsCompleted"],
        PriorityScore = reader["PriorityScore"] == DBNull.Value ? null : (decimal?)reader["PriorityScore"],
        CreatedAt     = (DateTime)reader["CreatedAt"]
    };
}
