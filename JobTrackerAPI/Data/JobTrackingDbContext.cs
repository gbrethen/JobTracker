using Microsoft.EntityFrameworkCore;
using JobTrackerAPI.Models.Entities;

namespace JobTrackerAPI.Data;

public class JobTrackingDbContext : DbContext
{
    public JobTrackingDbContext(DbContextOptions<JobTrackingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobType> JobTypes => Set<JobType>();
    public DbSet<JobLocation> JobLocations => Set<JobLocation>();
    public DbSet<JobNote> JobNotes => Set<JobNote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // JobType entity configuration
        modelBuilder.Entity<JobType>(entity =>
        {
            entity.ToTable("JobType");
            entity.HasKey(e => e.JobTypeId);

            entity.Property(e => e.TypeName)
                  .HasMaxLength(50)
                  .IsRequired();

            entity.HasIndex(e => e.TypeName)
                  .IsUnique();

            // Match seed data from sql script
            entity.HasData(
                new JobType { JobTypeId = 1, TypeName = "Full Time" },
                new JobType { JobTypeId = 2, TypeName = "Part Time" },
                new JobType { JobTypeId = 3, TypeName = "Contract" },
                new JobType { JobTypeId = 4, TypeName = "Temp" }
            );
        });

        // JobLocation entity configuration
        modelBuilder.Entity<JobLocation>(entity =>
        {
            entity.ToTable("JobLocation");
            entity.HasKey(e => e.JobLocationId);

            entity.Property(e => e.LocationType)
                  .HasMaxLength(50)
                  .IsRequired();

            entity.HasIndex(e => e.LocationType)
                  .IsUnique();

            // Match seed data from sql script
            entity.HasData(
                new JobLocation { JobLocationId = 1, LocationType = "On-Site" },
                new JobLocation { JobLocationId = 2, LocationType = "Remote" },
                new JobLocation { JobLocationId = 3, LocationType = "Hybrid" }
            );
        });

        // Job entity configuration
        modelBuilder.Entity<Job>(entity =>
        {
            entity.ToTable("Job");
            entity.HasKey(e => e.JobId);

            entity.Property(e => e.Company)
                  .HasMaxLength(150)
                  .IsRequired();

            entity.Property(e => e.JobTitle)
                  .HasMaxLength(150)
                  .IsRequired();

            entity.Property(e => e.PhysicalLocation)
                  .HasMaxLength(150);

            entity.Property(e => e.Status)
                  .HasMaxLength(50)
                  .HasDefaultValue("Applied")
                  .IsRequired();

            entity.Property(e => e.SalaryRange)
                  .HasMaxLength(100);

            entity.Property(e => e.JobUrl)
                  .HasMaxLength(2048);

            entity.Property(e => e.ContactPerson)
                  .HasMaxLength(100);

            entity.Property(e => e.ContactEmail)
                  .HasMaxLength(256);

            entity.Property(e => e.ApplicationDate)
                  .IsRequired();

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(e => e.UpdatedAt)
                  .HasDefaultValueSql("SYSUTCDATETIME()");

            // Relationships
            entity.HasOne(e => e.JobType)
                  .WithMany(t => t.Jobs)
                  .HasForeignKey(e => e.JobTypeId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.JobLocation)
                  .WithMany(l => l.Jobs)
                  .HasForeignKey(e => e.JobLocationId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.JobNotes)
                  .WithOne(n => n.Job)
                  .HasForeignKey(n => n.JobId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => e.JobTypeId);
            entity.HasIndex(e => e.JobLocationId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Company);
        });

        // JobNote entity configuration
        modelBuilder.Entity<JobNote>(entity =>
        {
            entity.ToTable("JobNotes");
            entity.HasKey(e => e.JobNoteId);

            entity.Property(e => e.NoteText)
                  .IsRequired();

            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.HasIndex(e => e.JobId);
        });
    }
}
