using JobTrackerAPI.Models.Entities;

namespace JobTrackerAPI.Repositories.Interfaces;

public interface IJobNotesRepository : IRepository<JobNote>
{
    Task<IReadOnlyList<JobNote>> GetNotesByJobIdAsync(int jobId);
    Task<JobNote?> GetNoteByIdAndJobIdAsync(int noteId, int jobId);
}
