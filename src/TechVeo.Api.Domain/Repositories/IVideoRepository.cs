using System;
using System.Threading.Tasks;
using TechVeo.Api.Domain.Entities;

namespace TechVeo.Api.Domain.Repositories;

public interface IVideoRepository
{
    Task<Guid> AddAsync(Video video);

    Task<Video?> GetByIdAsync(Guid id);
}
