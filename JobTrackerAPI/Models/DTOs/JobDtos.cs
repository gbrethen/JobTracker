using System.ComponentModel.DataAnnotations;

namespace JobTrackerAPI.Models.DTOs;

public class JobResponseDto
{
    public int JobId { get; set; }
    public string Company { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public int JobTypeId { get; set; }
    public string JobTypeName { get; set; } = string.Empty;
    public int JobLocationId { get; set; }
    public string JobLocationType { get; set; } = string.Empty;
    public string? PhysicalLocation { get; set; }
    public DateOnly ApplicationDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? SalaryRange { get; set; }
    public string? JobUrl { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactEmail { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int NotesCount { get; set; }
}

public class JobDetailResponseDto : JobResponseDto
{
    public List<JobNoteResponseDto> Notes { get; set; } = new();
}

public class JobCreateDto
{
    [Required]
    [StringLength(150, ErrorMessage = "Company name cannot exceed 150 characters.")]
    public string Company { get; set; } = string.Empty;

    [Required]
    [StringLength(150, ErrorMessage = "Job Title cannot exceed 150 characters.")]
    public string JobTitle { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "A valid JobTypeId is required.")]
    public int JobTypeId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "A valid JobLocationId is required.")]
    public int JobLocationId { get; set; }

    [StringLength(150)]
    public string? PhysicalLocation { get; set; }

    public DateOnly? ApplicationDate { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } = "Applied";

    [StringLength(100)]
    public string? SalaryRange { get; set; }

    [Url]
    [StringLength(2048)]
    public string? JobUrl { get; set; }

    [StringLength(100)]
    public string? ContactPerson { get; set; }

    [EmailAddress]
    [StringLength(256)]
    public string? ContactEmail { get; set; }

    public string? InitialNote { get; set; }
}

public class JobUpdateDto
{
    [Required]
    [StringLength(150)]
    public string Company { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string JobTitle { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int JobTypeId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int JobLocationId { get; set; }

    [StringLength(150)]
    public string? PhysicalLocation { get; set; }

    [Required]
    public DateOnly ApplicationDate { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Applied";

    [StringLength(100)]
    public string? SalaryRange { get; set; }

    [Url]
    [StringLength(2048)]
    public string? JobUrl { get; set; }

    [StringLength(100)]
    public string? ContactPerson { get; set; }

    [EmailAddress]
    [StringLength(256)]
    public string? ContactEmail { get; set; }
}

public class JobStatusUpdateDto
{
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = string.Empty;
}

public class JobFilterParameters
{
    public string? Search { get; set; }
    public string? Status { get; set; }
    public int? JobTypeId { get; set; }
    public int? JobLocationId { get; set; }
    public string? Company { get; set; }
}
