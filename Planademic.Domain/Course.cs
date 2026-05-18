namespace Planademic.Domain;

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string JoinCode { get; set; } = string.Empty;
    public int TeacherId { get; set; }
    public DateTime CreatedAt { get; set; }
}
