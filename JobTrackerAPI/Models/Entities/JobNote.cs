namespace JobTrackerAPI.Models.Entities;

public class JobNote
{
    public int JobNoteId { get; set; }
    public int JobId { get; set; }
    public string NoteText { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Job? Job { get; set; }
}
