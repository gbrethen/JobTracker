using Microsoft.AspNetCore.Mvc;
using JobTrackerAPI.Models.DTOs;
using JobTrackerAPI.Models.Entities;
using JobTrackerAPI.Repositories.Interfaces;

namespace JobTrackerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobLocationsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public JobLocationsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Gets all configured job location types (e.g. On-Site, Remote, Hybrid).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<JobLocationResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<JobLocationResponseDto>>> GetAll()
    {
        var locations = await _unitOfWork.JobLocations.GetAllAsync();
        var result = locations.Select(l => new JobLocationResponseDto
        {
            JobLocationId = l.JobLocationId,
            LocationType = l.LocationType
        });

        return Ok(result);
    }

    /// <summary>
    /// Gets a job location by ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(JobLocationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobLocationResponseDto>> GetById(int id)
    {
        var location = await _unitOfWork.JobLocations.GetByIdAsync(id);
        if (location == null)
        {
            return NotFound(new { message = $"JobLocation with ID {id} not found." });
        }

        return Ok(new JobLocationResponseDto
        {
            JobLocationId = location.JobLocationId,
            LocationType = location.LocationType
        });
    }

    /// <summary>
    /// Creates a new job location type.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(JobLocationResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JobLocationResponseDto>> Create([FromBody] JobLocationCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existing = await _unitOfWork.JobLocations.GetByTypeAsync(dto.LocationType);
        if (existing != null)
        {
            return BadRequest(new { message = $"JobLocation '{dto.LocationType}' already exists." });
        }

        var jobLocation = new JobLocation
        {
            LocationType = dto.LocationType.Trim()
        };

        await _unitOfWork.JobLocations.AddAsync(jobLocation);
        await _unitOfWork.CompleteAsync();

        var response = new JobLocationResponseDto
        {
            JobLocationId = jobLocation.JobLocationId,
            LocationType = jobLocation.LocationType
        };

        return CreatedAtAction(nameof(GetById), new { id = jobLocation.JobLocationId }, response);
    }
}
