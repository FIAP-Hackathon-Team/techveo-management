using System;
using System.Threading.Tasks;

namespace TechVeo.Management.Domain.Repositories;

public interface IVideoRepository
{
    Task<Guid> AddAsync(Entities.Video management);
    Task<Entities.Video?> GetByIdAsync(Guid id);
    Task<Entities.Video?> GetByUserIdAsync(Guid id);
}