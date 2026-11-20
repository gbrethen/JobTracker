using Microsoft.EntityFrameworkCore;
using JobTrackerAPI.Data;
using JobTrackerAPI.Models.Entities;
using JobTrackerAPI.Repositories.Interfaces;

namespace JobTrackerAPI.Repositories.Implementations;

public class JobTypeRepository : Repository<JobType>, IJobTypeRepository
{
    public JobTypeRepository(JobTrackingDbContext context) : base(context)
    {
    }

    public async Task<JobType?> GetByNameAsync(string typeName)
    {
        return await _context.JobTypes
            .FirstOrDefaultAsync(t => t.TypeName.ToLower() == typeName.Trim().ToLower());
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.JobTypes.AnyAsync(t => t.JobTypeId == id);
    }
}
