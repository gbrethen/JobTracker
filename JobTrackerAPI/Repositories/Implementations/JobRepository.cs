using Microsoft.EntityFrameworkCore;
using JobTrackerAPI.Data;
using JobTrackerAPI.Models.DTOs;
using JobTrackerAPI.Models.Entities;
using JobTrackerAPI.Repositories.Interfaces;

namespace JobTrackerAPI.Repositories.Implementations;

public class JobRepository : Repository<Job>, IJobRepository
{
    public JobRepository(JobTrackingDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Job>> GetJobsWithDetailsAsync(JobFilterParameters? filter = null)
    {
        var query = _context.Jobs
            .Include(j => j.JobType)
            .Include(j => j.JobLocation)
            .Include(j => j.JobNotes)
            .AsNoTracking()
            .AsQueryable();

        if (filter != null)
        {
            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                query = query.Where(j => j.Status == filter.Status);
            }

            if (filter.JobTypeId.HasValue && filter.JobTypeId.Value > 0)
            {
                query = query.Where(j => j.JobTypeId == filter.JobTypeId.Value);
            }

            if (filter.JobLocationId.HasValue && filter.JobLocationId.Value > 0)
            {
                query = query.Where(j => j.JobLocationId == filter.JobLocationId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Company))
            {
                query = query.Where(j => j.Company.Contains(filter.Company));
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var term = filter.Search.Trim();
                query = query.Where(j => j.Company.Contains(term) || j.JobTitle.Contains(term));
            }
        }

        return await query.OrderByDescending(j => j.ApplicationDate)
                          .ThenByDescending(j => j.JobId)
                          .ToListAsync();
    }

    public async Task<Job?> GetJobWithDetailsAsync(int id)
    {
        return await _context.Jobs
            .Include(j => j.JobType)
            .Include(j => j.JobLocation)
            .Include(j => j.JobNotes.OrderByDescending(n => n.CreatedAt))
            .FirstOrDefaultAsync(j => j.JobId == id);
    }
}
