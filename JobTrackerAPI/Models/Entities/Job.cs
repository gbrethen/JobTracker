namespace JobTrackerAPI.Models.Entities;

public class Job
{
    public int JobId { get; set; }
    public string Company { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public int JobTypeId { get; set; }
    public int JobLocationId { get; set; }
    public string? PhysicalLocation { get; set; }
    public DateOnly ApplicationDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public string Status { get; set; } = "Applied";
    public string? SalaryRange { get; set; }
    public string? JobUrl { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactEmail { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public JobType? JobType { get; set; }
    public JobLocation? JobLocation { get; set; }
    public ICollection<JobNote> JobNotes { get; set; } = new List<JobNote>();
}
