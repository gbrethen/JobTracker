using Microsoft.AspNetCore.Mvc;
using JobTrackerAPI.Models.DTOs;
using JobTrackerAPI.Models.Entities;
using JobTrackerAPI.Repositories.Interfaces;

namespace JobTrackerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<JobsController> _logger;

    public JobsController(IUnitOfWork unitOfWork, ILogger<JobsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Gets all jobs with optional filtering by status, job type, location, or search keyword.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<JobResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<JobResponseDto>>> GetAll([FromQuery] JobFilterParameters filter)
    {
        var jobs = await _unitOfWork.Jobs.GetJobsWithDetailsAsync(filter);

        var result = jobs.Select(j => new JobResponseDto
        {
            JobId = j.JobId,
            Company = j.Company,
            JobTitle = j.JobTitle,
            JobTypeId = j.JobTypeId,
            JobTypeName = j.JobType?.TypeName ?? string.Empty,
            JobLocationId = j.JobLocationId,
            JobLocationType = j.JobLocation?.LocationType ?? string.Empty,
            PhysicalLocation = j.PhysicalLocation,
            ApplicationDate = j.ApplicationDate,
            Status = j.Status,
            SalaryRange = j.SalaryRange,
            JobUrl = j.JobUrl,
            ContactPerson = j.ContactPerson,
            ContactEmail = j.ContactEmail,
            CreatedAt = j.CreatedAt,
            UpdatedAt = j.UpdatedAt,
            NotesCount = j.JobNotes?.Count ?? 0
        });

        return Ok(result);
    }

    /// <summary>
    /// Gets a specific job by ID including all associated notes.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(JobDetailResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobDetailResponseDto>> GetById(int id)
    {
        var job = await _unitOfWork.Jobs.GetJobWithDetailsAsync(id);
        if (job == null)
        {
            return NotFound(new { message = $"Job with ID {id} not found." });
        }

        var result = new JobDetailResponseDto
        {
            JobId = job.JobId,
            Company = job.Company,
            JobTitle = job.JobTitle,
            JobTypeId = job.JobTypeId,
            JobTypeName = job.JobType?.TypeName ?? string.Empty,
            JobLocationId = job.JobLocationId,
            JobLocationType = job.JobLocation?.LocationType ?? string.Empty,
            PhysicalLocation = job.PhysicalLocation,
            ApplicationDate = job.ApplicationDate,
            Status = job.Status,
            SalaryRange = job.SalaryRange,
            JobUrl = job.JobUrl,
            ContactPerson = job.ContactPerson,
            ContactEmail = job.ContactEmail,
            CreatedAt = job.CreatedAt,
            UpdatedAt = job.UpdatedAt,
            NotesCount = job.JobNotes?.Count ?? 0,
            Notes = job.JobNotes?.Select(n => new JobNoteResponseDto
            {
                JobNoteId = n.JobNoteId,
                JobId = n.JobId,
                NoteText = n.NoteText,
                CreatedAt = n.CreatedAt
            }).ToList() ?? new List<JobNoteResponseDto>()
        };

        return Ok(result);
    }

    /// <summary>
    /// Creates a new job application entry.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(JobDetailResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JobDetailResponseDto>> Create([FromBody] JobCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Validate foreign keys
        if (!await _unitOfWork.JobTypes.ExistsAsync(dto.JobTypeId))
        {
            return BadRequest(new { message = $"JobTypeId {dto.JobTypeId} is invalid." });
        }

        if (!await _unitOfWork.JobLocations.ExistsAsync(dto.JobLocationId))
        {
            return BadRequest(new { message = $"JobLocationId {dto.JobLocationId} is invalid." });
        }

        var now = DateTime.UtcNow;
        var job = new Job
        {
            Company = dto.Company.Trim(),
            JobTitle = dto.JobTitle.Trim(),
            JobTypeId = dto.JobTypeId,
            JobLocationId = dto.JobLocationId,
            PhysicalLocation = dto.PhysicalLocation?.Trim(),
            ApplicationDate = dto.ApplicationDate ?? DateOnly.FromDateTime(now),
            Status = string.IsNullOrWhiteSpace(dto.Status) ? "Applied" : dto.Status.Trim(),
            SalaryRange = dto.SalaryRange?.Trim(),
            JobUrl = dto.JobUrl?.Trim(),
            ContactPerson = dto.ContactPerson?.Trim(),
            ContactEmail = dto.ContactEmail?.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        if (!string.IsNullOrWhiteSpace(dto.InitialNote))
        {
            job.JobNotes.Add(new JobNote
            {
                NoteText = dto.InitialNote.Trim(),
                CreatedAt = now
            });
        }

        await _unitOfWork.Jobs.AddAsync(job);
        await _unitOfWork.CompleteAsync();

        // Reload with details for response
        var createdJob = await _unitOfWork.Jobs.GetJobWithDetailsAsync(job.JobId);

        var response = new JobDetailResponseDto
        {
            JobId = createdJob!.JobId,
            Company = createdJob.Company,
            JobTitle = createdJob.JobTitle,
            JobTypeId = createdJob.JobTypeId,
            JobTypeName = createdJob.JobType?.TypeName ?? string.Empty,
            JobLocationId = createdJob.JobLocationId,
            JobLocationType = createdJob.JobLocation?.LocationType ?? string.Empty,
            PhysicalLocation = createdJob.PhysicalLocation,
            ApplicationDate = createdJob.ApplicationDate,
            Status = createdJob.Status,
            SalaryRange = createdJob.SalaryRange,
            JobUrl = createdJob.JobUrl,
            ContactPerson = createdJob.ContactPerson,
            ContactEmail = createdJob.ContactEmail,
            CreatedAt = createdJob.CreatedAt,
            UpdatedAt = createdJob.UpdatedAt,
            NotesCount = createdJob.JobNotes?.Count ?? 0,
            Notes = createdJob.JobNotes?.Select(n => new JobNoteResponseDto
            {
                JobNoteId = n.JobNoteId,
                JobId = n.JobId,
                NoteText = n.NoteText,
                CreatedAt = n.CreatedAt
            }).ToList() ?? new List<JobNoteResponseDto>()
        };

        return CreatedAtAction(nameof(GetById), new { id = job.JobId }, response);
    }

    /// <summary>
    /// Updates an existing job application.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] JobUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var job = await _unitOfWork.Jobs.GetByIdAsync(id);
        if (job == null)
        {
            return NotFound(new { message = $"Job with ID {id} not found." });
        }

        if (!await _unitOfWork.JobTypes.ExistsAsync(dto.JobTypeId))
        {
            return BadRequest(new { message = $"JobTypeId {dto.JobTypeId} is invalid." });
        }

        if (!await _unitOfWork.JobLocations.ExistsAsync(dto.JobLocationId))
        {
            return BadRequest(new { message = $"JobLocationId {dto.JobLocationId} is invalid." });
        }

        job.Company = dto.Company.Trim();
        job.JobTitle = dto.JobTitle.Trim();
        job.JobTypeId = dto.JobTypeId;
        job.JobLocationId = dto.JobLocationId;
        job.PhysicalLocation = dto.PhysicalLocation?.Trim();
        job.ApplicationDate = dto.ApplicationDate;
        job.Status = dto.Status.Trim();
        job.SalaryRange = dto.SalaryRange?.Trim();
        job.JobUrl = dto.JobUrl?.Trim();
        job.ContactPerson = dto.ContactPerson?.Trim();
        job.ContactEmail = dto.ContactEmail?.Trim();
        job.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Jobs.Update(job);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }

    /// <summary>
    /// Updates only the status of an existing job application.
    /// </summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] JobStatusUpdateDto dto)
    {
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(dto.Status))
        {
            return BadRequest(new { message = "Status cannot be empty." });
        }

        var job = await _unitOfWork.Jobs.GetByIdAsync(id);
        if (job == null)
        {
            return NotFound(new { message = $"Job with ID {id} not found." });
        }

        job.Status = dto.Status.Trim();
        job.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Jobs.Update(job);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }

    /// <summary>
    /// Deletes a job application and all associated notes.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var job = await _unitOfWork.Jobs.GetByIdAsync(id);
        if (job == null)
        {
            return NotFound(new { message = $"Job with ID {id} not found." });
        }

        _unitOfWork.Jobs.Delete(job);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }
}
