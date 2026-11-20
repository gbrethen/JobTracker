namespace JobTrackerAPI.Models.Entities;

public class JobType
{
    public int JobTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;

    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}
