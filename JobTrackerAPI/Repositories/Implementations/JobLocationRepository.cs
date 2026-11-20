using Microsoft.EntityFrameworkCore;
using JobTrackerAPI.Data;
using JobTrackerAPI.Models.Entities;
using JobTrackerAPI.Repositories.Interfaces;

namespace JobTrackerAPI.Repositories.Implementations;

public class JobLocationRepository : Repository<JobLocation>, IJobLocationRepository
{
    public JobLocationRepository(JobTrackingDbContext context) : base(context)
    {
    }

    public async Task<JobLocation?> GetByTypeAsync(string locationType)
    {
        return await _context.JobLocations
            .FirstOrDefaultAsync(l => l.LocationType.ToLower() == locationType.Trim().ToLower());
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.JobLocations.AnyAsync(l => l.JobLocationId == id);
    }
}
