namespace Planademic.Domain;

public class Assignment
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
    public int Complexity { get; set; }
    public DateTime CreatedAt { get; set; }
}
