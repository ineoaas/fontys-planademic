namespace Planademic.Domain;

public class StudentTask
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int? AssignmentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
    public int Complexity { get; set; }

    // stores in the same table as StudentTask, but only for personal tasks (not linked to an assignment)
    public bool IsPersonal { get; set; }
    public bool IsCompleted { get; set; }
    public decimal? PriorityScore { get; set; }
    public DateTime CreatedAt { get; set; }
}
