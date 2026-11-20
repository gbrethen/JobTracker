using JobTrackerAPI.Data;
using JobTrackerAPI.Repositories.Interfaces;

namespace JobTrackerAPI.Repositories.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly JobTrackingDbContext _context;
    private IJobRepository? _jobs;
    private IJobNotesRepository? _jobNotes;
    private IJobTypeRepository? _jobTypes;
    private IJobLocationRepository? _jobLocations;
    private bool _disposed;

    public UnitOfWork(JobTrackingDbContext context)
    {
        _context = context;
    }

    public IJobRepository Jobs => _jobs ??= new JobRepository(_context);
    public IJobNotesRepository JobNotes => _jobNotes ??= new JobNotesRepository(_context);
    public IJobTypeRepository JobTypes => _jobTypes ??= new JobTypeRepository(_context);
    public IJobLocationRepository JobLocations => _jobLocations ??= new JobLocationRepository(_context);

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }
}
