using Microsoft.AspNetCore.Mvc;
using JobTrackerAPI.Models.DTOs;
using JobTrackerAPI.Models.Entities;
using JobTrackerAPI.Repositories.Interfaces;

namespace JobTrackerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobTypesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public JobTypesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Gets all configured job types (e.g. Full Time, Part Time, Contract, Temp).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<JobTypeResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<JobTypeResponseDto>>> GetAll()
    {
        var types = await _unitOfWork.JobTypes.GetAllAsync();
        var result = types.Select(t => new JobTypeResponseDto
        {
            JobTypeId = t.JobTypeId,
            TypeName = t.TypeName
        });

        return Ok(result);
    }

    /// <summary>
    /// Gets a job type by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(JobTypeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobTypeResponseDto>> GetById(int id)
    {
        var type = await _unitOfWork.JobTypes.GetByIdAsync(id);
        if (type == null)
        {
            return NotFound(new { message = $"JobType with ID {id} not found." });
        }

        return Ok(new JobTypeResponseDto
        {
            JobTypeId = type.JobTypeId,
            TypeName = type.TypeName
        });
    }

    /// <summary>
    /// Creates a new job type.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(JobTypeResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JobTypeResponseDto>> Create([FromBody] JobTypeCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existing = await _unitOfWork.JobTypes.GetByNameAsync(dto.TypeName);
        if (existing != null)
        {
            return BadRequest(new { message = $"JobType '{dto.TypeName}' already exists." });
        }

        var jobType = new JobType
        {
            TypeName = dto.TypeName.Trim()
        };

        await _unitOfWork.JobTypes.AddAsync(jobType);
        await _unitOfWork.CompleteAsync();

        var response = new JobTypeResponseDto
        {
            JobTypeId = jobType.JobTypeId,
            TypeName = jobType.TypeName
        };

        return CreatedAtAction(nameof(GetById), new { id = jobType.JobTypeId }, response);
    }
}
