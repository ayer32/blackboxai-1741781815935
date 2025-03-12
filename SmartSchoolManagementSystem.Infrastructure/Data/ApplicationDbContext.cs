using Microsoft.EntityFrameworkCore;
using SmartSchoolManagementSystem.Core.Entities;
using SmartSchoolManagementSystem.Core.Entities.Donation;
using SmartSchoolManagementSystem.Core.Entities.Library;
using SmartSchoolManagementSystem.Core.Entities.School;

namespace SmartSchoolManagementSystem.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Donation> Donations { get; set; } = null!;
    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<BookLending> BookLendings { get; set; } = null!;
    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<Teacher> Teachers { get; set; } = null!;
    public DbSet<Class> Classes { get; set; } = null!;
    public DbSet<Subject> Subjects { get; set; } = null!;
    public DbSet<Attendance> Attendances { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure soft delete filter
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(e => !EF.Property<bool>(e, "IsDeleted"));
            }
        }

        // Configure relationships
        modelBuilder.Entity<Class>()
            .HasOne(c => c.ClassTeacher)
            .WithMany(t => t.Classes)
            .HasForeignKey(c => c.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Subject>()
            .HasOne(s => s.Teacher)
            .WithMany(t => t.Subjects)
            .HasForeignKey(s => s.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Student>()
            .HasOne(s => s.Class)
            .WithMany(c => c.Students)
            .HasForeignKey(s => s.ClassId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Attendance>()
            .HasOne(a => a.Student)
            .WithMany(s => s.Attendances)
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BookLending>()
            .HasOne(bl => bl.Book)
            .WithMany(b => b.BookLendings)
            .HasForeignKey(bl => bl.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure many-to-many relationships
        modelBuilder.Entity<Class>()
            .HasMany(c => c.Subjects)
            .WithMany(s => s.Classes)
            .UsingEntity(j => j.ToTable("ClassSubjects"));

        // Configure indexes
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.StudentId)
            .IsUnique();

        modelBuilder.Entity<Teacher>()
            .HasIndex(t => t.TeacherId)
            .IsUnique();

        modelBuilder.Entity<Book>()
            .HasIndex(b => b.ISBN)
            .IsUnique();

        modelBuilder.Entity<Subject>()
            .HasIndex(s => s.Code)
            .IsUnique();

        modelBuilder.Entity<Donation>()
            .HasIndex(d => d.ReceiptNumber)
            .IsUnique();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
