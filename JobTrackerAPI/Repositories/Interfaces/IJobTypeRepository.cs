using JobTrackerAPI.Models.Entities;

namespace JobTrackerAPI.Repositories.Interfaces;

public interface IJobTypeRepository : IRepository<JobType>
{
    Task<JobType?> GetByNameAsync(string typeName);
    Task<bool> ExistsAsync(int id);
}
