using System.ComponentModel.DataAnnotations;

namespace JobTrackerAPI.Models.DTOs;

public class JobTypeResponseDto
{
    public int JobTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
}

public class JobTypeCreateDto
{
    [Required]
    [StringLength(50)]
    public string TypeName { get; set; } = string.Empty;
}

public class JobLocationResponseDto
{
    public int JobLocationId { get; set; }
    public string LocationType { get; set; } = string.Empty;
}

public class JobLocationCreateDto
{
    [Required]
    [StringLength(50)]
    public string LocationType { get; set; } = string.Empty;
}
