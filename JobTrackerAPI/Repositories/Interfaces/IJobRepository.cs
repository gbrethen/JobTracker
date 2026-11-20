using JobTrackerAPI.Models.DTOs;
using JobTrackerAPI.Models.Entities;

namespace JobTrackerAPI.Repositories.Interfaces;

public interface IJobRepository : IRepository<Job>
{
    Task<IReadOnlyList<Job>> GetJobsWithDetailsAsync(JobFilterParameters? filter = null);
    Task<Job?> GetJobWithDetailsAsync(int id);
}
