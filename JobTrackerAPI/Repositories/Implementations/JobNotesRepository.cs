using Microsoft.EntityFrameworkCore;
using JobTrackerAPI.Data;
using JobTrackerAPI.Models.Entities;
using JobTrackerAPI.Repositories.Interfaces;

namespace JobTrackerAPI.Repositories.Implementations;

public class JobNotesRepository : Repository<JobNote>, IJobNotesRepository
{
    public JobNotesRepository(JobTrackingDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<JobNote>> GetNotesByJobIdAsync(int jobId)
    {
        return await _context.JobNotes
            .Where(n => n.JobId == jobId)
            .OrderByDescending(n => n.CreatedAt)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<JobNote?> GetNoteByIdAndJobIdAsync(int noteId, int jobId)
    {
        return await _context.JobNotes
            .FirstOrDefaultAsync(n => n.JobNoteId == noteId && n.JobId == jobId);
    }
}
