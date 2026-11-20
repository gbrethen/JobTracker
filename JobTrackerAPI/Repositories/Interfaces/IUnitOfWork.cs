namespace JobTrackerAPI.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IJobRepository Jobs { get; }
    IJobNotesRepository JobNotes { get; }
    IJobTypeRepository JobTypes { get; }
    IJobLocationRepository JobLocations { get; }

    Task<int> CompleteAsync();
}
