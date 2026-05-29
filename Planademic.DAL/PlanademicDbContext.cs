using Microsoft.EntityFrameworkCore;
using Planademic.Domain;

namespace Planademic.DAL;

public class PlanademicDbContext : DbContext
{
    public PlanademicDbContext(DbContextOptions<PlanademicDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseEnrollment> Enrollments => Set<CourseEnrollment>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<StudentTask> Tasks => Set<StudentTask>();
    public DbSet<AvailabilitySlot> AvailabilitySlots => Set<AvailabilitySlot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email).HasMaxLength(150).IsRequired();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.PasswordHash).HasColumnName("PasswordHash").HasMaxLength(256).IsRequired();
            entity.Property(u => u.Role).HasMaxLength(20).IsRequired();
            entity.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.LastName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.ToTable("Courses");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).HasMaxLength(150).IsRequired();
            entity.Property(c => c.Description).HasMaxLength(500);
            entity.Property(c => c.JoinCode).HasMaxLength(10).IsRequired();
            entity.HasIndex(c => c.JoinCode).IsUnique();
            entity.Property(c => c.CreatedAt).HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<CourseEnrollment>(entity =>
        {
            entity.ToTable("CourseEnrollments");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.CourseId, e.StudentId }).IsUnique();
            entity.Property(e => e.EnrolledAt).HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.ToTable("Assignments");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Title).HasMaxLength(200).IsRequired();
            entity.Property(a => a.Description).HasMaxLength(1000);
            entity.Property(a => a.Complexity).IsRequired();
            entity.Property(a => a.Deadline).IsRequired();
            entity.Property(a => a.CourseId).IsRequired();
            entity.Property(a => a.CreatedAt).HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<StudentTask>(entity =>
        {
            entity.ToTable("Tasks");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Title).HasMaxLength(200).IsRequired();
            entity.Property(t => t.Deadline).IsRequired();
            entity.Property(t => t.Complexity).IsRequired();
            entity.Property(t => t.StudentId).IsRequired();
            entity.Property(t => t.IsPersonal).IsRequired();
            entity.Property(t => t.IsCompleted).IsRequired();
            entity.Property(t => t.PriorityScore).HasColumnType("decimal(10,2)");
            entity.Property(t => t.CreatedAt).HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<AvailabilitySlot>(entity =>
        {
            entity.ToTable("AvailabilitySlots");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.StudentId).IsRequired();
            entity.Property(a => a.DayOfWeek).IsRequired();
            entity.Property(a => a.StartTime).IsRequired();
            entity.Property(a => a.EndTime).IsRequired();
            entity.Property(a => a.IsRecurring).IsRequired();
        });
    }
}
