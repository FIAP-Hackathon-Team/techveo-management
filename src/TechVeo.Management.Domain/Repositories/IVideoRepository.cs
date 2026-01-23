using TechVeo.Management.Domain.Entities;

namespace TechVeo.Management.Domain.Repositories;

public interface IVideoRepository
{
    Task<Guid> AddAsync(Video management);
    Task<Video?> GetByIdAsync(Guid id);
    Task<List<Video>> GetByUserIdAsync(Guid id);
}
