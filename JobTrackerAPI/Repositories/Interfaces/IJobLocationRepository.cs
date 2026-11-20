using JobTrackerAPI.Models.Entities;

namespace JobTrackerAPI.Repositories.Interfaces;

public interface IJobLocationRepository : IRepository<JobLocation>
{
    Task<JobLocation?> GetByTypeAsync(string locationType);
    Task<bool> ExistsAsync(int id);
}
