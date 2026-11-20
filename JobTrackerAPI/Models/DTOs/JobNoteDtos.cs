using System.ComponentModel.DataAnnotations;

namespace JobTrackerAPI.Models.DTOs;

public class JobNoteResponseDto
{
    public int JobNoteId { get; set; }
    public int JobId { get; set; }
    public string NoteText { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class JobNoteCreateDto
{
    [Required]
    public string NoteText { get; set; } = string.Empty;
}

public class JobNoteUpdateDto
{
    [Required]
    public string NoteText { get; set; } = string.Empty;
}
