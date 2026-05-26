using Microsoft.EntityFrameworkCore;
using Planademic.Domain;

namespace Planademic.DAL;

public class PlanademicDbContext : DbContext
{
    public PlanademicDbContext(DbContextOptions<PlanademicDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseEnrollment> Enrollments => Set<CourseEnrollment>();

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
            entity.Property(c => c.Name).HasMaxLength(200).IsRequired();
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
    }
}
