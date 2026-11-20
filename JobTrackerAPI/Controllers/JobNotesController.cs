using Microsoft.AspNetCore.Mvc;
using JobTrackerAPI.Models.DTOs;
using JobTrackerAPI.Models.Entities;
using JobTrackerAPI.Repositories.Interfaces;

namespace JobTrackerAPI.Controllers;

[ApiController]
public class JobNotesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public JobNotesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Gets all notes for a specific job application.
    /// </summary>
    [HttpGet("api/jobs/{jobId:int}/notes")]
    [ProducesResponseType(typeof(IEnumerable<JobNoteResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<JobNoteResponseDto>>> GetNotesByJobId(int jobId)
    {
        var jobExists = await _unitOfWork.Jobs.GetByIdAsync(jobId);
        if (jobExists == null)
        {
            return NotFound(new { message = $"Job with ID {jobId} not found." });
        }

        var notes = await _unitOfWork.JobNotes.GetNotesByJobIdAsync(jobId);
        var result = notes.Select(n => new JobNoteResponseDto
        {
            JobNoteId = n.JobNoteId,
            JobId = n.JobId,
            NoteText = n.NoteText,
            CreatedAt = n.CreatedAt
        });

        return Ok(result);
    }

    /// <summary>
    /// Adds a new note to a job application.
    /// </summary>
    [HttpPost("api/jobs/{jobId:int}/notes")]
    [ProducesResponseType(typeof(JobNoteResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobNoteResponseDto>> AddNote(int jobId, [FromBody] JobNoteCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var job = await _unitOfWork.Jobs.GetByIdAsync(jobId);
        if (job == null)
        {
            return NotFound(new { message = $"Job with ID {jobId} not found." });
        }

        var note = new JobNote
        {
            JobId = jobId,
            NoteText = dto.NoteText.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.JobNotes.AddAsync(note);
        await _unitOfWork.CompleteAsync();

        var response = new JobNoteResponseDto
        {
            JobNoteId = note.JobNoteId,
            JobId = note.JobId,
            NoteText = note.NoteText,
            CreatedAt = note.CreatedAt
        };

        return CreatedAtAction(nameof(GetNoteById), new { id = note.JobNoteId }, response);
    }

    /// <summary>
    /// Gets a note by its ID.
    /// </summary>
    [HttpGet("api/notes/{id:int}")]
    [ProducesResponseType(typeof(JobNoteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobNoteResponseDto>> GetNoteById(int id)
    {
        var note = await _unitOfWork.JobNotes.GetByIdAsync(id);
        if (note == null)
        {
            return NotFound(new { message = $"Note with ID {id} not found." });
        }

        return Ok(new JobNoteResponseDto
        {
            JobNoteId = note.JobNoteId,
            JobId = note.JobId,
            NoteText = note.NoteText,
            CreatedAt = note.CreatedAt
        });
    }

    /// <summary>
    /// Updates an existing note.
    /// </summary>
    [HttpPut("api/notes/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateNote(int id, [FromBody] JobNoteUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var note = await _unitOfWork.JobNotes.GetByIdAsync(id);
        if (note == null)
        {
            return NotFound(new { message = $"Note with ID {id} not found." });
        }

        note.NoteText = dto.NoteText.Trim();
        _unitOfWork.JobNotes.Update(note);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }

    /// <summary>
    /// Deletes a specific note.
    /// </summary>
    [HttpDelete("api/notes/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteNote(int id)
    {
        var note = await _unitOfWork.JobNotes.GetByIdAsync(id);
        if (note == null)
        {
            return NotFound(new { message = $"Note with ID {id} not found." });
        }

        _unitOfWork.JobNotes.Delete(note);
        await _unitOfWork.CompleteAsync();

        return NoContent();
    }
}
