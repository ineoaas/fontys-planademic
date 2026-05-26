namespace Planademic.Domain;

public class CourseTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ComplexityScore { get; set; }
    public DateTime Deadline { get; set; }
    public int CourseId { get; set; }
    public int CreatedByTeacherId { get; set; }
    public DateTime CreatedAt { get; set; }
}
