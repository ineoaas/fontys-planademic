using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Planademic.Domain;

namespace Planademic.DAL.Repositories;

public class AvailabilityRepository : IAvailabilityRepository
{
    private readonly string _connectionString;

    public AvailabilityRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    public async Task<List<AvailabilitySlot>> GetByStudentIdAsync(int studentId)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(
            "SELECT Id, StudentId, DayOfWeek, StartTime, EndTime, IsRecurring FROM AvailabilitySlots WHERE StudentId = @StudentId",
            conn);
        cmd.Parameters.AddWithValue("@StudentId", studentId);

        await using var reader = await cmd.ExecuteReaderAsync();
        var slots = new List<AvailabilitySlot>();
        while (await reader.ReadAsync())
            slots.Add(MapSlot(reader));
        return slots;
    }

    public async Task SaveRangeAsync(int studentId, List<AvailabilitySlot> slots)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        // Delete all existing slots for this student first
        await using var deleteCmd = new SqlCommand(
            "DELETE FROM AvailabilitySlots WHERE StudentId = @StudentId",
            conn);
        deleteCmd.Parameters.AddWithValue("@StudentId", studentId);
        await deleteCmd.ExecuteNonQueryAsync();

        // Then insert each new slot one by one
        foreach (var slot in slots)
        {
            await using var insertCmd = new SqlCommand(@"
                INSERT INTO AvailabilitySlots (StudentId, DayOfWeek, StartTime, EndTime, IsRecurring)
                VALUES (@StudentId, @DayOfWeek, @StartTime, @EndTime, @IsRecurring)",
                conn);
            insertCmd.Parameters.AddWithValue("@StudentId", slot.StudentId);
            // Store the DayOfWeek enum as an int in the database.
            insertCmd.Parameters.AddWithValue("@DayOfWeek", (int)slot.DayOfWeek);
            // Convert TimeOnly to TimeSpan for SQL Server TIME type.
            insertCmd.Parameters.AddWithValue("@StartTime", slot.StartTime.ToTimeSpan());
            insertCmd.Parameters.AddWithValue("@EndTime", slot.EndTime.ToTimeSpan());
            insertCmd.Parameters.AddWithValue("@IsRecurring", slot.IsRecurring);
            await insertCmd.ExecuteNonQueryAsync();
        }
    }

    public async Task DeleteAsync(int slotId)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(
            "DELETE FROM AvailabilitySlots WHERE Id = @SlotId",
            conn);
        cmd.Parameters.AddWithValue("@SlotId", slotId);
        await cmd.ExecuteNonQueryAsync();
    }

    private static AvailabilitySlot MapSlot(SqlDataReader reader) => new AvailabilitySlot
    {
        Id          = (int)reader["Id"],
        StudentId   = (int)reader["StudentId"],
        // The DayOfWeek enum is stored as an int in the database, so we cast it back to the enum type.
        DayOfWeek   = (DayOfWeek)(int)reader["DayOfWeek"],
        // SQL Server TIME comes back as TimeSpan, so we convert it to TimeOnly.
        StartTime   = TimeOnly.FromTimeSpan((TimeSpan)reader["StartTime"]),
        EndTime     = TimeOnly.FromTimeSpan((TimeSpan)reader["EndTime"]),
        IsRecurring = (bool)reader["IsRecurring"]
    };
}
