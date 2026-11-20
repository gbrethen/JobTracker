namespace JobTrackerAPI.Models.Entities;

public class JobLocation
{
    public int JobLocationId { get; set; }
    public string LocationType { get; set; } = string.Empty;

    public ICollection<Job> Jobs { get; set; } = new List<Job>();
}
